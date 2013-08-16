module.exports = function () {
    return new CacheHash();
}

var CacheHash = function () {
}

CacheHash.prototype.baseMap = require('gaia-hash')();
CacheHash.prototype.getMap = function (mapName) {
    var map = this.baseMap.get(mapName);
    if (!map) {
        map = require('gaia-hash')();
        this.baseMap.set(mapName, map);
    }
    return map;
}

