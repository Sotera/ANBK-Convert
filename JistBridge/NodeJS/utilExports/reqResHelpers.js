var tmp = require('tmp')
  , parseXml = require('xml2js').parseString
  , fsHelpers = require('../utilExports/fsHelpers')
  , inspect = require('eyes').inspector({maxLength: false})
  , fs = require('fs');

exports.inspectReqRes = function (req, res, isXml) {
  var urlDebugString = req.method + ': ' + req.url;
  inspect('Req: ' + urlDebugString);
  req.on('data', function (data, encoding) {
    _inspectGeneric(data.toString('utf8'), isXml);
  });

  /*  if(req.method == 'POST'){
   getPostBuffer(req, function (buffer) {
   util.log('/' + buffer.utf8Data);
   });
   }*/

  res.responseValue = '';
  res.oldWrite = res.write;
  res.write = function (chunk, encoding) {
    if (chunk) {
      res.responseValue += chunk.toString('utf8');
    }
    res.oldWrite(chunk, encoding);
  }
  res.oldEnd = res.end;
  res.end = function (chunk, encoding) {
    if (chunk) {
      res.responseValue += chunk.toString('utf8');
    }
    res.oldEnd(chunk, encoding);
    inspect('Res: ' + urlDebugString);
    _inspectGeneric(res.responseValue, isXml);
  }
}

function _inspectGeneric(val, isXml) {
  if (isXml) {
    fsHelpers.writeToFile(val);
    console.log(val);
    parseXml(val, function (err, parseResult) {
      inspect(parseResult);
      fsHelpers.writeToFile(JSON.stringify(parseResult));
    });
  } else {
    var parseResult = JSON.parse(val);
    inspect(parseResult);
    fsHelpers.writeToFile(val);
  }
}
