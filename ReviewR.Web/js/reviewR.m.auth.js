/// <reference path="reviewR.app.js" />
/// <reference path="reviewR.utils.js" />
/// <reference path="reviewR.js" />

// reviewR.m.auth.js
// Authentication and login code

if (!window.rR) {
    throw 'reviewR.js must be imported before reviewR.m.auth.js';
}

(function (window) {
    "use strict";

    // Modals
    var login = (function () {
        var self = rR.models.modal({});

        // Fields
        self.email = ko.observable('');
        self.password = ko.observable('');

        // Operations
        self.login = function () {
            alert('email:' + self.email() + ';password:' + self.password());
        }
        return self;
    })();

    var register = (function () {
        var self = rR.models.modal({});

        // Fields
        self.email = ko.observable('');
        self.displayName = ko.observable('');
        self.password = ko.observable('');
        self.confirmPassword = ko.observable('');

        self.error = ko.observable('');
        self.loading = ko.observable(false);

        // Operations
        self.register = function () {
            self.loading(true);
            // Post a new user
            $.ajax({
                url: '~/api/users',
                data: { email: self.email(), displayName: self.displayName(), password: self.password(), confirmPassword: self.confirmPassword() },
                statusCode: {
                    409 : function() {
                        self.error('There is already a user with that email address!');
                    },
                    400 : function() {
                        self.error('There were missing or invalid fields!');
                    },
                    200 : function(data) {
                        // data contains a token
                        rR.app.login(data);
                    }
                },
                complete : function() { self.loading(false); }
            }
        }
        return self;
    })();


    $.extend(rR, {
        m: {
            auth: {
                login: login,
                register: register
            }
        }
    });
})(window.rR);