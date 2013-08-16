var httpProxy = require('http-proxy');
var routingProxy = new httpProxy.RoutingProxy();

exports.proxyRequest = function (req, res) {
  routingProxy.proxyRequest(req, res, {
    port: 8070,
    host: 'shepherdnext.ticom-geo.com',
    //host: 'jreeme-dev.ticom-geo.com',
    buffer: httpProxy.buffer(req)
  });
}
