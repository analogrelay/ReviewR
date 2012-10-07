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

    // Models
    function CommentViewModel(init, owner, line) {
        init = init || {};
        var self = this;
        var _owner = owner;
        var _line = line;

        // Fields
        self.id = ko.observable(init.id);
        self.body = ko.observable(init.body || '');
        self.author = ko.observable();
        self.postedOn = ko.observable();
        self.isAuthor = ko.observable(init.isAuthor || false);

        if (init.postedOn) {
            self.postedOn(new Date(init.postedOn));
        }

        self.displayPostedOn = ko.computed(function () {
            return $.timeago(self.postedOn());
        });

        if (init.author) {
            self.author(new rR.models.UserReference(init.author));
        }

        self.deleteComment = function () {
            $.ajax({
                url: '~/api/v1/comments/' + self.id(),
                type: 'delete',
            })
                .success(function () {
                    _line.comments.remove(self);
                })
                .statusCode({
                    404: function () {
                        alert('no such comment!');
                    },
                    403: function () {
                        alert('hey! you can\'t delete someone else\'s comment!');
                    }
                });
        };
    }

    function DiffLineViewModel(init, owner) {
        init = init || {};
        var self = this;

        var _owner = owner;
        if (window.hasOwnProperty('intellisense')) {
            _owner = new ViewReviewViewModel();
        }

        // Fields
        self.type = ko.observable(init.type);
        self.text = ko.observable(init.text || '');
        self.index = ko.observable(init.index);
        self.leftLine = ko.observable(init.leftLine);
        self.rightLine = ko.observable(init.rightLine);
        self.comments = ko.observableArray([]);
        self.newCommentBody = ko.observable();

        if (init.comments) {
            for (var i = 0; i < init.comments.length; i++) {
                self.comments.push(new CommentViewModel(init.comments[i], _owner, self));
            }
        }

        // Layout fields
        self.addingComment = ko.observable(false);

        self.startComment = function () {
            self.addingComment(true);
        }

        self.cancelComment = function () {
            self.newCommentBody('');
            self.addingComment(false);
        }

        self.postComment = function () {
            $.ajax({
                url: '~/api/v1/comments',
                type: 'post',
                data: { line: self.index(), changeId: _owner.activeFile().id(), body: self.newCommentBody() }
            })
                .success(function(data) {
                    self.comments.push(new CommentViewModel(data, _owner, self));
                })
                .statusCode({
                    404: function () {
                        sy.utils.fail('todo: tell user the iteration does not exist');
                        sy.bus.navigate.publish('');
                    },
                    403: function () {
                        sy.utils.fail("todo: tell user they aren't on this review");
                        sy.bus.navigate.publish('');
                    }
                });
            self.newCommentBody('');
        }

        return self;
    }

    function DiffViewModel(init, file) {
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

    function FolderViewModel(init) {
        init = init || {};
        var self = this;

        // Fields
        self.name = ko.observable(init.name || '');
        self.files = ko.observableArray(init.files || []);

        return self;
    }

    function FileViewModel(owner, init) {
        init = init || {};
        var self = this;
        var _owner = owner;
        if (window.hasOwnProperty('intellisense')) {
            _owner = new ViewReviewViewModel();
        }

        var fullPath = init.fullPath;
        if (fullPath[0] === '/') {
            fullPath = fullPath.substr(1);
        }

        // Fields
        self.id = ko.observable(init.id);
        self.fileName = ko.observable(init.fileName);
        self.fullPath = ko.observable(fullPath);
        self.changeType = ko.observable(init.changeType);
        self.diff = ko.observable();
        self.hasComments = ko.observable(init.hasComments);
        self.active = ko.computed(function () {
            return (_owner.activeFile() && _owner.activeFile().id() === self.id());
        });

        // Computed
        self.viewUrl = ko.computed(function () {
            return '/reviews/' + _owner.id() + '/iterations/' + _owner.activeIteration().id() + '/' + self.fullPath() + '/r';
        });

        self.activate = function () {
            _owner.activeFile(self);
            if (!self.diff()) {
                $.ajax({
                    url: '~/api/v1/changes/' + self.id(),
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
                            self.diff(new DiffViewModel({
                                id: data.id,
                                fileName: data.fileName,
                                fullPath: data.fullPath,
                                deletions: data.deletions,
                                insertions: data.insertions,
                                binary: data.binary,
                                lines: data.lines.map(function (l) { return new DiffLineViewModel(l, _owner) })
                            }));
                        }
                    }
                });
            }
        }

        return self;
    }

    function IterationViewModel(owner, init) {
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

        var _startPath;
        self.activate = function (startPath) {
            _startPath = startPath;
            _owner.selectFile();
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
                            self.folders.push(new FolderViewModel({
                                name: data[i].name,
                                files: data[i].files.map(function(f) { return new FileViewModel(_owner, f); })
                            }));
                        }
                        if (_startPath) {
                            _owner.selectFile(_startPath);
                            _startPath = null;
                        }
                    }
                }
            });
        };

        self.publish = function () {
            $.ajax({
                url: '~/api/v1/iterations/' + self.id(),
                type: 'put',
                data: { published: true }
            })
             .success(function(data) {
                 self.published(true);
             })
             .statusCode({
                 404: function () {
                     rR.utils.fail('todo: tell user no such iteration');
                 },
                 403: function () {
                     rR.utils.fail("todo: tell user they aren't the author of this review");
                 }
             });
        };

        self.unpublish = function () {
            $.ajax({
                url: '~/api/v1/iterations/' + self.id(),
                type: 'put',
                data: { published: false }
            })
             .success(function(data) {
                 self.published(false);
             })
             .statusCode({
                 404: function () {
                     rR.utils.fail('todo: tell user no such iteration');
                 },
                 403: function () {
                     rR.utils.fail("todo: tell user they aren't the author of this review");
                 }
             });
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
                                var next = oldOrder;
                                if (next > _owner.iterations().length - 1) {
                                    next = _owner.iterations().length - 1;
                                }
                                var iter = _owner.iterations()[next];
                                sy.bus.navigate.publish('reviews/' + _owner.id() + '/iterations/' + iter.id());
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

        var startIter = init.startIter;

        self.id = ko.observable(init.id);
        self.description = ko.observable(init.description || '');
        self.iterations = ko.observableArray(init.iterations || []);
        self.participants = ko.observableArray(init.participants || []);
        self.owner = ko.observable(init.owner);
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
                        sy.utils.fail('todo: prompt user for login');
                        sy.bus.navigate.publish('');
                    },
                    403: function () {
                        sy.utils.fail("todo: tell user they aren't on this review");
                        sy.bus.navigate.publish('');
                    },
                    200: function (data) {
                        self.title(data.title);
                        self.description(data.description);
                        self.author(new rR.models.User(data.author));
                        self.owner(data.owner);
                        self.iterations.removeAll();
                        for (var i = 0; i < data.iterations.length; i++) { self.iterations.push(new IterationViewModel(self, data.iterations[i])); }
                        if (self.iterations().length > 0) {
                            if (startIter) {
                                self.selectIteration(startIter, init.startPath);
                            } else {
                                self.iterations()[0].activate(init.startPath);
                            }
                        }
                    }
                }
            });
        };

        self.selectIteration = function (id, startPath) {
            var iter;
            for (var i = 0; i < self.iterations().length; i++) {
                if (self.iterations()[i].id() == id) {
                    iter = self.iterations()[i];
                    break;
                }
            }
            if (!iter) {
                alert('Unknown iteration!');
            } else {
                iter.activate(startPath);
            }
        }

        self.selectFile = function (path) {
            if (!path) {
                self.activeFile(null);
                return;
            }

            var iter = self.activeIteration();

            // TODO: jslinq?
            for (var i = 0; i < iter.folders().length; i++) {
                for (var j = 0; j < iter.folders()[i].files().length; j++) {
                    if (iter.folders()[i].files()[j].fullPath() === path) {
                        iter.folders()[i].files()[j].activate();
                        return;
                    }
                }
            }
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
                        var iter = new IterationViewModel(self, {
                            id: data.id,
                            order: order
                        });
                        self.iterations.push(iter);
                        sy.bus.navigate.publish('reviews/' + self.id() + '/iterations/' + iter.id());
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
                            sy.utils.fail("todo: tell user that this iteration doesn't exist");
                            sy.bus.dialog.dismiss.publish();
                        },
                        403: function () {
                            sy.utils.fail("todo: tell user they aren't on this review");
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

        function openReview(id, order, path) {
            _activeReview = new ViewReviewViewModel({ id: id, startIter: order, startPath: path });
            self.openPage('view', _activeReview);
        }

        self.page('view', ViewReviewViewModel);
        self.dialog('upload', UploadFileViewModel);

        self.route('File', 'reviews/:reviewId/iterations/:iterId/*path/r', function (reviewId, iterId, path) {
            if (!_activeReview || _activeReview.id() !== reviewId) {
                openReview(reviewId, iterId, path);
            } else {
                if (_activeReview.activeIteration().id() !== iterId) {
                    _activeReview.selectIteration(iterId);
                }
                _activeReview.selectFile(path);
            }
        });

        self.route('Iteration', 'reviews/:reviewId/iterations/:iterId', function (reviewId, iterId) {
            if (!_activeReview || _activeReview.id() !== reviewId) {
                openReview(reviewId, iterId);
            } else {
                _activeReview.selectIteration(iterId);
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