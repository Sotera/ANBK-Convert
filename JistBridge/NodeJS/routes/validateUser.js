var netHelpers = require('../utilExports/netHelpers')
    , querystring = require('querystring')

exports.doVerb = function (req, res) {
    netHelpers.getPostBuffer(req, function (postBuffer) {
        var loginCreds = querystring.parse(postBuffer.utf8Data);
        var response = {
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
        };
        res.end(JSON.stringify(response));
    });
};
