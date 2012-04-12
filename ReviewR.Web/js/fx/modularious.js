/// <reference path="../../Scripts/signals.js" />
/// <reference path="classes.js" />
/// <reference path="Backbone.lite.js" />

// modularious - a system for modular JavaScript application development
(function (undefined) {
    classes.namespace('modularious', function (ns) {
        function moduleHostContract() {
            return {
            };
        };
        ns.module = classes.mixin(moduleHostContract, function () {
            var self = this;
            this.attach = function (app) {
                self.app = app;
            }
        });

        function autoconfig(key) {
            return document.documentElement.getAttribute('data-' + key);
        }

        ns.App = function (rootUrl, environment) {
            var self = this;
            var _rootUrl = rootUrl || autoconfig('root');
            var _environment = environment || autoconfig('environment');
            var _modules = [];

            self.router = new modularious.routing.Router();
            self.mapRoute = self.router.map;
            
            self.module = function (module) {
                _modules.push(module);
            };

            self.start = function () {
                for (var i = 0; i < _modules.length; i++) {
                    _modules[i].attach(self);
                }

                self.router.start(_rootUrl);
            };
        };

        ns.Signal = function () {
        }
        ns.Signal.prototype = new signals.Signal();
        ns.Signal.prototype.asListener = function () {
            var self = this;
            return {
                add: function (listener, listenerContext, priority) { self.add(listener, listenerContext, priority); },
                addOnce: function (listener, listenerContext, priority) { self.addOnce(listener, listenerContext, priority); },
                remove: function (listener, context) { self.remove(listener, context); },
            };
        }
    });

    classes.namespace('modularious.routing', function (ns) {
        ns.Router = function () {
            var self = this;
            var _history = new Backbone.History();
            var _router = new Backbone.Router();

            self.map = function (name, url, handler) {
                _router.route(url, name, handler);
            };

            self.start = function (root) {
                _history.start({ root: root });
            };

            self.stop = function () {
                _history.stop();
            }
        };
    });

    classes.namespace('modularious.rendering', function (ns) {
        var jqTemplateEngine = {
            renderTemplate: function (host, template) { },
            lookupTemplate: function (id) { }
        };

        ns.ViewHost = function (element) {
            var self = this;
            var _hostElement = element;
            var _removing = new modularious.Signal();
            var _injecting = new modularious.Signal();

            self.removing = _removing.asListener();
            self.injecting = _injecting.asListener();

            self.setView = function (name, element) {
            };
        };
    });
})();