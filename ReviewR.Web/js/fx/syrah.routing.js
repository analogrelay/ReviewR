/// <reference path="Backbone.lite.js" />
(function (undefined) {
    classes.namespace('syrah.routing', function (ns) {
        ns.Router = function () {
            Backbone.history || (Backbone.history = new Backbone.History());

            var self = this;
            var _router = new Backbone.Router();

            self.map = function (name, url, handler) {
                _router.route(url, name, handler);
            };

            self.start = function (root) {
                Backbone.history.start({ root: root });
            };

            self.stop = function () {
                Backbone.history.stop();
            }
        };
    });
})();