/// <reference path="namespace.js" />
/// <reference path="syrah.plugins.js" />
/// <reference path="syrah.rendering.js" />
(function (undefined) {
    "use strict";

    function KnockoutViewHost(element) {
        /// <param name="element" type="HTMLElement"/>
        var self = this;
        self._injectView = function (name, model) {
            ko.renderTemplate(name, model);
        };
        syrah.rendering.ViewHost.apply(self, [element]);
    }

    function Binder(currentAccessor) {
        var self = this;
        if (currentAccessor) {
            syrah.plugins.Proxy.apply(self, [currentAccessor]);
        }

        self.applyBindings = function (rootElement, model) {
            return self.requireCurrent('applyBindings', 'Override applyBindings(rootElement, model)')
                       .applyBindings(rootElement, model);
        };

        self.applyDescendantBindings = function (rootElement, model) {
            return self.requireCurrent('applyDescendantBindings', 'Override applyDescendantBindings(rootElement, model)')
                       .applyDescendantBindings(rootElement, model);
        };

        self.createViewHost = function (element) {
            return self.requireCurrent('createViewHost', 'Override createViewHost(element)')
                       .createViewHost(element);
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