var netHelpers = require('../utilExports/netHelpers')
    , querystring = require('querystring')
    , fs = require('fs');

exports.doVerb = function (req, res) {
    netHelpers.getPostBuffer(req, function (postBuffer) {
        var report = postBuffer.utf8Data.split('$');

        var resourceId = report[2].split('=')[1];
        var fileName = '/tmp/' + resourceId + '.json';

        fs.writeFile(fileName, postBuffer.utf8Data, function(err) {
            if(err) {
                console.log(err);
            } else {
                console.log("JSON saved to ");
            }
        });

        var response = {
            resultCode:1,
            description:'Success',
            resourceId:resourceId
        };
        res.setHeader('Content-Type','application/json');
        res.end(JSON.stringify(response));
    });
};
