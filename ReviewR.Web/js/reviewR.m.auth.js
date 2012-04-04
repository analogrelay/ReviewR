/// <reference path="reviewR.utils.js" />
/// <reference path="reviewR.js" />

// reviewR.m.auth.js
// Authentication and login code

if (!window.rR) {
    throw 'reviewR.js must be imported before reviewR.m.auth.js';
}

(function (rR) {
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

        // Operations
        self.register = function () {
            alert('registering ' + self.displayName());
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