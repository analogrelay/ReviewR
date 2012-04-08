/// <reference path="rR.app.js" />
/// <reference path="rR.utils.js" />
/// <reference path="rR.models.js" />

// rR.vm.reviews.js
// Review management
(function (window, undefined) {
    "use strict";

    // Modals
    var create = (function () {
        var self = rR.models.dialog();

        self.title = ko.observable('').required('Title is required');
        self.description = ko.observable('');

        ko.validation.validatableModel(self);

        self.createReview = function () {
            self.validate();
            if (self.isValid()) {
                // Post a new user
                $.ajax({
                    url: '~/api/reviews',
                    type: 'post',
                    data: { title: self.title(), description: self.description() },
                    statusCode: {
                        401: function () {
                            self.customError("You aren't logged in! How did that happen?");
                        },
                        400: function () {
                            self.customError('Whoops, there were some errors :(');
                        },
                        500: function () {
                            self.customError('Uurp... something bad happened on the server.');
                        },
                        201: function (data) {
                            if (!data.id) {
                                self.customError('Uurp... something bad happened on the server.');
                            } else {
                                rR.bus.navigate.publish('/reviews/' + data.id);
                            }
                        }
                    },
                    complete: function () { self.loading(false); }
                });
            }
        }

        return self;
    })();

    var view = (function () {
        var self = rR.models.page();

        self.id = ko.observable();

        self.opened.add(function () {
            self.id(rR.app.currentParams.id);
        });

        return self;
    })();

    rR.publish('vm.reviews', {
        create: create,
        view: view
    });
})(window);