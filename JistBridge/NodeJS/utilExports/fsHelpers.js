var tmp = require('tmp')
, fs = require('fs')

var counter = 0;

exports.writeToFile = function(data) {
  setTimeout(function(){
    fs.writeFileSync('c:/tmp/shepherd/tmp-' + pad(counter++, 4) + '.txt', data);
  }, 1000);
/*  tmp.tmpName({dir: 'c:/tmp/shepherd', keep: true, postfix: '.txt'}, function (err, path) {
    fs.writeFileSync(path, data);
  });*/
}

exports.getDataFolder = function (cwd) {
    var fs = require('fs');
    var retVal = require('path').join(cwd, 'data');
    var stats = null
    try {
        stats = fs.lstatSync(retVal);
    } catch (err) {
        console.log('\'' + retVal + '\' does not exist, attempting to create ...');
        try {
            fs.mkdirSync(retVal);
            stats = fs.lstatSync(retVal);
        } catch (err) {
            throw 'Unable to create \'' + retVal + '\'.';
        }
    }

    if (stats.isDirectory()) {
        console.log('\'' + retVal + '\' exists and is writable. Everything is OK.');
    } else {
        throw '\'' + retVal + '\' exists but is either not a directory or is not writable.';
    }

    return retVal;
}

function pad(n, width, z) {
  z = z || '0';
  n = n + '';
  return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
}
