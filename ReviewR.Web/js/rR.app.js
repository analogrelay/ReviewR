/// <reference path="rR.models.js" />
/// <reference path="fx/syrah.js" />
// rR.app.js
// Core application code

(function (sy, undefined) {
    "use strict";

    namespace.define('rR', function (ns) {
        var _modules = [];
        ns.App = function (rootUrl, environment, pageHost, dialogHost) {
            var self = this;
            sy.App.apply(self, [rootUrl, environment, pageHost, dialogHost]);

            // Fields
            self.environment = ko.observable(environment || '');
            self.currentUser = ko.observable(new rR.models.User());

            // Computed Properties
            self.isDev = ko.computed(function () { self.environment() === 'Development'; });
            self.isTest = ko.computed(function () { self.environment() === 'Test'; });
            self.isProd = ko.computed(function () { self.environment() === 'Production'; });
            self.isStage = ko.computed(function () { self.environment() === 'Staging'; });

            // Top-level commands
            self.logout = function () {
                sy.bus.exec.publish('auth.logout');
            };
            sy.bus.register('auth.setToken').subscribe(function(user, session, persistent) {
                sy.utils.update(self.currentUser(), user);
                self.currentUser().loggedIn(true);
                self.currentUser().serverVerified(true);
                self.currentUser().token = session;

                // Capture the persistent token in local storage
                if(persistent) {
                    window.localStorage.setItem('auth', JSON.stringify({
                        email: self.currentUser().email(),
                        emailHash: self.currentUser().emailHash(),
                        displayName: self.currentUser().displayName(),
                        token: persistent
                    }));
                }
            });
            self.clearToken = function () {
                self.currentUser(new rR.models.User());
                window.localStorage.removeItem('auth');
            };

            // Check for an existing token
            var auth = window.localStorage['auth'];
            if (auth && (auth = JSON.parse(auth))) {
                // Load cached data before going to server
                self.currentUser().loggedIn(true);
                self.currentUser().serverVerified(false);
                self.currentUser().email(auth.email);
                self.currentUser().emailHash(auth.emailHash);
                self.currentUser().displayName(auth.displayName);

                $.post('~/api/v1/sessions/restore', { persistentToken: auth.token })
                 .success(function (data) {
                     sy.bus.auth.setToken.publish(data.user, data.token);
                 })
                .statusCode({
                    404: function () {
                        console.log('Auth token has been revoked or expired, removing');
                        self.clearToken();
                    }
                });
            }
        };

        ns.start = function (rootUrl, environment, pageHost, dialogHost) {
            if (!Modernizr.localstorage) {
                alert('Sorry, this app requires a browser which supports local storage :(');
            }
            rR.app = new rR.App(rootUrl, environment, pageHost, dialogHost);

            for (var i = 0; i < _modules.length; i++) {
                rR.app.module(_modules[i]);
            }
            _modules = null;

            rR.app.start();
        }

        ns.module = function (name, fn) {
            /// <param name="name" type="String" />
            /// <param name="fn" type="Function" />
            var module = new sy.Module(name);
            fn.apply(module);

            if (rR.app) {
                // Inject it right away
                rR.app.module(module);
            }
            _modules.push(module);
            return module;
        }
    });

    $(function () {
        rR.start();
    });
})(syrah);