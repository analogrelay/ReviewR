/// <reference path="../rR.app.js" />
/// <reference path="../rR.models.js" />

// rR.m.reviews.js
// Review management
(function (sy, rR, undefined) {
    "use strict";

    var fileChangeType = {
        Added: 0,
        Modified: 1,
        Removed: 2
    };

    // Modals
    function DiffLine(init) {
        init = init || {};
        var self = this;

        // Fields
        self.type = ko.observable(init.type);
        self.text = ko.observable(init.text || '');
        self.index = ko.observable(init.index);
        self.leftLine = ko.observable(init.leftLine);
        self.rightLine = ko.observable(init.rightLine);

        return self;
    }

    function Diff(init) {
        init = init || {};
        var self = this;

        // Fields
        self.id = ko.observable(init.id);
        self.fileName = ko.observable(init.fileName || '');
        self.deletions = ko.observable(init.deletions);
        self.insertions = ko.observable(init.insertions);
        self.binary = ko.observable(init.binary || false);
        self.lines = ko.observableArray(init.lines || []);

        return self;
    }

    function Folder(init) {
        init = init || {};
        var self = this;

        // Fields
        self.name = ko.observable(init.name || '');
        self.files = ko.observableArray(init.files || []);

        return self;
    }

    function File(owner, init) {
        init = init || {};
        var self = this;
        var _owner = owner;
        if (window.hasOwnProperty('intellisense')) {
            _owner = new ViewReviewViewModel();
        }

        // Fields
        self.id = ko.observable(init.id);
        self.fileName = ko.observable(init.fileName);
        self.changeType = ko.observable(init.changeType);
        self.diff = ko.observable();
        self.active = ko.computed(function () {
            return (_owner.activeFile() && _owner.activeFile().id() === self.id());
        });

        self.activate = function () {
            view.activeFile(self);
            if (!self.diff()) {
                //$.ajax({
                //    url: '~/api/v1/changes/' + self.id(),
                //    type: 'get',
                //    statusCode: {
                //        404: function () {
                //            rR.utils.fail('todo: tell user the iteration does not exist');
                //            rR.bus.navigate.publish('');
                //        },
                //        403: function () {
                //            rR.utils.fail("todo: tell user they aren't on this review");
                //            rR.bus.navigate.publish('');
                //        },
                //        200: function (data) {
                //            self.diff(diff({
                //                id: data.id,
                //                fileName: data.fileName,
                //                deletions: data.deletions,
                //                insertions: data.insertions,
                //                binary: data.binary,
                //                lines: data.lines.map(function (l) { return line(l) })
                //            }));
                //        }
                //    }
                //});
            }
        }

        return self;
    }

    function Iteration(owner, init) {
        /// <param name="owner" type="ViewReviewViewModel" />
        init = init || {};
        var self = this;

        var _owner = owner;
        if (window.hasOwnProperty('intellisense')) {
            _owner = new ViewReviewViewModel();
        }

        // Fields
        self.id = ko.observable(init.id);
        self.order = ko.observable(init.order || 0);
        self.published = ko.observable(init.published || false);
        self.description = ko.observable(init.description || '');
        self.folders = ko.observableArray([]);
        self.active = ko.computed(function () {
            return _owner.activeIteration() && _owner.activeIteration().id() === self.id();
        });

        self.activate = function () {
            _owner.activeIteration(self);
        }

        self.active.subscribe(function (newValue) {
            if (self.active()) {
                self.refreshFiles();
            }
        });

        self.refreshFiles = function () {
            // Reload files list
            $.ajax({
                url: '~/api/v1/iterations/' + self.id(),
                type: 'get',
                statusCode: {
                    404: function () {
                        sy.utils.fail('todo: tell user the iteration does not exist');
                        sy.bus.navigate.publish('');
                    },
                    403: function () {
                        sy.utils.fail("todo: tell user they aren't on this review");
                        sy.bus.navigate.publish('');
                    },
                    200: function (data) {
                        self.folders.removeAll();
                        for (var i = 0; i < data.length; i++) {
                            self.folders.push(new Folder({
                                name: data[i].name,
                                files: data[i].files.map(function(f) { return new File(_owner, f); })
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
            sy.bus.exec.publish('reviews.upload');
        };

        self.remove = function () {
            if (confirm('Are you sure you want to delete this iteration? This cannot be undone. If you just want to hide it from users, you can unpublish it')) {
                $.ajax({
                    url: '~/api/v1/iterations/' + self.id(),
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
                            var oldOrder = self.order();
                            _owner.iterations.remove(self);
                            if (_owner.iterations().length > 0) {
                                var order = oldOrder;
                                if (order > _owner.iterations().length - 1) {
                                    order = _owner.iterations().length - 1;
                                }
                                if (order === oldOrder) {
                                    _owner.iterations()[order].activate();
                                } else {
                                    sy.bus.navigate.publish('reviews/' + _owner.id() + '/iterations/' + (order + 1));
                                }
                            } else {
                                sy.bus.navigate.publish('reviews/' + _owner.id());
                            }
                        }
                    }
                });
            }
        }

        return self;
    }

    function ViewReviewViewModel(init) {
        var self = this;

        var startIter = (init.startIter - 1) || 0;

        self.id = ko.observable(init.id);
        self.description = ko.observable(init.description || '');
        self.iterations = ko.observableArray(init.iterations || []);
        self.participants = ko.observableArray(init.participants || []);
        self.title = ko.observable(init.title || '');
        self.author = ko.observable(init.author);
        self.activeFile = ko.observable(init.activeFile);
        self.activeIteration = ko.observable(init.activeIteration);

        self.iterations.subscribe(function () {
            // Reorder
            for (var i = 0; i < self.iterations().length; i++) {
                self.iterations()[i].order(i);
            }
        });

        self.load = function () {
            rR.app.appBarVisible(true);
            $.ajax({
                url: '~/api/v1/reviews/' + self.id(),
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
                        self.title(data.title);
                        self.description(data.description);
                        self.author(new rR.models.User(data.author));
                        self.iterations.removeAll();
                        for (var i = 0; i < data.iterations.length; i++) { self.iterations.push(new Iteration(self, data.iterations[i])); }
                        if (self.iterations().length > 0) {
                            if (startIter > self.iterations().length - 1) {
                                alert('No such iteration!');
                                startIter = 0;
                            }
                            self.selectIteration(startIter + 1);
                        }
                    }
                }
            });
        };

        self.selectIteration = function (order) {
            order = order - 1;
            if (order < 0 || order > self.iterations().length - 1) {
                alert('Unknown iteration!');
            }
            self.iterations()[order].activate();
        }

        self.newIteration = function () {
            // Add a new iteration
            $.ajax({
                url: '~/api/v1/iterations',
                type: 'post',
                data: { reviewId: self.id() },
                statusCode: {
                    403: function () {
                        sy.utils.fail("todo: tell user they aren't on this review");
                        sy.bus.navigate.publish('');
                    },
                    201: function (data) {
                        var lastIter = self.iterations()[self.iterations().length - 1];
                        var order = lastIter ? lastIter.order() + 1 : 0;
                        var iter = new Iteration(self, {
                            id: data.id,
                            order: order
                        });
                        self.iterations.push(iter);
                        sy.bus.navigate.publish('reviews/' + self.id() + '/iterations/' + (iter.order() + 1));
                    }
                }
            });
        };
    };

    function UploadFileViewModel(owner) {
        var self = this;
        var _owner = owner;
        if(window.hasOwnProperty('intellisense')) {
            _owner = new ViewReviewViewModel();
        }

        self.diff = ko.observable('').required('Diff is required');
        ko.validation.addValidation(self);

        self.upload = function () {
            self.validate();
            if (self.isValid()) {
                $.ajax({
                    url: '~/api/v1/iterations/' + _owner.activeIteration().id(),
                    type: 'put',
                    data: { diff: self.diff() },
                    statusCode: {
                        404: function () {
                            rR.utils.fail("todo: tell user that this iteration doesn't exist");
                            sy.bus.dialog.dismiss.publish();
                        },
                        403: function () {
                            rR.utils.fail("todo: tell user they aren't on this review");
                            sy.bus.navigate.publish('');
                        },
                        204: function () {
                            sy.bus.dialog.dismiss.publish();
                            _owner.activeIteration().refreshFiles();
                        }
                    }
                });
            }
        }
    }

    var reviews = rR.module('reviews', function () {
        var self = this;

        var _activeReview;
        if (window.hasOwnProperty('intellisense')) {
            _activeReview = new ViewReviewViewModel();
        }

        function openReview(id, order) {
            _activeReview = new ViewReviewViewModel({ id: id, startIter: order });
            self.openPage('view', _activeReview);
        }

        self.page('view', ViewReviewViewModel);
        self.dialog('upload', UploadFileViewModel);

        self.route('Iteration', 'reviews/:reviewId/iterations/:order', function (reviewId, order) {
            if (!_activeReview || _activeReview.id() !== reviewId) {
                openReview(reviewId, order);
            } else {
                _activeReview.selectIteration(order);
            }
        });

        self.route('Review', 'reviews/:id', function (id) {
            openReview(id);
        });

        self.action('upload', function () {
            this.showDialog('upload', new UploadFileViewModel(_activeReview));
        });
    });
})(syrah, rR);