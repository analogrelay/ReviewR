var namespace = (function (undefined) {
    "use strict";
    var exports = {};

    function publishSymbol(target, name, impl, writable) {
        if (Object.defineProperty) {
            Object.defineProperty(target, name, {
                value: impl,
            });
        } else {
            target[name] = impl;
        }
    }

    function define(name, definition) {
        var ns = {};
        if (typeof definition === "function") {
            var ret = definition(ns);
            if (ret) {
                ns = ret;
            }
        } else {
            ns = definition;
        }

        var parts = name.split('.');
        var current = window;
        for (var i = 0; i < parts.length; i++) {
            if (!current[parts[i]]) {
                current[parts[i]] = {};
            }
            current = current[parts[i]];
        }

        for (var key in ns) {
            if (ns.hasOwnProperty(key)) {
                current[key] = ns[key];
            }
        }
    }

    publishSymbol(exports, 'define', define);

    return exports;
})();

// Define namespace namespace using namespace!
namespace.define('namespace', namespace);