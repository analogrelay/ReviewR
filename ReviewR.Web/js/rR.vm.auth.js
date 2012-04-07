/// <reference path="rR.js" />
/// <reference path="rR.app.js" />
/// <reference path="rR.models.js" />
/// <reference path="rR.utils.js" />

// rR.vm.auth.js
// Authentication and login code

(function (window) {
    "use strict";

    // Modals
    var login = (function () {
        var self = rR.models.dialog({});

        // Fields
        self.email = ko.observable('').required('Email address is required');
        self.password = ko.observable('').required('Password is required');
        self.rememberMe = ko.observable(false);

        // Mixin validation helpers
        ko.validation.validatableModel(self);

        // UI State
        self.serverError = ko.observable('');
        self.loading = ko.observable(false);

        self.errorMessage = ko.computed(function () {
            return self.isValid() ? self.serverError() : 'Whoops, there were some errors :(';
        });
        self.hasMessage = ko.computed(function () {
            return !self.isValid() || (self.serverError() && (self.serverError().length > 0));
        });

        // Operations
        self.login = function () {
            self.validate();
            if (self.isValid()) {
                self.loading(true);
                // Post a new user
                $.ajax({
                    url: '~/api/sessions',
                    type: 'post',
                    data: { email: self.email(), password: self.password(), rememberMe: self.rememberMe() },
                    statusCode: {
                        403: function () {
                            self.serverError('Invalid user name or password!');
                        },
                        400: function () {
                            self.serverError('Whoops, there were some errors :(');
                        },
                        500: function () {
                            self.serverError('Uurp... something bad happened on the server.');
                        },
                        201: function (data) {
                            // data contains a user token
                            rR.app.login(data);
                        }
                    },
                    complete: function () { self.loading(false); }
                });
            }
        }
        return self;
    })();

    var register = (function () {
        var self = rR.models.dialog({});

        // Fields
        self.email =
            ko.observable('')
              .required('Email address is required');
        self.displayName =
            ko.observable('')
              .required('Display name is required');
        self.password =
            ko.observable('')
              .required('Password is required');
        self.confirmPassword =
            ko.observable('')
              .required('Password is required')
              .equalTo('You must enter the same password for the confirmation password', self.password);

        // Mixin validation helpers
        ko.validation.validatableModel(self);

        // UI State
        self.serverError = ko.observable('');
        self.loading = ko.observable(false);

        self.errorMessage = ko.computed(function () {
            return self.isValid() ? self.serverError() : 'Whoops, there were some errors :(';
        });
        self.hasMessage = ko.computed(function () {
            return !self.isValid() || (self.serverError() && (self.serverError().length > 0));
        });

        // Operations
        self.register = function () {
            self.validate();
            if (self.isValid()) {
                self.loading(true);
                // Post a new user
                $.ajax({
                    url: '~/api/users',
                    type: 'post',
                    data: { email: self.email(), displayName: self.displayName(), password: self.password(), confirmPassword: self.confirmPassword() },
                    statusCode: {
                        409: function () {
                            self.serverError('There is already a user with that email address!');
                        },
                        400: function () {
                            self.serverError('Whoops, there were some errors :(');
                        },
                        500: function () {
                            self.serverError('Uurp... something bad happened on the server.');
                        },
                        201: function (data) {
                            // data contains a user token
                            rR.app.login(data);
                        }
                    },
                    complete: function () { self.loading(false); }
                });
            }
        }
        return self;
    })();

    rR.publish('vm.auth', {
        login: login,
        register: register
    });
})(window);