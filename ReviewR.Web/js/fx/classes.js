var classes = (function (undefined) {
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

    function mix(obj, mixin) {
        /// <param name="obj" type="Object" />
        /// <param name="mixin" type="Function" />
        mixin.apply(obj, []);
    }

    function mixin(contract, definition) {
        /// <param name="contract" type="Function" />
        /// <param name="definition" type="Function" />
        if (definition === undefined) {
            definition = contract;
            contract = function () { return {}; };
        }

        // vsdoc_only
        definition.apply(contract(), []); // For design-time support.
        // end vsdoc_only
        return function () {
            definition.apply(this, []);
        };
    }

    function namespace(name, definition) {
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

    publishSymbol(exports, 'mixin', mixin);
    publishSymbol(exports, 'mix', mix);
    publishSymbol(exports, 'namespace', namespace);

    return exports;
})();

// Define classes namespace using classes!
classes.namespace('classes', classes);