var netHelpers = require('../utilExports/netHelpers')
  , parseXml = require('xml2js').parseString
  , nimble = require('nimble')
  , gaiaHash = require('gaia-hash')
  , tmaQuery = require('../utilExports/tmaQuery')
  , dateHelpers = require('../utilExports/dateHelpers')
  , sprintf = require('sprintf-js').sprintf
  , shepherdProxy = require('../utilExports/shepherdProxy');

var useCache = true;
global.geoResponseCache = useCache ? gaiaHash() : null;

exports.doVerb = function (req, res) {
  if(global.proxyToShepherdServer){
    shepherdProxy.proxyRequest(req, res);
    return;
  }

  netHelpers.getPostBuffer(req, function (postBuffer) {
    var whichOnes = req.params.whichOnes;//BOTH,TNG,SHEPHERD
    var sessions = JSON.parse(postBuffer.utf8Data);
    var returnDwellArray = [];
    var perSessionDwellArray = [];
    var dwellsByDwellIdMap = gaiaHash();
    var arrayOfFunctionsToRunInParallel = [];

    if (global.geoResponseCache) {
      var sessionsToFetch = [];
      for (var i = 0, length_i = sessions.length; i < length_i; ++i) {
        var cachedSession = global.geoResponseCache.get(sessions[i]);
        if (cachedSession) {
          returnDwellArray = returnDwellArray.concat(cachedSession);
        } else {
          sessionsToFetch.push(sessions[i]);
        }
      }
      sessions = sessionsToFetch;
    }

    for (i = 0, length_i = sessions.length; i < length_i; ++i) {
      arrayOfFunctionsToRunInParallel.push(function (cb) {
        var session = sessions.shift();
        var ssrId = session.split('-')[2];
        nimble.series([
          function (cb) {
            var xmlPostBuffer = '<?xml version="1.0" encoding="UTF-8" standalone="yes"?>';
            xmlPostBuffer += '<rfdmessage><ssrId>' + ssrId + '</ssrId></rfdmessage>';
            tmaQuery.httpPost('/aoio.tng.services/seam/resource/rest/messages/matching', xmlPostBuffer, function (data) {
              parseXml(data, function (err, parseResult) {
                if (!parseResult || !parseResult.collection) {
                  cb();
                  return;
                }
                var rfdMessages = parseResult.collection.rfdmessage;
                if (!rfdMessages){
                  cb();
                  return;
                }

                for (var j = 0, length_j = rfdMessages.length; j < length_j; ++j) {
                  var rfdMessage = rfdMessages[j];

                  //Construct dwellKey
                  var dwellKeyBase = session;
                  var dwellIdDate = dateHelpers.DateFromSensorNanoTime(parseInt(rfdMessage.dwellID[0]));
                  var dwellId = dwellIdDate.getTime();
                  //var year = parseInt((dwellIdDate.getUTCFullYear() + '').substring(2,4));
                  //var dayOfYear = dateHelpers.DiffInYearsAndDays(new Date(year, 0, 1), dwellIdDate)[1];
                  var year = parseInt(rfdMessage.signalDataStartYear[0]);
                  var dayOfYear = parseInt(rfdMessage.signalDataStartDay[0]);
                  var hours = dwellIdDate.getUTCHours();
                  var minutes = dwellIdDate.getUTCMinutes();
                  var seconds = dwellIdDate.getUTCSeconds();
                  var fractionSeconds = dwellIdDate.getUTCMilliseconds() / 10;
                  var fileFormatTime = sprintf('%02d%03d%02d%02d%02d%02d', year, dayOfYear, hours, minutes, seconds, fractionSeconds);
                  var dwellKey = dwellKeyBase + '-' + fileFormatTime + '-t';

                  //Calculate dwell start & end times
                  var signalDataStartSeconds = parseFloat(rfdMessage.signalDataStartSeconds[0]);
                  var signalDataEndSeconds = parseFloat(rfdMessage.signalDataEndSeconds[0]);
                  var dwellStartTime = dateHelpers.DateFromDwellIdAndSeconds(dwellId, signalDataStartSeconds).getTime();
                  var dwellEndTime = dateHelpers.DateFromDwellIdAndSeconds(dwellId, signalDataEndSeconds).getTime();

                  var signalFrequencyMHz = parseFloat(rfdMessage.signalFreq[0]) / 1e6;
                  signalFrequencyMHz = Math.round(signalFrequencyMHz * 1e4) / 1e4;//need 4 decimal places
                  var collBandwidthKHz = Math.floor(parseFloat(rfdMessage.collectionBW[0]) / 1000);
                  var dwell = {
                    rfdId: rfdMessage.$.id,
                    details: {
                      perSensor: []
                    },
                    dwell: {
                      key: dwellKey,
                      dwellID: dwellId,
                      dwellStartTime: dwellStartTime,
                      dwellEndTime: dwellEndTime,
                      signalFrequencyMHz: signalFrequencyMHz,
                      collBandwidthKHz: collBandwidthKHz
                    },
                    origDwellKey: dwellKey,
                    origDwellID: dwellId,
                    geo: null,
                    truthMarker: null,
                    reproc: null
                  };
                  //Get sensor info started
                  for (var k = 0; k < 6; ++k) {
                    var collectorNodeID = 'collector' + (k + 1) + 'NodeID';
                    if (rfdMessage[collectorNodeID]) {
                      var collector = rfdMessage[collectorNodeID][0];
                      if (collector) {
                        //We'll fill out lat & lon when we go through CRIs, below
                        dwell.details.perSensor.push({sensorID: collector, position: {lat: 0.0, lon: 0.0}});
                      }
                    }
                  }
                  perSessionDwellArray.push(dwell);
                  dwellsByDwellIdMap.set(rfdMessage.dwellID[0], dwell);
                }
                cb();
              })
            });
          },
          function (cb) {
            tmaQuery.httpGet('/aoio.tng.services/seam/resource/rest/messages/crinsv/' + ssrId, function (data) {
              parseXml(data, function (err, parseResult) {
                if (!parseResult) {
                  return;
                }
                //Load up sensor positions from CRI FOV position
                var criMessages = parseResult.collection.crimessage;
                if (criMessages) {
                  for (var i = 0, length_i = criMessages.length; i < length_i; ++i) {
                    var criMessage = criMessages[i];
                    var dwell = dwellsByDwellIdMap.get(criMessage.dwellID[0]);
                    if (dwell) {
                      var sourceId = criMessage.sourceId[0];
                      for (var j = 0, length_j = dwell.details.perSensor.length; j < length_j; ++j) {
                        var perSensor = dwell.details.perSensor[j];
                        if (perSensor.sensorID == sourceId) {
                          perSensor.position.lat = parseFloat(criMessage.fovCenterLat[0]);
                          perSensor.position.lon = parseFloat(criMessage.fovCenterLon[0]);
                          break;
                        }
                      }
                    }
                  }
                }
                //var nsvMessages = parseResult.collection.nsvmessage;
                cb();
              })
            });
          },
          function (cb) {
            var xmlPostBuffer = '<?xml version="1.0" encoding="UTF-8" standalone="yes"?>';
            xmlPostBuffer += '<locmessage><ssrId>' + ssrId + '</ssrId></locmessage>';
            tmaQuery.httpPost('/aoio.tng.services/seam/resource/rest/messages/matchingUnique', xmlPostBuffer, function (data) {
              parseXml(data, function (err, parseResult) {
                if (!parseResult) {
                  return;
                }
                var locMessages = parseResult.collection.locmessage;
                if (locMessages) {
                  for (var i = 0, length_i = locMessages.length; i < length_i; ++i) {
                    var locMessage = locMessages[i];
                    var dwell = dwellsByDwellIdMap.get(locMessage.dwellID[0]);
                    if (dwell) {
                      if (dwell.geo) {//if geo has already been filled in it's AMBIGUOUS and we need to clone a new dwell
                        dwell = JSON.parse(JSON.stringify(dwell));//We can get away with this method of cloning in this case
                        perSessionDwellArray.push(dwell);
                      }
                      dwell.geo = {
                        location: {lat: parseFloat(locMessage.latitude[0]), lon: parseFloat(locMessage.longitude[0])}
                      };

                      //validityTime
                      var dwellId = dateHelpers.DateFromSensorNanoTime(locMessage.dwellID[0]).getTime();
                      var locTimeSeconds = parseFloat(locMessage.locTimeSeconds[0]);
                      dwell.geo.validityTime = dateHelpers.DateFromDwellIdAndSeconds(dwellId, locTimeSeconds).getTime();

                      //LOC ellipse info
                      dwell.geo.semiMajor = parseFloat(locMessage.eepSemiMajorAxis[0]);
                      dwell.geo.semiMinor = parseFloat(locMessage.eepSemiMinorAxis[0]);
                      dwell.geo.orientation = parseFloat(locMessage.eepAxisOrientation[0]);

                      //LOC Freq & Bandwidth info
                      var signalFrequencyMHz = parseFloat(locMessage.signalFreq[0]) / 1e6;
                      signalFrequencyMHz = Math.round(signalFrequencyMHz * 1e6) / 1e6;//need 6 decimal places
                      var collBandwidthKHz = Math.floor(parseFloat(locMessage.collectionBW[0]) / 1000);
                      dwell.geo.frequencyMHz = signalFrequencyMHz;
                      dwell.geo.bandwidthKHz = collBandwidthKHz;

                      //Don't know where to get this from LOC message
                      dwell.geo.extraStatusText = null;

                      //locNumber
                      dwell.geo.locNumber = parseInt(locMessage.locTfmNum[0]);

                      //sensors
                      dwell.geo.sensors = [];
                      for (var j = 0; j < 6; ++j) {
                        var collectorNodeID = 'collector' + (j + 1) + 'NodeID';
                        if (locMessage[collectorNodeID]) {
                          var collector = locMessage[collectorNodeID][0];
                          if (collector) {
                            //We'll fill out lat & lon when we go through CRIs, below
                            dwell.geo.sensors.push(collector);
                          }
                        }
                      }
                      //locationStatuses, validityType, validityBitDescriptions
                      var locStatusVal = parseInt(locMessage.locStatusVal[0]);
                      var locQualifier = parseInt(locMessage.locQualifier[0]);
                      dwell.geo.validityType = 'INVALID';
                      if ((locStatusVal & (1 << 1)) != 0) {
                        if ((locQualifier & (1 << 11)) != 0) {
                          dwell.geo.validityType = 'COMBINED';
                        }
                        else if ((locQualifier & (1 << 1)) != 0) {
                          dwell.geo.validityType = 'SINGLE';
                        }
                      }
                      else if ((locStatusVal & (1 << 2)) != 0) {
                        dwell.geo.validityType = 'AMBIGUOUS';
                      }
                      //
                      dwell.geo.validityBitDescriptions = [];
                      var setBits = whichBitsAreSet(locQualifier);
                      for (var k = 0, length_k = setBits.length; k < length_k; ++k) {
                        dwell.geo.validityBitDescriptions.push(validityStatusBit[setBits[k]]);
                      }
                      dwell.geo.locationStatuses = [];
                      var setBits = whichBitsAreSet(locStatusVal);
                      for (k = 0, length_k = setBits.length; k < length_k; ++k) {
                        dwell.geo.locationStatuses.push(
                          {
                            statusBitDescription: locationStatusBitAndHint[setBits[k]][0],
                            diagnosticHint: locationStatusBitAndHint[setBits[k]][1]
                          }
                        );
                      }
                    }
                  }
                }
                cb();
              })
            });
          }
          /*        function (cb) {
           var xmlPostBuffer = '<?xml version="1.0" encoding="UTF-8" standalone="yes"?>';
           xmlPostBuffer += '<locmessage><ssrId>' + ssrId + '</ssrId></locmessage>';
           tmaQuery.httpPost('/aoio.shepherd.services/seam/resource/rest/shepherd/matchingUnique', xmlPostBuffer, function (data) {
           parseXml(data, function (err, parseResult) {
           if (!parseResult) {
           return;
           }
           })
           });
           },
           function (cb) {
           var xmlPostBuffer = '<?xml version="1.0" encoding="UTF-8" standalone="yes"?>';
           xmlPostBuffer += '<session-dwell><baselineSsrId>' + ssrId + '</session-dwell></baselineSsrId>';
           tmaQuery.httpPost('/aoio.shepherd.services/seam/resource/rest/shepherd/matching/geoinstance', xmlPostBuffer, function (data) {
           parseXml(data, function (err, parseResult) {
           if (!parseResult) {
           return;
           }
           })
           });
           }*/
          , function (cb) {
            if (global.geoResponseCache) {
              global.geoResponseCache.set(session, perSessionDwellArray);
            }
            returnDwellArray = returnDwellArray.concat(perSessionDwellArray);
            perSessionDwellArray = [];
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
        res.end(JSON.stringify(returnDwellArray));
        cb();
      }
    ]);
  });
};

function whichBitsAreSet(bitField) {
  var retVal = [];
  for (var i = 0; i < 64; ++i) {
    if (bitField & 1) {
      retVal.push(i);
    }
    bitField = bitField >> 1;
  }
  return retVal;
}

var validityStatusBit = [
  "(Unused bit)",
  "EEP confidence level satisfied.",
  "No signal detected.",
  "Insufficient data.",
  "Excessive interference present.",
  "Not within bandwidth limits.",
  "Not at cued location.",
  "Not in area of interest.",
  "Image ambiguity resolution failed.",
  "Image ambiguity resolution not possible.",
  "Multiple emitters in location processing.",
  "LOC is a combined geolocation. Defined as combined over time (multiple dwells)."
];

var locationStatusBitAndHint = [
  [ "(Unused bit)", null ],
  [ "Successful geolocation - single solution.", null ],
  [ "Successful geolocation - 2 ambiguous solutions.", null ],
  [ "No solution - iterations failed to converge.", null ],
  [ "No solution - failed SNR threshold.", null ],
  [ "No solution - normalized residual error too large.", null ],
  [ "No solution - geolocation not visible from node.", null ],
  [ "No solution - emitter motion detected. Try TDOA only solution.", null ],
  [ "Error - no frequency band overlap.", "Check Frequency field in each node's CRI" ],
  [ "Error - no time interval overlap.", "Check Start/Stop time for each SIG in dwell" ],
  [ "Error - no archive data found.", "Check that each RFD sent has a matching CRI/SIG/NSV" ],
  [ "Error - geolocation process failed due to internal system error.", null ],
  [ "Error - timeout waiting for geolocation processing.", null ],
  [ "Error - problem with collector data.", null ],
  [ "Error - unable to retrieve all collector data.", "Check that each RFD sent has a matching CRI/SIG/NSV" ],
  [ "Invalid collection platform position.", "Check that there are sufficient PVTs in each node's NSV file." ],
  [ "Invalid collection platform velocity.", "Check that there are sufficient PVTs in each node's NSV file." ],
  [ "Invalid collection platform acceleration.", "Check that there are sufficient PVTs in each node's NSV file." ],
  [ "Collection platform navigation mode - GPS.", null ],
  [ "Collection platform navigation mode - INS.", null ],
  [ "Bias corrected with reference emitter data.", null ],
  [ "Emitter motion detected.", null ]
];
