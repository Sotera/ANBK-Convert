var http = require('http');

exports.httpGet = function (url, cb, tag) {
  var optionsget = {
    host: 'localhost',
    port: 8777,
    path: url,
    method: 'GET'
  };

  var reqGet = http.request(optionsget, function (res) {
    //console.log("statusCode: ", res.statusCode);
    //console.log("headers: ", res.headers);

    res.setEncoding('utf8');
    res.resultBuffer = '';
    res.on('data', function (data) {
      res.resultBuffer += data;
    });
    res.on('end', function () {
      cb(res.resultBuffer, tag);
    });
  });

  reqGet.end();
  reqGet.on('error', function (e) {
    console.error(e);
  });
}

exports.httpPost = function (url, postBuffer, cb) {
  var postHeaders = {
    'Content-Type': 'application/xml'
  };

  var optionsget = {
    host: 'localhost',
    port: 8777,
    path: url,
    method: 'POST',
    headers: postHeaders
  };

  var reqPost = http.request(optionsget, function (res) {
    //console.log("statusCode: ", res.statusCode);
    //console.log("headers: ", res.headers);

    res.setEncoding('utf8');
    res.resultBuffer = '';
    res.on('data', function (data) {
      res.resultBuffer += data;
    });
    res.on('end', function () {
      cb(res.resultBuffer);
    });
  });

  reqPost.write(postBuffer);
  reqPost.end();
  reqPost.on('error', function (e) {
    console.error(e);
  });
}
