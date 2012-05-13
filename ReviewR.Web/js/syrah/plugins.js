define(['syrah/utils'], function (utils) {
    "use strict";
    var exports = {};
    exports.Plugin = function (name, baseType) {
        /// <param name="baseType" type="Function" />
        var self = this;
        var base = new baseType();

        function createCurrent() {
            for (var key in self.implementations) {
                if (self.implementations.hasOwnProperty(key) &&
                    self.implementations[key].isAvailable &&
                    self.implementations[key].isAvailable()) {
                        return new self.implementations[key]();
                    }
            }
        };
        baseType.apply(this, [createCurrent]);
        self.implementations = {};

        self.extend = function (name, func, isAvailable) {
            /// <param name="name" type="String" />
            /// <param name="func" type="Function" />
            /// <param name="isAvailable" type="Function" />
            func.prototype = base;
            func.isAvailable = isAvailable;
            self.implementations[name] = func;
        };

        self.setCurrent = function (current) {
            _current = current;
        }
    }

    exports.Proxy = function (currentAccessor) {
        var self = this;
        var _current;
        var _currentAccessor = currentAccessor;

        self.current = function (msg) {
            if (!_current) {
                _current = _currentAccessor();
            }
            return _current;
        }
    }

    exports.add = function (name, baseType) {
        /// <param name="name" type="String" />
        /// <param name="baseType" type="Function" />
        if (syrah.plugins.hasOwnProperty(name)) {
            throw 'Plugin ' + name + ' already exists or conflicts with another member of the syrah.plugins namespace';
        }
        syrah.plugins[name] = new syrah.plugins.Plugin(name, baseType);
    }
});