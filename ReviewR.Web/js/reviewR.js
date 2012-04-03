// reviewR.js
// Top-level view models

if (!window.rR) {
    window.rR = {}
}

(function (rR) {
    "use strict";

    // User View model
    function user(source) {
        var exports = {};
        source = source || {};

        // Fields
        exports.id = ko.observable(source.id || 0);
        exports.email = ko.observable(source.email || '');
        exports.displayName = ko.observable(source.displayName || '');
        exports.roles = ko.observableArray(source.roles || []);
        exports.loggedIn = ko.observable(source.loggedIn || false);
        exports.isAdmin = ko.computed(function () {
            return _.indexOf(currentUser.roles(), 'admin') > -1;
        });

        return exports;
    }

    // Page view model
    function page(source) {
        var exports = {};
        source = source || {};

        // Fields
        exports.view = ko.observable(source.view || '');
        exports.model = ko.observable(source.model || {});
        exports.start = ko.observable(source.start);
        exports.templateId = ko.computed(function () { return 't:' + exports.view(); });

        return exports;
    }

    // System view model
    function system(source) {
        var exports = {};
        source = source || {};

        // Fields
        exports.environment = ko.observable(source.environment || '');
        exports.currentUser = ko.observable(source.currentUser || {});
        exports.activePage = ko.observable(source.activePage || page());
        exports.isDev = ko.computed(function () { exports.environment() === 'Development'; });
        exports.isTest = ko.computed(function () { exports.environment() === 'Test'; });
        exports.isProd = ko.computed(function () { exports.environment() === 'Production'; });
        exports.isStage = ko.computed(function () { exports.environment() === 'Staging'; });

        // Actions
        exports.startLogin = function () {
        };

        exports.startRegister = function () {
        }

        return exports;
    }

    $.extend(rR, {
        models: {
            user: user,
            system: system,
            page: page
        }
    });
})(window.rR);