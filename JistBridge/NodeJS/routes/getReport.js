var netHelpers = require('../utilExports/netHelpers')
    , querystring = require('querystring')

exports.doVerb = function (req, res) {
    netHelpers.getPostBuffer(req, function (postBuffer) {
        var userCreds = querystring.parse(postBuffer.utf8Data);
        var response = {
            resultCode:1,
            description:'Success',
            report:{
                metadata:{
                    resourceId:'9333823E-21A6-4738-8F4F-DC06DD9AE9B0',
                    resourceField:'fieldName_resourceId_in_icons',
                    offsetField:'fieldName_offset_of_text_in_icons',
                    textField:'fieldName_selected_text_in_icons',
                    fields:[
                        {fieldName:'fieldName1',fieldValue:'fieldValue1'},
                        {fieldName:'fieldName2',fieldValue:'fieldValue2'},
                        {fieldName:'fieldName3',fieldValue:'fieldValue3'}
                    ]
                },
                texts:[
                    {offset:234,text:'How Now Brown Cow?'},
                    {offset:345,text:'How Brown Now Cow?'},
                    {offset:456,text:'Now Cow Brown Now?'}
                ],
                diagram: '<some binary encoded data>'
            }
        };
        res.end(JSON.stringify(response));
    });
};
