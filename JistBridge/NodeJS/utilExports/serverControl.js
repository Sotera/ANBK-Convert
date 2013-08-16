var httpProxy = require('http-proxy')
    , netHelpers = require('./netHelpers')
    , http = require('http')
    , util = require('util')
    , gaiaHash = require('gaia-hash');

var serverHash = gaiaHash();


exports.startProxyServer = function (proxyDefinition, res) {
    var pd = proxyDefinition;
    pd.cacheHash = gaiaHash();

    var server = httpProxy.createServer(function (req, res, proxy) {
        // All client calls to the proxy come through here. This is where we decide which calls need to be routed
        // to the App server and which to the REST server.

        var restCallRegex = new RegExp(pd.restUrlRegex);
        var path = require('url').parse(req.url).path;
        var regexMatches = restCallRegex.exec(path);
        var isRestCall = (regexMatches != null);

        if (isRestCall) {
                var restCallName = regexMatches[pd.restUrlRegexIdx];
                // weblog.log('Proxying REST call to ' + restCallName);
                // everything is getting passed thru for now

                // TODO : load restOverrides from global.restOverridesHash by IDs

                var overrideObj = null;
                for (var i = 0; i < pd.restOverrideIds.length; i++) {
                    var or = global.restOverrideHash.get(pd.restOverrideIds[i]);
                    if (or == null) {
                        weblog.log('Problem loading restOverride from hash for ID : ' + pd.restOverrideIds[i]);
                        continue;
                    }

                    if ((or.useRegex && new RegExp(or.restCall).exec(restCallName) != null) || (or.restCall == restCallName)) {
                        overrideObj = or;
                        break;
                    }
                }

                if (overrideObj != null) {
                    if (overrideObj.action == 'data') {
                        weblog.log('Overriding with action ' + 'data'.cyan + ' for REST call ' + restCallName);
                        res.end(overrideObj.data);
                    } else if (overrideObj.action == 'cache') {
                        cacheResponse(req, res, pd, overrideObj, restCallName, proxy, null);
                    } else if (overrideObj.action == 'passthru') {
                        weblog.log('Overriding with action ' + 'passthru'.cyan + ' for REST call ' + restCallName);
                        proxy.proxyRequest(req, res, { host:pd.restHostname, port:pd.restPort });
                    } else {
                        weblog.log(("Unable to handle action '" + overrideObj.action + "' for REST call handler '" + overrideObj.restCall + "'").yellow);
                    }

                } else {
                    // no override, just pass-thru
                    weblog.log('Forwarding REST call ' + restCallName);
                    // TODO : send notification to the client if this is the first time this REST call has been made
                    proxy.proxyRequest(req, res, { host:pd.restHostname, port:pd.restPort });
                }

                /*
                switch (res.parseResult.route) {
                    case('login'):
                    {
                        res.oldWrite = res.write;
                        res.write = function (a, b) {
                            if (a.toString('utf8') == 'true') {
                            }
                            res.oldWrite(a, b);
                        }
                        proxy.proxyRequest(req, res, { host:pd.restHostname, port:pd.restPort });
                        break;
                    }
                    case('init'):
                    {
                        res.oldWrite = res.write;
                        res.write = function (a, b) {
                            if (a.toString('utf8') == 'true') {
                                pd.sessionCacheHash.getMap('sessionData').set('jsessionId', netHelpers.getCookies(req)['JSESSIONID']);
                                pd.sessionCacheHash.getMap('sessionData').set('clientId', res.parseResult.clientId);
                            }
                            res.oldWrite(a, b);
                        }
                        proxy.proxyRequest(req, res, { host:pd.restHostname, port:pd.restPort });
                        break;
                    }
                    case('reproc'):
                    {
                        processRequest(req, res, pd, proxy);
                        break;
                    }
                    default:
                    {
                        processRequest(req, res, pd, proxy);
                    }
                }
                */
            } else {
                weblog.log('Forwarding call ' + req.url);
                proxy.proxyRequest(req, res, { host:pd.webappHostname, port:pd.webappPort });
            }
    }).listen(pd.proxyPort);
    serverHash.set(pd.id, server);

    var proxyUrl = 'http://' + pd.proxyHostname + ':' + pd.proxyPort;
    weblog.log('Proxy Server \'' + pd.proxyDefinitionName.green + '\' started on <a href="' + proxyUrl + '" target="_blank" style="color: white;">' + proxyUrl + '</a>');

    //This is response to the browser that started the proxy server
    res.send({
        data:[
        ],
        meta:{
            success:true,
            msg:'Looks Really Good!'
        }
    });
};

exports.stopProxyServer = function(proxyDefinition, res) {
    var pd = proxyDefinition;
    var server = serverHash.get(pd.id);
    if (server != null) {
        server.close(function() {
            weblog.log('Proxy Server \'' + pd.proxyDefinitionName.red + '\' stopped.');
            serverHash.del(pd.id);
            serverHash.clean();
            res.send({
                data:[
                ],
                meta:{
                    success:true,
                    msg:'Looks Really Good!'
                }
            });
        });
    } else {
        weblog.log('Proxy Server \'' + pd.proxyDefinitionName + '\' not found.');
        res.send({
            data:[
            ],
            meta:{
                success:false,
                msg:'Server \'' + pd.proxyDefinitionName + '\' not found.'
            }
        });
    }
};

function cacheResponse(req, res, pd, overrideObj, restCallName, proxy, buffer) {
    res.responseValue = pd.cacheHash.get(overrideObj.id);
    if (res.responseValue) {
        weblog.log('Overriding with action ' + 'cache'.cyan + ' for REST call ' + restCallName + ', returning previously cached response');
        res.end(res.responseValue);
    } else {
        res.responseValue = '';
        weblog.log('Overriding with action ' + 'cache'.cyan + ' for REST call ' + restCallName + ', forwarding call to REST server');
        // capture the response and save it to sessionCacheHash
        res.oldWrite = res.write;
        res.write = function (chunk, encoding) {
            if (chunk) res.responseValue += chunk.toString('utf8');
            res.oldWrite(chunk, encoding);
        }
        res.oldEnd = res.end;
        res.end = function (chunk, encoding) {
            if (chunk) res.responseValue += chunk.toString('utf8');
            res.oldEnd(chunk, encoding);

            weblog.log('Done caching response from server');
            pd.cacheHash.set(overrideObj.id, res.responseValue);
        }
        proxy.proxyRequest(req, res, { host:pd.restHostname, port:pd.restPort, buffer:buffer });
    }
}

function processRequest(req, res, pd, proxy) {
    if (req.method == 'GET') {
        res.key = '/' + res.parseResult.runId;
        cacheResponse(req, res, pd, proxy, null, function (responseValue) {
            var response = JSON.parse(responseValue);
            for (var i = 0, length_i = response.length; i < length_i; ++i) {
                if (response[i].status == 'COMPLETED') {
                    return true;
                }
            }
            return false;
        });
    } else if (req.method == 'POST') {
        // do we need to use the buffer for cache response if not using getPostBuffer() ?
        netHelpers.getPostBuffer(req, function (buffer) {
            res.key = '/' + buffer.utf8Data;
            cacheResponse(req, res, pd, proxy, buffer);
        });
    }
}

