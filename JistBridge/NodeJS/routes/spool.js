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
  shepherdProxy.proxyRequest(req, res);
};

