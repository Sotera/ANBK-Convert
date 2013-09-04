var netHelpers = require('../utilExports/netHelpers')
    , querystring = require('querystring')
    , fs = require('fs')

exports.doVerb = function (req, res) {
    netHelpers.getPostBuffer(req, function (postBuffer) {
        var loginCreds = querystring.parse(postBuffer.utf8Data);
        fs.readFile('data/QueueReport.json', 'utf8', function(err,data){
            res.setHeader('Content-Type','application/json');
            res.end(data);
        })

/*        var response = {
            ValidateUser: {
                roleInfo: [
                    {
                        name: 'AdminRole',
                        poid: 'BO^Intel.Role^bf8d0fa:1405f8b0833:-540a'
                    }
                ],
                description: 'Successful authentication',
                resultCode:0,
                userInfo:{
                    lastName: 'User',
                    poid:'BO^Intel.IntelUser^bf8d0fa:1405f8b0833:-540c',
                    userName:'admin',
                    firstName:'Admin'
                }
            }
 res.setHeader('Content-Type','application/json');
 res.end(JSON.stringify(response));
        };*/
    });
};
