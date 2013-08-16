var  shepherdProxy = require('../utilExports/shepherdProxy');

exports.doVerb = function (req, res) {
  if(global.proxyToShepherdServer){
    shepherdProxy.proxyRequest(req, res);
    return;
  }
  var clientId = req.params.clientId;
  res.end('true');
};
