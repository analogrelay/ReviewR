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

    function iteration(init) {
        init = init || {};
        var self = rR.models.model();

        // Fields
        self.id = ko.observable(init.id);
        self.order = ko.observable(init.order || 0);
        self.description = ko.observable(init.description || '');
        self.active = ko.observable(false);

        self.activate = function () {
            if (!self.active()) {
                if (view.activeIteration()) {
                    view.activeIteration().active(false);
                }
                self.active(true);
            }
        }

        self.remove = function () {
            $.ajax({
                url: '~/api/iterations/' + self.id(),
                type: 'delete',
                statusCode: {
                    404: function () {
                        rR.utils.fail('todo: tell user no such review');
                        rR.bus.navigate.publish('');
                    },
                    403: function () {
                        rR.utils.fail("todo: tell user they aren't on this review");
                        rR.bus.navigate.publish('');
                    },
                    200: function (data) {
                        // Calculate next order based on current numbers
                        var nextOrder = self.order() > 0 ? self.order() - 1 : self.order() + 1;
                        var newActive = view.iterations()[nextOrder];
                        view.iterations.remove(self);
                        if (newActive) {
                            newActive.active(true);
                        }
                    }
                }
            });
        }

        return self;
    }

    var view = (function () {
        var self = rR.models.page();

        self.id = ko.observable();
        self.description = ko.observable('');
        self.iterations = ko.observableArray([]);
        self.participants = ko.observableArray([]);
        self.title = ko.observable('');
        self.author = ko.observable();
        self.activeIteration = ko.computed(function () {
            return ko.utils.arrayFirst(self.iterations(), function (item) { return item.active(); });
        });

        self.iterations.subscribe(function() {
            // Reorder
            for(var i = 0; i < self.iterations().length; i++) {
                self.iterations()[i].order(i);
            }
        });

        self.opened.add(function () {
            var id = rR.app.currentParams.id;
            $.ajax({
                url: '~/api/reviews/' + id,
                type: 'get',
                statusCode: {
                    401: function () {
                        rR.utils.fail('todo: prompt user for login');
                        rR.bus.navigate.publish('');
                    },
                    403: function () {
                        rR.utils.fail("todo: tell user they aren't on this review");
                        rR.bus.navigate.publish('');
                    },
                    200: function (data) {
                        self.id(data.id);
                        self.title(data.title);
                        self.description(data.description);
                        self.author(rR.models.user(data.author));
                        self.iterations.removeAll();
                        for (var i = 0; i < data.iterations.length; i++) { self.iterations.push(iteration(data.iterations[i])); }
                        if (self.iterations().length > 0) { self.iterations()[0].active(true); }
                        //self.participants.removeAll();
                        //for(var i = 0; i < data.participants.length; i++) { self.participants.push(rR.models.participant(data.participants[i])); }
                    }
                }
            });
        });

        self.newIteration = function () {
            // Add a new iteration
            $.ajax({
                url: '~/api/iterations',
                type: 'post',
                data: { reviewId: self.id() },
                statusCode: {
                    403: function () {
                        rR.utils.fail("todo: tell user they aren't on this review");
                        rR.bus.navigate.publish('');
                    },
                    201: function (data) {
                        var lastIter = self.iterations()[self.iterations().length - 1];
                        var order = lastIter ? lastIter.order() + 1 : 0;
                        var iter = iteration({
                            id: data.id,
                            order: order
                        });
                        self.iterations.push(iter);
                        iter.activate();
                    }
                }
            });
        };

        return self;
    })();

    rR.publish('vm.reviews', {
        create: create,
        view: view
    });
})(window);