/// <reference path="rR.app.js" />
/// <reference path="rR.utils.js" />
/// <reference path="rR.models.js" />

// rR.vm.reviews.js
// Review management
(function (window, undefined) {
    "use strict";

    var fileChangeType = {
        Added: 0,
        Modified: 1,
        Removed: 2
    };

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
                    }
                });
            }
        }

        return self;
    })();

    function line(init) {
        init = init || {};
        var self = rR.models.model();

        // Fields
        self.type = ko.observable(init.type);
        self.text = ko.observable(init.text || '');
        self.index = ko.observable(init.index);
        self.leftLine = ko.observable(init.leftLine);
        self.rightLine = ko.observable(init.rightLine);

        return self;
    }

    function diff(init) {
        init = init || {};
        var self = rR.models.model();

        // Fields
        self.id = ko.observable(init.id);
        self.fileName = ko.observable(init.fileName || '');
        self.deletions = ko.observable(init.deletions);
        self.insertions = ko.observable(init.insertions);
        self.binary = ko.observable(init.binary || false);
        self.lines = ko.observableArray(init.lines || []);

        return self;
    }

    function folder(init) {
        init = init || {};
        var self = rR.models.model();

        // Fields
        self.name = ko.observable(init.name || '');
        self.files = ko.observableArray(init.files || []);

        return self;
    }

    function file(init) {
        init = init || {};
        var self = rR.models.model();

        // Fields
        self.id = ko.observable(init.id);
        self.fileName = ko.observable(init.fileName);
        self.changeType = ko.observable(init.changeType);
        self.diff = ko.observable();
        self.active = ko.computed(function () {
            return (view.activeFile() && view.activeFile().id() === self.id());
        });

        self.activate = function () {
            view.activeFile(self);
            if (!self.diff()) {
                $.ajax({
                    url: '~/api/changes/' + self.id(),
                    type: 'get',
                    statusCode: {
                        404: function () {
                            rR.utils.fail('todo: tell user the iteration does not exist');
                            rR.bus.navigate.publish('');
                        },
                        403: function () {
                            rR.utils.fail("todo: tell user they aren't on this review");
                            rR.bus.navigate.publish('');
                        },
                        200: function (data) {
                            self.diff(diff({
                                id: data.id,
                                fileName: data.fileName,
                                deletions: data.deletions,
                                insertions: data.insertions,
                                binary: data.binary,
                                lines: data.lines.map(function (l) { return line(l) })
                            }));
                        }
                    }
                });
            }
        }

        return self;
    }

    function iteration(init) {
        init = init || {};
        var self = rR.models.model();

        // Fields
        self.id = ko.observable(init.id);
        self.order = ko.observable(init.order || 0);
        self.published = ko.observable(init.published || false);
        self.description = ko.observable(init.description || '');
        self.folders = ko.observableArray([]);
        self.active = ko.computed(function () {
            return new Boolean(view.activeIteration() && view.activeIteration().id() === self.id());
        });

        self.activate = function () {
            view.activeIteration(self);
        }

        self.active.subscribe(function (newValue) {
            if (self.active()) {
                self.refreshFiles();
            }
        });

        self.refreshFiles = function () {
            // Reload files list
            $.ajax({
                url: '~/api/iterations/' + self.id(),
                type: 'get',
                statusCode: {
                    404: function () {
                        rR.utils.fail('todo: tell user the iteration does not exist');
                        rR.bus.navigate.publish('');
                    },
                    403: function () {
                        rR.utils.fail("todo: tell user they aren't on this review");
                        rR.bus.navigate.publish('');
                    },
                    200: function (data) {
                        self.folders.removeAll();
                        for (var i = 0; i < data.length; i++) {
                            self.folders.push(folder({
                                name: data[i].name,
                                files: data[i].files.map(function(f) { return file(f); })
                            }));
                        }
                    }
                }
            });
        };

        self.publish = function () {
            alert('todo: update server');
            self.published(true);
        };

        self.unpublish = function () {
            alert('todo: update server');
            self.published(false);
        };

        self.addFiles = function () {
            rR.bus.showDialog.publish('reviews.upload');
        };

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
                    204: function (data) {
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
        self.activeFile = ko.observable();
        self.activeIteration = ko.observable();

        self.iterations.subscribe(function () {
            // Reorder
            for (var i = 0; i < self.iterations().length; i++) {
                self.iterations()[i].order(i);
            }
        });

        self.opened.add(function () {
            rR.app.viewModel.appBarVisible(true);
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
                        if (self.iterations().length > 0) { self.activeIteration(self.iterations()[0]); }
                        //self.participants.removeAll();
                        //for(var i = 0; i < data.participants.length; i++) { self.participants.push(rR.models.participant(data.participants[i])); }
                    }
                }
            });
        });
        self.closed.add(function () {
            rR.app.viewModel.appBarVisible(false);
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

    var upload = (function () {
        var self = rR.models.dialog();

        self.diff = ko.observable().required('You must specify a diff file');

        ko.validation.validatableModel(self);

        self.upload = function () {
            $.ajax({
                url: '~/api/iterations/' + view.activeIteration().id(),
                type: 'put',
                data: { diff: self.diff() },
                statusCode: {
                    404: function () {
                        rR.utils.fail("todo: tell user that this iteration doesn't exist");
                        rR.bus.closeDialog.publish();
                    },
                    403: function () {
                        rR.utils.fail("todo: tell user they aren't on this review");
                        rR.bus.navigate.publish('');
                    },
                    204: function () {
                        rR.bus.closeDialog.publish();
                        view.activeIteration().refreshFiles();
                    }
                }
            });
        };

        return self;
    })();

    rR.publish('vm.reviews', {
        create: create,
        view: view,
        upload: upload
    });
})(window);