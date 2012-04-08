/// <reference path="rR.js" />
/// <reference path="rR.app.js" />
// rR.models.js
// Top-level view models
(function (window) {
    "use strict";

    function model(self) {
        self = self || {};

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
                    if (ko.isWriteableObservable(val)) {
                        var value = val();
                        if (typeof value === "boolean") {
                            val(false);
                        } else if (typeof value === "string") {
                            val('');
                        } else {
                            val(null);
                        }
                    }
                    if (ko.isObservable(val) && val.validating) {
                        val.validating(false);
                    }
                }
            }
        }
        return $.extend(self, {
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
        var self = model();
        
        // Fields
        self.id = ko.observable(init.id || 0);
        self.email = ko.observable(init.email || '');
        self.emailHash = ko.observable(init.emailHash || '');
        self.displayName = ko.observable(init.displayName || '');
        self.roles = ko.observableArray(init.roles || []);
        self.loggedIn = ko.observable(init.loggedIn || false);
        self.isAdmin = ko.computed(function () {
            return $.inArray('Admin', self.roles()) > -1;
        });
        self.gravatarUrl = ko.computed(function () {
            return 'http://www.gravatar.com/avatar/' + self.emailHash() + '?s=16';
        });

        return self;
    }

    // System view model
    function application(init) {
        init = init || {};
        var self = model();

        if (!init.loginDialog) {
            throw 'Must provide loginDialog property in parameter object to system';
        }
        if (!init.registerDialog) {
            throw 'Must provide registerDialog property in parameter object to system';
        }

        // Fields
        self.environment = ko.observable(init.environment || '');
        self.currentUser = ko.observable(init.currentUser || user());
        self.activePage = ko.observable(init.activePage);
        self.activeDialog = ko.observable(init.activeDialog || '');
        
        // Computed Properties
        self.isDev = ko.computed(function () { self.environment() === 'Development'; });
        self.isTest = ko.computed(function () { self.environment() === 'Test'; });
        self.isProd = ko.computed(function () { self.environment() === 'Production'; });
        self.isStage = ko.computed(function () { self.environment() === 'Staging'; });

        // Actions
        self.showLogin = function () {
            self.activeDialog(init.loginDialog);
        };

        self.showRegister = function () {
            self.activeDialog(init.registerDialog);
        }

        self.logout = function () {
            rR.bus.logout.publish();
            $.ajax({
                url: '~/api/sessions',
                type: 'delete'
            });
        }
        return self;
    }

    // Page model
    function page(init) {
        init = init || {};
        var self = model(init);

        // Signals
        self.opened = new signals.Signal();
        self.closed = new signals.Signal();

        self.root = ko.observable(rR.app.viewModel);
        self.open = function () { self.opened.dispatch(); }
        self.close = function () {
            if (self._ && self._.reset) {
                self._.reset();
            }
            self.closed.dispatch();
        }
        return self;
    }

    // Dialog model
    function dialog(init) {
        init = init || {};
        var self = page(init);

        // Dismiss dialog on close
        self.closed.add(function () {
            rR.bus.closeDialog.publish();
        });
        return self;
    }

    function review(init) {
        init = init || {};
        var self = {};

        self.id = ko.observable(init.id);
        self.title = ko.observable(init.title);
        self.authorName = ko.observable(init.authorName);
        self.authorEmail = ko.observable(init.authorEmail);
        self.authorEmailHash = ko.observable(init.authorEmailHash);

        self.authorGravatarUrl = ko.computed(function () {
            return 'http://www.gravatar.com/avatar/' + self.authorEmailHash() + '?s=16';
        });

        self.url = ko.computed(function () { return 'reviews/' + self.id(); });

        return self;
    }

    ko.bindingHandlers.page = {
        init: function (element, valueAccessor) {
            return ko.bindingHandlers.template.init(element, function () { return { name: rR.utils.getViewId(ko.utils.unwrapObservable(valueAccessor())) }; });
        },
        update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
            var $element = $(element);
            var val = ko.utils.unwrapObservable(valueAccessor());
            if (!val || val === '') {
                var model = $element.data('model');
                if (model && model.close) { model.close(); }
            } else {
                var model = rR.utils.getModel(val);
                rR.utils.assert(model, "Model not found: '" + val + "'");
                rR.utils.assert(rR.utils.getView(val), "View not found: '" + val + "'");
                if (model && model.open) { model.open(); }

                $element.data('model', model);

                // Set up the template binding
                var templateValueAccessor = function () {
                    return {
                        name: rR.utils.getViewId(val),
                        data: model
                    };
                }

                // Bind the template and show the dialog
                ko.bindingHandlers.template.update(element, templateValueAccessor, allBindingsAccessor, viewModel, bindingContext);
            }
        }
    };

    ko.bindingHandlers.dialog = {
        init: function (element, valueAccessor) {
            /// <param name="element" type="HTMLElement" />
            $(element).modal({ show: false });
            $(element).on('hidden', function () {
                // Update the bound value and dump the DOM contents
                valueAccessor()('');
                ko.utils.emptyDomNode(element);
            });
            return ko.bindingHandlers.page.init(element, valueAccessor);
        },
        update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
            var $element = $(element);
            // How are we being updated?
            var val = ko.utils.unwrapObservable(valueAccessor());
            ko.bindingHandlers.page.update(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
            if (!val || val === '') {
                // Just hide the dialog
                $element.modal('hide');
            } else {
                $element.modal('show');
            }
        }
    };

    // Tweak jQuery ajax:
    var oldjQAjax = $.ajax;
    $.ajax = function () {
        if (arguments[0] && arguments[0].url) {
            arguments[0].url = rR.utils.resolveUrl(arguments[0].url);
        }
        oldjQAjax.apply(this, Array.prototype.slice.call(arguments, 0));
    }

    rR.publish('models', {
        user: user,
        application: application,
        page: page,
        dialog: dialog,
        model: model,
        review: review
    });
})(window);