/// <reference path="../../Scripts/signals.js" />
/// <reference path="../fx/namespace.js" />
/// <reference path="../fx/syrah.js" />
/// <reference path="../rR.app.js" />
(function (sy, rR, undefined) {
    "use strict";

    // Models
    function LoginViewModel() {
        var self = this;

        // Fields
        self.showRegister = ko.observable(false);
        self.displayName = ko.observable('').required();
        self.email = ko.observable('').required();
    };
    LoginViewModel.prototype = new syrah.DialogViewModel();


    // Set up the module
    var auth = rR.module('auth', function () {
        var self = this;

        self.attached.add(function () {
            sy.bus.janrain.ready.subscribe(function () {
                janrain.events.onProviderLoginToken.addHandler(function (tokenResponse) {
                    $.post(
                        '~/api/v1/sessions/new',
                        { authToken: tokenResponse.token })
                        .success(function (data) {
                            // Grab the rest of the user data and send it to the app level login action
                            sy.bus.auth.setToken.publish(data.user, data.tokens.session, data.tokens.persistent);

                            // Dismiss the dialog
                            janrain.engage.signin.modal.close();
                        })
                        .statusCode({
                            400: function (data) {
                                alert('Need moar data! Still need: ' + data.join(","));
                            }
                        });
                });
            });
        });

        self.action('login', function () {
            this.showDialog('login');
        });
    });
})(syrah, rR);