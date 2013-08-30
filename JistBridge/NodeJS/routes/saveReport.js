var netHelpers = require('../utilExports/netHelpers')
    , querystring = require('querystring')
    , fs = require('fs');

exports.doVerb = function (req, res) {
    netHelpers.getPostBuffer(req, function (postBuffer) {
        var report = eval("(" + postBuffer.utf8Data + ')'); //querystring.parse(postBuffer.utf8Data);
        var outputFilename = '/tmp/' + report.metadata.resourceId + '.json';
        fs.writeFile(outputFilename, JSON.stringify(report, null, 4), function(err) {
            if(err) {
                console.log(err);
            } else {
                console.log("JSON saved to ");
            }
        });

        var response = {
            resultCode:1,
            description:'Success',
            resourceId:report.metadata.resourceId
        };
        res.setHeader('Content-Type','application/json');
        res.end(JSON.stringify(response));
    });
};
