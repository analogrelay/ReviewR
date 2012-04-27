/// <reference path="../../Scripts/signals.js" />
/// <reference path="../fx/namespace.js" />
/// <reference path="../fx/syrah.js" />
/// <reference path="../rR.app.js" />
(function (sy, rR, undefined) {
    "use strict";

    // Set up the module
    var auth = rR.module('auth', function () {
        var self = this;

        self.action('logout', function () {
            $.ajax('~/api/v1/sessions', { type: 'delete' })
                .done(function () {
                    sy.bus.auth.clearToken.publish();
                });
        });

        self.attached.add(function () {
            sy.bus.janrain.ready.subscribe(function () {
                janrain.events.onProviderLoginToken.addHandler(function (tokenResponse) {
                    $.post(
                        '~/api/v1/sessions',
                        { authToken: tokenResponse.token })
                        .success(function (data) {
                            // Grab the rest of the user data and send it to the app level login action
                            sy.bus.auth.setToken.publish(data.user);

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
    });
})(syrah, rR);