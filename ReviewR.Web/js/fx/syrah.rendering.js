/// <reference path="syrah.js" />
/// <reference path="syrah.dom.js" />
(function (undefined) {
    "use strict";
    namespace.define('syrah.rendering', function (ns) {
        ns.View = function (templateId, modelConstructor) {
            var self = this;
            self.injected = new syrah.Signal();
            self.removed = new syrah.Signal();
            self.obscured = new syrah.Signal();
            self.revealed = new syrah.Signal();
            self.templateId = templateId;
            self.modelConstructor = modelConstructor;
        }

        ns.ViewHost = function (element) {
            var self = this;
            var _hostElement = element;
            var _currentView;

            self._injectView = function (name, model) {
                throw 'Override _injectView(name, model)';
            }

            self.obscure = function () {
                if (_currentView && _currentView.obscured) {
                    _currentView.obscured.dispatch();
                }
            }

            self.reveal = function () {
                if (_currentView && _currentView.revealed) {
                    _currentView.revealed.dispatch();
                }
            }

            self.clearView = function () {
                _hostElement.innerHTML = '';
                var old = _currentView;
                _currentView = null;
                if (old && old.removed) {
                    old.removed.dispatch();
                }
            }

            self.setView = function (view, model) {
                /// <param name="view" type="syrah.rendering.View" />
                /// <param name="model" type="Object" />
                self.clearView();

                // Create a view model
                if (!model) { model = new view.modelConstructor(); }
                self._injectView(view.templateId, model);
                _currentView = view;

                if (_currentView && _currentView.injected) {
                    _currentView.injected.dispatch();
                }
            };

            self.initDialog = function () {
                syrah.dom.initDialog(_hostElement, function () {
                    self.clearView();
                });
            }

            self.showDialog = function (view, model) {
                if (view) {
                    self.setView(view, model);
                }
                syrah.dom.showDialog(_hostElement);
            };

            self.closeDialog = function () {
                syrah.dom.closeDialog(_hostElement);
                self.clearView();
            }
        };

        ns.KnockoutViewHost = function (element) {
            /// <param name="element" type="HTMLElement"/>
            var self = this;
            self._injectView = function (name, model) {
                ko.renderTemplate(name, model);
            };
            ns.ViewHost.apply(self, [element]);
        }
    });
})();