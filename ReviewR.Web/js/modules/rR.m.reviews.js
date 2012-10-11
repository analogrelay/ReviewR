var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
var sy = require('../fx/syrah')
var sybus = require('../fx/syrah.bus')
var rR = require('../rR.app')
var models = require('../rR.models')
(function (FileChangeType) {
    FileChangeType._map = [];
    FileChangeType.Added = 0;
    FileChangeType.Modified = 1;
    FileChangeType.Removed = 2;
})(exports.FileChangeType || (exports.FileChangeType = {}));

(function (LineChangeType) {
    LineChangeType._map = [];
    LineChangeType.Same = 0;
    LineChangeType.Added = 1;
    LineChangeType.Removed = 2;
    LineChangeType.HunkHeader = 3;
})(exports.LineChangeType || (exports.LineChangeType = {}));

var CommentViewModel = (function (_super) {
    __extends(CommentViewModel, _super);
    function CommentViewModel(owner, line, init) {
        var _this = this;
        _super.call(this);
        this.owner = owner;
        this.line = line;
        this.id = ko.observable(init.id || 0);
        this.body = ko.observable(init.body || '');
        this.author = ko.observable();
        this.isAuthor = ko.observable(init.isAuthor || false);
        this.postedOn = ko.observable();
        if(init.postedOn) {
            this.postedOn(new Date(init.postedOn));
        }
        if(init.author) {
            this.author(new models.UserReference(init.author));
        }
        this.displayPostedOn = ko.computed(function () {
            return $.timeago(_this.postedOn());
        });
    }
    CommentViewModel.prototype.deleteComment = function () {
        $.ajax('~/api/v1/comments/' + this.id(), {
            type: 'delete'
        }).done(function () {
            this.line.comments.remove(self);
        }).fail(function (xhr) {
            switch(xhr.status) {
                case 404: {
                    alert('no such comment!');
                    break;

                }
                case 403: {
                    alert('hey! you can\'t delete someone else\'s comment!');
                    break;

                }
            }
        });
    };
    return CommentViewModel;
})(sy.ViewModelBase);
exports.CommentViewModel = CommentViewModel;
var DiffLineViewModel = (function (_super) {
    __extends(DiffLineViewModel, _super);
    function DiffLineViewModel(init, owner) {
        _super.call(this);
        this.owner = owner;
        this.type = ko.observable(init.type);
        this.text = ko.observable(init.text || '');
        this.index = ko.observable(init.index || 0);
        this.leftLine = ko.observable(init.leftLine || 0);
        this.rightLine = ko.observable(init.rightLine || 0);
        this.comments = ko.observableArray([]);
        this.newCommentBody = ko.observable();
        this.addingComment = ko.observable(false);
        if(init.comments) {
            for(var i = 0; i < init.comments.length; i++) {
                this.comments.push(new CommentViewModel(this.owner, this, init.comments[i]));
            }
        }
    }
    DiffLineViewModel.prototype.startComment = function () {
        this.addingComment(true);
    };
    DiffLineViewModel.prototype.cancelComment = function () {
        this.newCommentBody('');
        this.addingComment(false);
    };
    DiffLineViewModel.prototype.postComment = function () {
        var _this = this;
        $.ajax('~/api/v1/comments', {
            type: 'post',
            data: {
                line: this.index(),
                changeId: this.owner.activeFile().id(),
                body: this.newCommentBody()
            }
        }).done(function (data) {
            _this.comments.push(new CommentViewModel(_this.owner, _this, data));
        }).fail(function (xhr) {
            switch(xhr.status) {
                case 404: {
                    sybus.get('navigate').publish('');
                    break;

                }
                case 403: {
                    sybus.get('navigate').publish('');
                    break;

                }
            }
        });
        this.newCommentBody('');
    };
    return DiffLineViewModel;
})(sy.ViewModelBase);
exports.DiffLineViewModel = DiffLineViewModel;
var DiffViewModel = (function (_super) {
    __extends(DiffViewModel, _super);
    function DiffViewModel(init) {
        _super.call(this);
        this.id = ko.observable(init.id);
        this.fileName = ko.observable(init.fileName);
        this.deletions = ko.observable(init.deletions);
        this.insertions = ko.observable(init.insertions);
        this.binary = ko.observable(init.binary);
        this.lines = ko.observableArray(init.lines);
    }
    return DiffViewModel;
})(sy.ViewModelBase);
exports.DiffViewModel = DiffViewModel;
var FolderViewModel = (function (_super) {
    __extends(FolderViewModel, _super);
    function FolderViewModel(init) {
        _super.call(this);
        this.name = ko.observable(init.name);
        this.files = ko.observableArray(init.files);
    }
    return FolderViewModel;
})(sy.ViewModelBase);
exports.FolderViewModel = FolderViewModel;
var FileViewModel = (function (_super) {
    __extends(FileViewModel, _super);
    function FileViewModel(init, owner) {
        var _this = this;
        _super.call(this);
        this.owner = owner;
        this.id = ko.observable(init.id);
        this.fileName = ko.observable(init.fileName);
        this.changeType = ko.observable(init.changeType);
        this.hasComments = ko.observable(init.hasComments);
        this.diff = ko.observable();
        this.fullPath = init.fullPath[0] === '/' ? init.fullPath.substr(1) : init.fullPath;
        this.active = ko.computed(function () {
            return _this.owner.activeFile() && _this.owner.activeFile().id() === _this.id();
        });
        this.viewUrl = ko.computed(function () {
            return '/reviews/' + _this.owner.id() + '/iterations/' + _this.owner.activeIteration().id() + '/' + _this.fullPath() + '/r';
        });
    }
    FileViewModel.prototype.activate = function () {
        var _this = this;
        this.owner.activeFile(this);
        if(!this.diff()) {
            $.ajax('~/api/v1/changes/' + this.id(), {
                type: 'get',
                statusCode: {
                    404: function () {
                        sybus.get('navigate').publish('');
                    },
                    403: function () {
                        sybus.get('navigate').publish('');
                    },
                    200: function (data) {
                        this.diff(new DiffViewModel({
                            id: data.id,
                            fileName: data.fileName,
                            fullPath: data.fullPath,
                            deletions: data.deletions,
                            insertions: data.insertions,
                            binary: data.binary,
                            lines: data.lines.map(function (l) {
                                return new DiffLineViewModel(l, _this.owner);
                            })
                        }));
                    }
                }
            });
        }
    };
    return FileViewModel;
})(sy.ViewModelBase);
exports.FileViewModel = FileViewModel;
var IterationViewModel = (function (_super) {
    __extends(IterationViewModel, _super);
    function IterationViewModel(init, owner) {
        var _this = this;
        _super.call(this);
        this.owner = owner;
        this.id = ko.observable(init.id);
        this.order = ko.observable(init.order);
        this.published = ko.observable(init.published);
        this.description = ko.observable(init.description);
        this.folders = ko.observableArray([]);
        this.active.subscribe(function (newValue) {
            if(_this.active()) {
                _this.refreshFiles();
            }
        });
    }
    IterationViewModel.prototype.activate = function (startPath) {
        this.startPath = startPath;
        this.owner.selectFile();
        this.owner.activeIteration(this);
    };
    IterationViewModel.prototype.refreshFiles = function () {
        var _this = this;
        $.ajax('~/api/v1/iterations/' + this.id(), {
            type: 'get'
        }).done(function (data) {
            _this.folders.removeAll();
            for(var i = 0; i < data.length; i++) {
                _this.folders.push(new FolderViewModel({
                    name: data[i].name,
                    files: data[i].files.map(function (f) {
                        return new FileViewModel(_owner, f);
                    })
                }));
            }
            if(_this.startPath) {
                _this.owner.selectFile(_this.startPath);
                _this.startPath = null;
            }
        }).fail(function (xhr) {
            switch(xhr.status) {
                case 404: {
                    sybus.get('navigate').publish('');
                    break;

                }
                case 403: {
                    sybus.get('navigate').publish('');
                    break;

                }
            }
        });
    };
    IterationViewModel.prototype.publish = function () {
        var _this = this;
        $.ajax('~/api/v1/iterations/' + this.id(), {
            type: 'put',
            data: {
                published: true
            }
        }).done(function (data) {
            _this.published(true);
        }).fail(function (xhr) {
            switch(xhr.status) {
                case 404: {
                    alert('todo: tell user no such iteration');
                    break;

                }
                case 403: {
                    alert("todo: tell user they aren't the author of this review");
                    break;

                }
            }
        });
    };
    IterationViewModel.prototype.unpublish = function () {
        var _this = this;
        $.ajax('~/api/v1/iterations/' + this.id(), {
            type: 'put',
            data: {
                published: false
            }
        }).done(function (data) {
            _this.published(false);
        }).fail(function (xhr) {
            switch(xhr.status) {
                case 404: {
                    alert('todo: tell user no such iteration');
                    break;

                }
                case 403: {
                    alert("todo: tell user they aren't the author of this review");
                    break;

                }
            }
        });
    };
    IterationViewModel.prototype.addFiles = function () {
        sybus.get('exec').publish('reviews.upload');
    };
    IterationViewModel.prototype.remove = function () {
        var _this = this;
        if(confirm('Are you sure you want to delete this iteration? This cannot be undone. If you just want to hide it from users, you can unpublish it')) {
            $.ajax('~/api/v1/iterations/' + this.id(), {
                type: 'delete'
            }).done(function (data) {
                var oldOrder = _this.order();
                _this.owner.iterations.remove(self);
                if(_this.owner.iterations().length > 0) {
                    var next = oldOrder;
                    if(next > _this.owner.iterations().length - 1) {
                        next = _this.owner.iterations().length - 1;
                    }
                    var iter = _this.owner.iterations()[next];
                    sybus.get('navigate').publish('reviews/' + _this.owner.id() + '/iterations/' + iter.id());
                } else {
                    sybus.get('navigate').publish('reviews/' + _this.owner.id());
                }
            }).fail(function (xhr) {
                switch(xhr.status) {
                    case 404: {
                        alert('todo: tell user no such review');
                        sybus.get('navigate').publish('');
                        break;

                    }
                    case 403: {
                        alert("todo: tell user they aren't on this review");
                        sybus.get('navigate').publish('');
                        break;

                    }
                }
            });
        }
    };
    return IterationViewModel;
})(sy.ViewModelBase);
exports.IterationViewModel = IterationViewModel;
var ViewReviewViewModel = (function (_super) {
    __extends(ViewReviewViewModel, _super);
    function ViewReviewViewModel() {
        var _this = this;
        _super.call(this);
        this.id = ko.observable(0);
        this.description = ko.observable('');
        this.iterations = ko.observableArray([]);
        this.participants = ko.observableArray([]);
        this.owner = ko.observable();
        this.title = ko.observable('');
        this.author = ko.observable();
        this.activeFile = ko.observable();
        this.activeIteration = ko.observable();
        this.iterations.subscribe(function () {
            for(var i = 0; i < _this.iterations().length; i++) {
                _this.iterations()[i].order(i);
            }
        });
    }
    ViewReviewViewModel.prototype.load = function () {
        rR.app.appBarVisible(true);
        $.ajax('~/api/v1/reviews/' + this.id(), {
            type: 'get',
            statusCode: {
                401: function () {
                    alert('todo: prompt user for login');
                    sybus.get('navigate').publish('');
                },
                403: function () {
                    alert("todo: tell user they aren't on this review");
                    sybus.get('navigate').publish('');
                },
                200: function (data) {
                    this.title(data.title);
                    this.description(data.description);
                    this.author(new models.User(data.author));
                    this.owner(data.owner);
                    this.iterations.removeAll();
                    for(var i = 0; i < data.iterations.length; i++) {
                        this.iterations.push(new IterationViewModel(self, data.iterations[i]));
                    }
                    if(this.iterations().length > 0) {
                        if(startIter) {
                            this.selectIteration(startIter, init.startPath);
                        } else {
                            this.iterations()[0].activate(init.startPath);
                        }
                    }
                }
            }
        });
    };
    return ViewReviewViewModel;
})(sy.ViewModelBase);
exports.ViewReviewViewModel = ViewReviewViewModel;

//@ sourceMappingURL=rR.m.reviews.js.map
