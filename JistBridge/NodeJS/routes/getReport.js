var netHelpers = require('../utilExports/netHelpers')
    , querystring = require('querystring')

exports.doVerb = function (req, res) {
    netHelpers.getPostBuffer(req, function (postBuffer) {
        var userCreds = querystring.parse(postBuffer.utf8Data);
        var response = {
            resultCode: 1,
            description: 'Success',
            report: {
                metadata: {
                    resourceId: '9333823E-21A6-4738-8F4F-DC06DD9AE9B0',
                    resourceField: 'reportKey',
                    offsetField: 'sourceOffset',
                    textField: 'label',
                    fields: {
                        dtg: '2013-05-28T15:25:55',
                        sourceSystem:'CIDNE',
                        analyst:'admin'
                    }
                },
                texts: [
                    {offset: 234, text: 'How Now Brown Cow?'},
                    {offset: 345, text: 'How Brown Now Cow?'},
                    {offset: 456, text: 'Now Cow Brown Now?'}
                ],
                diagram: null
            }
        };
        res.setHeader('Content-Type','application/json');
        res.end(JSON.stringify(response));
    });
};
