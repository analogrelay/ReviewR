/// <reference path="namespace.js" />
(function (undefined) {
    "use strict";
    namespace.define('syrah.utils', function (ns) {
        ns.update = function (source, modified) {
            for (var key in modified) {
                if (modified.hasOwnProperty(key) && source.hasOwnProperty(key)) {
                    var current = source[key];
                    if (ko && ko.isWriteableObservable(current)) {
                        current(modified[key]);
                    } else {
                        source[key] = modified[key];
                    }
                }
            }
        };
    });
})();