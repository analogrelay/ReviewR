/// <reference path="rR.models.js" />
// rR.app.js
// Core application code

(function (window, undefined) {
    "use strict";

    namespace.define('rR', function (ns) {
        var _modules = [];
        ns.App = function (rootUrl, environment, pageHost, dialogHost) {
            var self = this;
            syrah.App.apply(self, rootUrl, environment, pageHost, dialogHost);

            // Fields
            self.environment = ko.observable(environment || '');
            self.currentUser = ko.observable(new rR.models.User());

            // Computed Properties
            self.isDev = ko.computed(function () { self.environment() === 'Development'; });
            self.isTest = ko.computed(function () { self.environment() === 'Test'; });
            self.isProd = ko.computed(function () { self.environment() === 'Production'; });
            self.isStage = ko.computed(function () { self.environment() === 'Staging'; });
        };

        ns.start = function (rootUrl, environment, pageHost, dialogHost) {
            rR.app = new rR.App(rootUrl, environment, pageHost, dialogHost);

            for (var i = 0; i < _modules.length; i++) {
                rR.app.module(_modules[i]);
            }
            _modules = null;

            rR.app.start();
        }

        ns.module = function (fn) {
            /// <param name="fn" type="Function" />
            var module = new syrah.Module();
            fn.apply(module);

            if (rR.app) {
                // Inject it right away
                rR.app.module(module);
            }
            _modules.push(module);
        }
    });

    $(function () {
        rR.start();
    });
})(window);