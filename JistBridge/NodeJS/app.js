var express = require('express')
  , xml = require('node-xml')
  , path = require('path')
  , sessionquery = require('./routes/sessionquery')
  , signaldetails = require('./routes/signaldetails')
  , spool = require('./routes/spool')
  , measurements = require('./routes/measurements')
  , curves = require('./routes/curves')
  , reproc = require('./routes/results/reproc')
  , geotable = require('./routes/geotable')
  , init = require('./routes/init')
  , login = require('./routes/login')
  , util = require('util')
  , inspect = require('eyes').inspector({maxLength: false})
  , reqResHelpers = require('./utilExports/reqResHelpers')
  , fs = require('fs')
  , fsHelpers = require('./utilExports/fsHelpers')
  , http = require('http');

// handle all uncaught exceptions
/*process.on('uncaughtException', function (err) {
 util.log('Caught exception: ' + err);
 process.exit(1);
 });*/

global.proxyToShepherdServer = true;
var inspectTmaTraffic = false;
var inspectShepherdTraffic = true;

//If we can't get a writable data folder just bail out
try {
  global.dataFolder = fsHelpers.getDataFolder(__dirname);
} catch (err) {
  console.log('Unable to get data directory, halting.');
  console.log('Error: ' + err);
  process.exit(1);
}

//Create Express instances
var app = global.app = express();
var api = global.api = express();

//Configure 'app' middleware
app.use(express.static(path.join(__dirname, 'webapp')));

//Configure 'api' middleware
api.use(function (req, res, next) {
  if(inspectShepherdTraffic){
    reqResHelpers.inspectReqRes(req, res, false);
  }
  next();
});

//Route any url starting with 'svr' to 'api' express instance
app.all('/svr*', api);
app.all('/spool*', api);

api.all('/svr/re/resources/login', login.doVerb);
api.all('/svr/re/resources/init/:clientId', init.doVerb);
api.all('/svr/re/resources/session-query/:clientId', sessionquery.doVerb);
api.all('/svr/re/resources/signal-details*', signaldetails.doVerb);
api.all('/spool*', spool.doVerb);
api.all('/svr/re/resources/geo-table/:whichOnes', geotable.doVerb);
api.all('/svr/re/resources/measurements/:clientId', measurements.doVerb);
api.all('/svr/re/resources/curves', curves.doVerb);
api.all('/svr/re/resources/results/reproc/:runId', reproc.doVerb);

//Startup proxy server
app.listen(5000, function () {
  util.log('Shepherd server port: 5000');
});

//TMA proxy server code, below vvv
var tmaProxyServer = require('http-proxy').createServer(function (req, res, proxy) {
  if(inspectTmaTraffic){
    reqResHelpers.inspectReqRes(req, res, true);
  }
  proxy.proxyRequest(req, res, { host: 'ms-shephnext.ticom-geo.com', port: 8060 });
});

tmaProxyServer.listen(8777, function () {
  util.log('TMA proxy running on 8777');
});

