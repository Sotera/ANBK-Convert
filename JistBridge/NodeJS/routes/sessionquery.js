var netHelpers = require('../utilExports/netHelpers')
  , parseXml = require('xml2js').parseString
  , tmaQuery = require('../utilExports/tmaQuery')
  , gaiaHash = require('gaia-hash')
  , nimble = require('nimble')
  , dateHelpers = require('../utilExports/dateHelpers')
  , reqResHelpers = require('../utilExports/reqResHelpers')
  , shepherdProxy = require('../utilExports/shepherdProxy');

var useCache = true;
var ssrResponseCache = useCache ? gaiaHash() : null;

exports.doVerb = function (req, res) {
  if(global.proxyToShepherdServer){
    shepherdProxy.proxyRequest(req, res);
    return;
  }

  netHelpers.getPostBuffer(req, function (postBuffer) {
    var clientId = req.params.clientId;
    var sessionQueryParams = JSON.parse(postBuffer.utf8Data);

    //Check-o the cache-o
    if (ssrResponseCache) {
      var cachedResponseJson = ssrResponseCache.get(sessionQueryParams);
      if (cachedResponseJson) {
        res.end(cachedResponseJson);
        return;
      }
    }

    var xmlPostBuffer = '<?xml version="1.0" encoding="UTF-8" standalone="yes"?><ssr-request/>';
    tmaQuery.httpPost('/aoio.tng.services/seam/resource/rest/messages/SSR', xmlPostBuffer, function (data) {
      parseXml(data, function (err, parseResult) {
        if (!parseResult) {
          return;
        }
        var ssrMsgBySessionIdMap = gaiaHash();
        var ssrMsgs = parseResult.collection.ssrmessage;
        if (!ssrMsgs) {
          return;
        }
        for (var i = 0, length_i = ssrMsgs.length; i < length_i; ++i) {
          var ssrMsgArray = ssrMsgBySessionIdMap.get(ssrMsgs[i].sessionID[0]);
          if (!ssrMsgArray) {
            ssrMsgArray = [];
            ssrMsgBySessionIdMap.set(ssrMsgs[i].sessionID[0], ssrMsgArray);
          }
          ssrMsgArray.push(ssrMsgs[i]);
        }
        //Now we have arrays of ssr messages by sessionId, let's parse 'em & pack 'em
        var keys = ssrMsgBySessionIdMap.keys;
        var shepherdRestResponses = [];
        for (i = 0, length_i = keys.length; i < length_i; ++i) {
          ssrMsgArray = ssrMsgBySessionIdMap.get(keys[i]);
          var ssrMsg = ssrMsgArray[0];
          var shepherdRestResponse = {
            requestedSensors: [],
            reprocGeoCount: 0,
            id: '',
            geoCount: 0,
            bandwidthKHz: 0,
            frequencyMHz: 0,
            startTime: 0,
            endTime: 0,
            key: {
              sessionKey: '',
              recordIDs: [],
              sessionID: ''
            }
          };
          //Dates
          shepherdRestResponse.startTime = dateHelpers.DateFromSensorNanoTime(parseInt(ssrMsg.sessionStartTime)).getTime();
          shepherdRestResponse.endTime = dateHelpers.DateFromSensorNanoTime(parseInt(ssrMsg.sessionEndTime)).getTime();
          shepherdRestResponse.frequencyMHz = parseFloat(ssrMsg.collectionEmitterFreq[0]) / 1e6;
          shepherdRestResponse.bandwidthKHz = parseFloat(ssrMsg.collectionBW[0]) / 1e3;

          for (var k = 0; k < 6; ++k) {
            var collectorNodeID = 'collector' + (k + 1) + 'NodeID';
            if (ssrMsg[collectorNodeID]) {
              var collector = ssrMsg[collectorNodeID][0];
              if (collector) {
                shepherdRestResponse.requestedSensors.push(collector);
              }
            }
          }
          shepherdRestResponse.key.recordIDs.push(parseInt(ssrMsg.$.id));
          shepherdRestResponse.key.sessionID = ssrMsg.sessionID;
          var createdDateMillisString = new Date(ssrMsg.createdDate).getTime().toString();
          shepherdRestResponse.id = ssrMsg.sessionID + '-' + createdDateMillisString;
          shepherdRestResponse.key.sessionKey = shepherdRestResponse.id;
          for (var j = 1, length_j = ssrMsgArray.length; j < length_j; ++j) {
            ssrMsg = ssrMsgBySessionIdMap.get(keys[i])[j];
            shepherdRestResponse.key.recordIDs.push(parseInt(ssrMsg.$.id));
          }
          shepherdRestResponses.push(shepherdRestResponse);
        }
        //Use nimble to grab geo counts in parallel
        var arrayOfFunctionsToRunInParallel = [];
        var urls = [];

        for (i = 0, length_i = shepherdRestResponses.length; i < length_i; ++i) {
          var shepherdRestResponse = shepherdRestResponses[i];
          var recordIDs = shepherdRestResponse.key.recordIDs;
          for (j = 0, length_j = recordIDs.length; j < length_j; ++j) {
            urls.push({url: '/aoio.tng.services/seam/resource/rest/messages/count/session/' + recordIDs[j],
              tag: {ssr: shepherdRestResponse, propName: 'geoCount'}});
            urls.push({url: '/aoio.shepherd.services/seam/resource/rest/shepherd/count/session/' + recordIDs[j],
              tag: {ssr: shepherdRestResponse, propName: 'reprocGeoCount'}});
          }
        }
        for (i = 0, length_i = urls.length; i < length_i; ++i) {
          arrayOfFunctionsToRunInParallel.push(function (cb) {
            var url = urls.shift();
            tmaQuery.httpGet(url.url, function (data, tag) {
              parseXml(data, function (err, parseResult) {
                if (!tag.ssr[tag.propName]) {
                  tag.ssr[tag.propName] = parseInt(parseResult.message.$.value);
                }
                cb();
              });
            }, url.tag);
          });
        }
        nimble.series([
          function (cb) {
            //nimble.parallel(arrayOfFunctionsToRunInParallel, cb);
            nimble.series(arrayOfFunctionsToRunInParallel, cb);
          },
          function (cb) {
            var responseJson = JSON.stringify(shepherdRestResponses);

            //Cache results by query params
            if (ssrResponseCache) {
              ssrResponseCache.set(sessionQueryParams, responseJson);
            }

            //Send restResponse to client
            res.end(responseJson);

            cb();
          }
        ]);
      });
    });
  });
};

