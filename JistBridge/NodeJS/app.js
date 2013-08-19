var express = require('express')
    , xml = require('node-xml')
    , path = require('path')
    , validateUser = require('./routes/validateUser')
    , getReport = require('./routes/getReport')
    , saveReport = require('./routes/saveReport')
    , util = require('util')
    , inspect = require('eyes').inspector({maxLength: false})
    , reqResHelpers = require('./utilExports/reqResHelpers')
    , fs = require('fs')
    , fsHelpers = require('./utilExports/fsHelpers')
    , http = require('http');

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
/*api.use(function (req, res, next) {
    reqResHelpers.inspectReqRes(req, res, false);
    next();
});*/

//Route any url starting with 'svr' to 'api' express instance
app.all('/*', api);

api.post('/UserHTTP/ValidateUser', validateUser.doVerb);
api.post('/UserHTTP/GetReport', getReport.doVerb);
api.post('/UserHTTP/SaveReport', saveReport.doVerb);

//Startup proxy server
app.listen(8080, function () {
    util.log('CIDNE server port: 8080');
});

