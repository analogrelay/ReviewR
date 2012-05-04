/// <reference path="../../Scripts/signals.js" />
/// <reference path="../fx/namespace.js" />
/// <reference path="../fx/syrah.js" />
/// <reference path="../rR.app.js" />
(function (sy, rR, undefined) {
    "use strict";

    function MoreDataViewModel(init) {
        var self = this;
        sy.DialogViewModel.apply(this);

        self.displayName = ko.observable('');
        self.email = ko.observable('');

        self.authToken = init.authToken;
        self.needName = init.needName || false;
        self.needEmail = init.needEmail || false;

        if (self.needName) {
            self.displayName.required('Display Name is required');
        }
        if (self.needEmail) {
            self.email.required('Email Address is required');
        }
        ko.validation.addValidation(self);

        self.submit = function () {
            self.validate();
            if (self.isValid()) {
                $.post(
                        '~/api/v1/sessions',
                        { authToken: self.authToken, email: self.email(), displayName: self.displayName() })
                        .success(function (data) {
                            // Grab the rest of the user data and send it to the app level login action
                            sy.bus.auth.setToken.publish(data.user);

                            // Dismiss the dialog
                            self.close();
                        })
                        .statusCode({
                            400: function (xhr) {
                                // Still not enough data?
                                self.customError('There was a validation error on the server');
                            },
                            409: function () {
                                // Conflict, someone already has this email address
                                self.customError('Someone already has that email address registered!');
                            },
                        });
            }
        }
    }

    // Set up the module
    var auth = rR.module('auth', function () {
        var self = this;

        self.dialog('moreData', MoreDataViewModel);

        self.action('logout', function () {
            $.ajax('~/api/v1/sessions', { type: 'delete' })
                .done(function () {
                    sy.bus.auth.clearToken.publish();
                });
        });

        self.action('moredata', function (authToken, missingFields) {
            this.showDialog('moreData', new MoreDataViewModel({
                authToken: authToken,
                needName: missingFields.indexOf('displayName') > -1, 
                needEmail: missingFields.indexOf('email') > -1
            }));
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
                            400: function (xhr) {
                                var data = JSON.parse(xhr.responseText);
                                janrain.engage.signin.modal.close();
                                sy.bus.exec.publish('auth.moredata', tokenResponse.token, data.missingFields);
                            },
                            500: function () {
                                alert('There was an error processing your login. Please try again!');
                                janrain.engage.signin.modal.close();
                            }
                        });
                });
            });
        });
    });
})(syrah, rR);