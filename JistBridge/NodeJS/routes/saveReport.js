var netHelpers = require('../utilExports/netHelpers')
    , querystring = require('querystring')

exports.doVerb = function (req, res) {
    netHelpers.getPostBuffer(req, function (postBuffer) {
        var userCreds = querystring.parse(postBuffer.utf8Data);
        var response = {
            resultCode:1,
            description:'Success'
        };
        res.end(JSON.stringify(response));
    });
};
