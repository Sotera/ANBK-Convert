var netHelpers = require('../utilExports/netHelpers')
  , shepherdProxy = require('../utilExports/shepherdProxy')
  , parseXml = require('xml2js').parseString
  , tmaQuery = require('../utilExports/tmaQuery');

exports.doVerb = function (req, res) {
  if(global.proxyToShepherdServer){
    shepherdProxy.proxyRequest(req, res);
    return;
  }
  //Contact TMA server directly
  netHelpers.getPostBuffer(req, function (postBuffer) {
    var loginCreds = JSON.parse(postBuffer.utf8Data);
    tmaQuery.httpGet('/aoio.tng.services/seam/resource/rest/security/authenticate/' + loginCreds.id, function (data) {
      parseXml(data, function (err, parseResult) {
        //inspect(parseResult);
      });
      res.end('true');
    });
  });
};
