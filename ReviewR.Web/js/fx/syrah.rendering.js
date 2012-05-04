/// <reference path="syrah.plugins.dom.js" />
/// <reference path="syrah.js" />
(function (undefined) {
    "use strict";
    namespace.define('syrah.rendering', function (ns) {
        ns.View = function (templateId, modelConstructor, options) {
            var self = this;
            self.injected = new signals.Signal();
            self.removed = new signals.Signal();
            self.templateId = templateId;
            self.modelConstructor = modelConstructor;
            self.options = options;
        }

        ns.Page = function (templateId, modelConstructor, options) {
            var self = this;
            syrah.rendering.View.apply(this, [templateId, modelConstructor, options]);
            self.obscured = new syrah.Signal();
            self.revealed = new syrah.Signal();
        }

        ns.ViewHost = function (element, dialogMode) {
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
                    _currentView.injected.dispatch(_currentView, model);
                }

                if (model.load && typeof(model.load) === 'function') {
                    model.load();
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
                syrah.plugins.dom.showDialog(_hostElement);
            };

            self.closeDialog = function () {
                syrah.plugins.dom.closeDialog(_hostElement);
            }

            function dialogHidden() {
                self.clearView();
            }

            if (dialogMode) {
                syrah.plugins.dom.initDialog(_hostElement, dialogHidden);
            }
        };
    });
})();