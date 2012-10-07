/// <reference path="namespace.js" />
/// <reference path="syrah.plugins.js" />
/// <reference path="syrah.rendering.js" />
(function (undefined) {
    "use strict";

    function KnockoutViewHost(element) {
        /// <param name="element" type="HTMLElement"/>
        var self = this;
        syrah.rendering.ViewHost.apply(self, [element]);
        self._injectView = function (name, model) {
            ko.renderTemplate('v:' + name, model, {}, element);
        };
    }

    function Binder(currentAccessor) {
        var self = this;
        if (currentAccessor) {
            syrah.plugins.Proxy.apply(self, [currentAccessor]);
        }

        self.applyBindings = function (rootElement, model) {
            if (self.current && self.current().applyBindings) {
                return self.current().applyBindings(rootElement, model);
            } else {
                throw 'Override applyBindings(rootElement, model)';
            }
        };

        self.applyDescendantBindings = function (rootElement, model) {
            if (self.current && self.current().applyDescendantBindings) {
                return self.current().applyDescendantBindings(rootElement, model);
            } else {
                throw 'Override applyDescendantBindings(rootElement, model)';
            }
        };

        self.createViewHost = function (element) {
            if (self.current && self.current().createViewHost) {
                return self.current().createViewHost(element);
            } else {
                throw 'Override createViewHost(element)';
            }
        }
    }
    syrah.plugins.add('binding', Binder);

    syrah.plugins.binding.extend('KnckoutBinder', function () {
        var self = this;

        self.applyBindings = function (rootElement, model) {
            return ko.applyBindings(model, rootElement);
        };

        self.applyDescendantBindings = function (rootElement, model) {
            return ko.applyBindingsToDescendants(model, rootElement);
        };

        self.createViewHost = function (element) {
            return new KnockoutViewHost(element);
        }
    }, function () {
        return ko;
    });
})();