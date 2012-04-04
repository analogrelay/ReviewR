/// <reference path="reviewR.utils.js" />
// reviewR.js
// Top-level view models

if (!window.rR) {
    window.rR = {}
}
(function (rR) {
    "use strict";

    function model(self) {
        function update(obj) {
            for (var key in obj) {
                if (self.hasOwnProperty(key)) {
                    self[key](obj[key]);
                }
            }
        }
        function reset() {
            for (var key in self) {
                if (self.hasOwnProperty(key)) {
                    var val = self[key];
                    if (ko.isObservable(val)) {
                        val(null);
                    }
                }
            }
        }
        return $.extend(self || {}, {
            // Use "_" to separate core methods from actual model methods
            '_': {
                update: update,
                reset: reset
            }
        });
    }

    // User View model
    function user(init) {
        init = init || {};
        var self = {};
        model({});
        
        // Fields
        self.id = ko.observable(init.id || 0);
        self.email = ko.observable(init.email || '');
        self.displayName = ko.observable(init.displayName || '');
        self.roles = ko.observableArray(init.roles || []);
        self.loggedIn = ko.observable(init.loggedIn || false);
        self.isAdmin = ko.computed(function () {
            return _.indexOf(currentUser.roles(), 'admin') > -1;
        });

        return self;
    }

    // Page view model
    function page(init) {
        init = init || {};
        var self = model(init);
        
        // Fields
        self.view = ko.observable(init.view || '');
        self.model = ko.observable(init.model || {});
        self.open = ko.observable(init.open);
        self.close = ko.observable(init.close);
        self.templateId = ko.computed(function () { return rR.utils.getViewId(self.view()); });

        return self;
    }

    // System view model
    function application(init) {
        init = init || {};
        var self = model(init);

        if (!init.loginModal) {
            throw 'Must provide loginModal property in parameter object to system';
        }
        if (!init.registerModal) {
            throw 'Must provide registerModal property in parameter object to system';
        }

        // Fields
        self.environment = ko.observable(init.environment || '');
        self.currentUser = ko.observable(init.currentUser || {});
        self.activePage = ko.observable(init.activePage);
        self.activeModal = ko.observable(init.activeModal || '');
        
        // Computed Properties
        self.isDev = ko.computed(function () { self.environment() === 'Development'; });
        self.isTest = ko.computed(function () { self.environment() === 'Test'; });
        self.isProd = ko.computed(function () { self.environment() === 'Production'; });
        self.isStage = ko.computed(function () { self.environment() === 'Staging'; });

        // Actions
        self.showLogin = function () {
            self.activeModal(init.loginModal);
        };

        self.showRegister = function () {
            self.activeModal(init.registerModal);
        }

        return self;
    }

    // Modal model (confused?)
    function modal(init) {
        init = init || {};
        var self = model(init);

        self.close = function () {
            rR.app.dismissModal();
        }
        return self;
    }

    ko.bindingHandlers.modal = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            /// <param name="element" type="HTMLElement" />
            $(element).modal({ show: false });
            $(element).on('hidden', function () {
                // Update the bound value and dump the DOM contents
                valueAccessor()('');
                ko.utils.emptyDomNode(element);
            });
            return ko.bindingHandlers.template.init(element, function () { return { name: 'm:' + ko.utils.unwrapObservable(valueAccessor()) } });
        },
        update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
            var $element = $(element);
            // How are we being updated?
            var val = ko.utils.unwrapObservable(valueAccessor());
            if (val === '') {
                // Just hide the modal
                var model = $element.data('model');
                if (model && model._ && model._.reset) {
                    model._.reset();
                }
                $element.modal('hide');
            } else {
                var model = rR.utils.getModel(val);
                rR.utils.assert(model, "Modal model not found: '" + val + "'");
                rR.utils.assert(rR.utils.getView(val), "Modal view not found: '" + val + "'");

                $element.data('model', model);

                // Set up the template binding
                var templateValueAccessor = function () {
                    return {
                        name: rR.utils.getViewId(val),
                        data: model
                    };
                }

                // Bind the template and show the modal
                ko.bindingHandlers.template.update(element, templateValueAccessor, allBindingsAccessor, viewModel, bindingContext);
                $element.modal('show');
            }
        }
    };

    $.extend(rR, {
        models: {
            user: user,
            application: application,
            page: page,
            modal: modal
        }
    });
})(window.rR);