define(['backbone.lite'], function (Backbone) {
    "use strict";
    var exports = {};
    exports.Router = function () {
        Backbone.history || (Backbone.history = new Backbone.History());

        var self = this;
        var _router = new Backbone.Router();
        var _currentRoute;

        self.currentRoute = function () {
            return _currentRoute;
        };

        self.refresh = function () {
            _router.refresh();
        }

        self.map = function (name, url, handler) {
            _router.route(url, name, handler);
        };

        self.start = function (root) {
            Backbone.history.start({ root: root, pushState: Modernizr.history });
        };

        self.stop = function () {
            Backbone.history.stop();
        }

        self.navigate = function (url) {
            _router.navigate(url, { trigger: true });
        }
    };
    return exports;
});