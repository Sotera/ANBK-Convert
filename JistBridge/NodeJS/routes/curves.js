var netHelpers = require('../utilExports/netHelpers')
  , dateHelpers = require('../utilExports/dateHelpers')
  , gaiaHash = require('gaia-hash')
  , parseXml = require('xml2js').parseString
  , nimble = require('nimble')
  , tmaQuery = require('../utilExports/tmaQuery')
  , shepherdProxy = require('../utilExports/shepherdProxy');

var useCache = true;
var tfmCache = useCache ? gaiaHash() : null;

exports.doVerb = function (req, res) {
  if(global.proxyToShepherdServer){
    shepherdProxy.proxyRequest(req, res);
    return;
  }

  netHelpers.getPostBuffer(req, function (postBuffer) {
    var clientId = req.params.clientId;
    var dwellIds = JSON.parse(postBuffer.utf8Data);
    var returnTfmArray = [];
    var perDwellIdTfmArray = [];
    var tfmsByDwellIdMap = gaiaHash();
    var arrayOfFunctionsToRunInParallel = [];

    if (tfmCache) {
      var dwellIdsToFetch = [];
      for (var i = 0, length_i = dwellIds.length; i < length_i; ++i) {
        var cachedTfms = tfmCache.get(dwellIds[i]);
        if (cachedTfms) {
          returnTfmArray = returnTfmArray.concat(cachedTfms);
        } else {
          dwellIdsToFetch.push(dwellIds[i]);
        }
      }
      dwellIds = dwellIdsToFetch;
    }

    for (var i = 0, length_i = dwellIds.length; i < length_i; ++i) {
      arrayOfFunctionsToRunInParallel.push(function (cb) {
        var dwellId = dwellIds.shift();
        var dwell = getDwellByDwellId(dwellId);
        var rfdId = dwell.rfdId;
        nimble.series([
          function (cb) {
            var xmlPostBuffer = '<?xml version="1.0" encoding="UTF-8" standalone="yes"?>';
            xmlPostBuffer += '<tfmmessage><rfdId>' + rfdId + '</rfdId><node1Matrix/><node2Matrix/></tfmmessage>';
            tmaQuery.httpPost('/aoio.tng.services/seam/resource/rest/messages/matchingUnique', xmlPostBuffer, function (data) {
              parseXml(data, function (err, parseResult) {
                perDwellIdTfmArray = [];
                if (!parseResult || !parseResult.collection) {
                  cb();
                  return;
                }
                var tfmMessages = parseResult.collection.tfmmessage;
                if(!tfmMessages){
                  cb();
                  return;
                }

                for (var j = 0, length_j = tfmMessages.length; j < length_j; ++j) {
                  var tfmMessage = tfmMessages[j];
                  var validTime = dateHelpers.DateFromDwellIdAndSeconds(dwell.dwell.dwellID, tfmMessage.tfmSeconds[0]).getTime();
                  var tfm = {
                    sequenceNumber: parseInt(tfmMessage.locTfmNum[0]),
                    validTime: validTime,
                    collector1: tfmMessage.collectorNode1[0],
                    collector2: tfmMessage.collectorNode2[0],
                    dwellDurationMillis: parseInt(tfmMessage.dwellDuration[0]),
                    tdoaNanos: parseFloat(tfmMessage.tdoa[0]),
                    fdoaHz: parseFloat(tfmMessage.fdoa[0]),
                    tdoaUncertNanos: parseFloat(tfmMessage.tdoaError[0]),
                    fdoaUncertHz: parseFloat(tfmMessage.fdoaError[0]),
                    snr1dB: parseFloat(tfmMessage.snrCollector1[0]),
                    snr2dB: parseFloat(tfmMessage.snrCollector2[0]),
                    dwell: {
                      key: dwell.dwell.key,
                      dwellID: dwell.dwell.dwellID,
                      dwellStartTime: dwell.dwell.dwellStartTime,
                      dwellEndTime: dwell.dwell.dwellEndTime,
                      signalFrequencyMHz: dwell.dwell.signalFrequencyMHz,
                      collBandwidthKHz: dwell.dwell.collBandwidthKHz
                    },
                    vel1: {
                      x: parseFloat(tfmMessage.node1VelocityX[0]),
                      y: parseFloat(tfmMessage.node1VelocityY[0]),
                      z: parseFloat(tfmMessage.node1VelocityZ[0])
                    },
                    vel2: {
                      x: parseFloat(tfmMessage.node2VelocityX[0]),
                      y: parseFloat(tfmMessage.node2VelocityY[0]),
                      z: parseFloat(tfmMessage.node2VelocityZ[0])
                    },
                    pos1: {
                      xMeters: parseFloat(tfmMessage.node1PositionX[0]),
                      yMeters: parseFloat(tfmMessage.node1PositionY[0]),
                      zMeters: parseFloat(tfmMessage.node1PositionZ[0])
                    },
                    pos2: {
                      xMeters: parseFloat(tfmMessage.node2PositionX[0]),
                      yMeters: parseFloat(tfmMessage.node2PositionY[0]),
                      zMeters: parseFloat(tfmMessage.node2PositionZ[0])
                    }
                  };
                  perDwellIdTfmArray.push(tfm);
                  tfmsByDwellIdMap.set(dwellId, tfm);
                }
                cb();
              });
            });
          },
          function (cb) {
            if (tfmCache) {
              tfmCache.set(dwellId, perDwellIdTfmArray);
            }
            returnTfmArray = returnTfmArray.concat(perDwellIdTfmArray);
            perDwellIdTfmArray = [];
            cb();
          }
        ], cb);
      });
    }

    nimble.series([
      function (cb) {
        //nimble.parallel(arrayOfFunctionsToRunInParallel, cb);
        nimble.series(arrayOfFunctionsToRunInParallel, cb);
      },
      function (cb) {
        res.end(JSON.stringify(returnTfmArray));
        cb();
      }
    ]);
  });
};

function getDwellByDwellId(dwellId) {
  var regExp = new RegExp('(.*)-\\d{13}-.$');
  var matches = dwellId.match(regExp);
  var key = matches[1];
  var dwellArray = global.geoResponseCache.get(key);
  for (var i = 0, length_i = dwellArray.length; i < length_i; ++i) {
    if (dwellArray[i].origDwellKey == dwellId) {
      return dwellArray[i];
    }
  }
}
