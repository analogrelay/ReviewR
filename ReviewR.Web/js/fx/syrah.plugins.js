/// <reference path="syrah.utils.js" />
/// <reference path="namespace.js" />
(function (undefined) {
    "use strict";
    namespace.define('syrah.plugins', function (ns) {
        ns.Plugin = function (name, baseType) {
            /// <param name="baseType" type="Function" />
            var self = this;
            var base = new baseType();

            var _current;
            function getCurrent() {
                if (!_current) {
                    for (var key in self.implementations) {
                        if (self.implementations.hasOwnProperty(key) &&
                            self.implementations[key].isAvailable &&
                            self.implementations[key].isAvailable()) {
                                _current = self.implementations[key];
                                break;
                            }
                    }
                }
                return _current;
            };
            var proxy = new baseType(getCurrent);
            
            baseType.apply(this);
            self.implementations = {};
            
            self.extend = function (name, func, isAvailable) {
                /// <param name="name" type="String" />
                /// <param name="func" type="Function" />
                /// <param name="isAvailable" type="Function" />
                func.prototype = base;
                func.isAvailable = isAvailable;
                self.implementations[name] = func
            };

            self.setCurrent = function (current) {
                _current = current;
            }
        }

        ns.Proxy = function (currentAccessor) {
            var self = this;
            var _current;

            self.current = function (msg) {
                if (!_current) {
                    _current = currentAccessor();
                }
            }

            self.requireCurrent = function (key, msg) {
                if(self.currentSupports(key)) {
                    throw msg;
                }
                return cur;
            }

            self.currentSupports = function(key) {
                var cur = self.current();
                return cur && cur.hasOwnProperty(key);
            }
        }

        ns.add = function (name, baseType) {
            /// <param name="name" type="String" />
            /// <param name="baseType" type="Function" />
            if (syrah.plugins.hasOwnProperty(name)) {
                throw 'Plugin ' + name + ' already exists or conflicts with another member of the syrah.plugins namespace';
            }
            syrah.plugins[name] = new syrah.plugins.Plugin(name, baseType);
        }
    });
})();