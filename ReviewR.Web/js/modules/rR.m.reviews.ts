/// <reference path="..\ref\knockout.d.ts" />
/// <reference path="..\ref\jquery.timeago.d.ts" />

import sy = module('../fx/syrah');
import sybus = module('../fx/syrah.bus');

import rR = module('../rR.app');
import models = module('../rR.models');

export enum FileChangeType {
    Added = 0,
    Modified = 1,
    Removed = 2
}

export enum LineChangeType {
    Same = 0,
    Added = 1,
    Removed = 2,
    HunkHeader = 3
}

export interface CommentViewModelInit {
    id?: number;
    body?: string;
    isAuthor?: bool;
    postedOn?: number;
    author?: models.UserReferenceInit;
}

export class CommentViewModel extends sy.ViewModelBase {
    public id = ko.observable(init.id || 0);
    public body = ko.observable(init.body || '');
    public author = ko.observable();
    public isAuthor = ko.observable(init.isAuthor || false);
    public postedOn = ko.observable();
    public displayPostedOn: Knockout.Observable;

    constructor (owner: ViewReviewViewModel, line: DiffLineViewModel);
    constructor (owner: ViewReviewViewModel, line: DiffLineViewModel, init: CommentViewModelInit);
    constructor (private owner: ViewReviewViewModel, private line: DiffLineViewModel, init?: CommentViewModelInit) {
        super();

        if (init.postedOn) {
            this.postedOn(new Date(init.postedOn));
        }
        if (init.author) {
            this.author(new models.UserReference(init.author));
        }
        this.displayPostedOn = ko.computed(() => $.timeago(this.postedOn()));
    }

    public deleteComment() {
        $.ajax('~/api/v1/comments/' + this.id(), {
            type: 'delete',
        }).done(function () {
            this.line.comments.remove(self);
        }).fail(xhr => {
            switch (xhr.status) {
                case 404:
                    alert('no such comment!');
                    break;
                case 403:
                    alert('hey! you can\'t delete someone else\'s comment!');
                    break;
            }
        });
    }
}

export interface DiffLineViewModelInit {
    type?: LineChangeType;
    text?: string;
    index?: number;
    leftLine?: number;
    rightLine?: number;
    comments?: CommentViewModelInit[];
}

export class DiffLineViewModel extends sy.ViewModelBase {
    public type = ko.observable(init.type);
    public text = ko.observable(init.text || '');
    public index = ko.observable(init.index || 0);
    public leftLine = ko.observable(init.leftLine || 0);
    public rightLine = ko.observable(init.rightLine || 0);
    public comments = ko.observableArray([]);
    public newCommentBody = ko.observable();
    public addingComment = ko.observable(false);

    constructor (init: DiffLineViewModelInit, private owner: ViewReviewViewModel) {
        super();

        if (init.comments) {
            for (var i = 0; i < init.comments.length; i++) {
                this.comments.push(new CommentViewModel(this.owner, this, init.comments[i]));
            }
        }
    }

    public startComment() {
        this.addingComment(true);
    }

    public cancelComment() {
        this.newCommentBody('');
        this.addingComment(false);
    }

    public postComment() {
        $.ajax('~/api/v1/comments', {
                type: 'post',
                data: { line: this.index(), changeId: this.owner.activeFile().id(), body: this.newCommentBody() }
            }).done(data => {
                this.comments.push(new CommentViewModel(this.owner, this, data));
            }).fail(xhr => {
                switch (xhr.status) {
                    case 404:
                        sybus.get('navigate').publish('');
                        break;
                    case 403:
                        sybus.get('navigate').publish('');
                        break;
                }
            });
            this.newCommentBody('');
    }
}

export interface DiffViewModelInit {
    id?: number;
    fileName?: string;
    deletions?: number;
    insertions?: number;
    binary?: bool;
    lines?: DiffLineViewModelInit[];
}

export class DiffViewModel extends sy.ViewModelBase {
    public id = ko.observable(init.id);
    public fileName = ko.observable(init.fileName);
    public deletions = ko.observable(init.deletions);
    public insertions = ko.observable(init.insertions);
    public binary = ko.observable(init.binary);
    public lines = ko.observableArray(init.lines);

    constructor (init: DiffViewModelInit) {
        super();
    }
}

export interface FolderViewModelInit {
    name: string;
    files: FileViewModel[];
}

export class FolderViewModel extends sy.ViewModelBase {
    public name = ko.observable(init.name);
    public files = ko.observableArray(init.files);

    constructor (init: FolderViewModelInit) {
        super();
    }
}

export interface FileViewModelInit {
    id?: number;
    fileName?: string;
    fullPath?: string;
    changeType?: FileChangeType;
    hasComments?: bool;
}

export class FileViewModel extends sy.ViewModelBase {
    public id = ko.observable(init.id);
    public fileName = ko.observable(init.fileName);
    public changeType = ko.observable(init.changeType);
    public hasComments = ko.observable(init.hasComments);
    public diff = ko.observable();
    public fullPath = init.fullPath[0] === '/' ? init.fullPath.substr(1) : init.fullPath;
    public active: Knockout.Observable;
    public viewUrl: Knockout.Observable;

    constructor (init: FileViewModelInit, private owner: ViewReviewViewModel) {
        super();

        this.active = ko.computed(() => 
            this.owner.activeFile() && 
            this.owner.activeFile().id() === this.id());
        this.viewUrl = ko.computed(() =>
            '/reviews/' + this.owner.id() +
            '/iterations/' + this.owner.activeIteration().id() +
            '/' + this.fullPath() + '/r');
    }

    public activate() {
        this.owner.activeFile(this);
        if (!this.diff()) {
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
                            lines: data.lines.map(l => new DiffLineViewModel(l, this.owner))
                        }));
                    }
                }
            });
        }
    }
}

export interface IterationViewModelInit {
    id?: number;
    order?: number;
    published?: bool;
    description?: string;
}

export class IterationViewModel extends sy.ViewModelBase {
    public id = ko.observable(init.id);
    public order = ko.observable(init.order);
    public published = ko.observable(init.published);
    public description = ko.observable(init.description);
    public folders = ko.observableArray([]);
    public active: Knockout.Observable;
    private startPath: string;

    constructor (init: IterationViewModelInit, private owner: ViewReviewViewModel) {
        super();

        this.active.subscribe(newValue => {
            if (this.active()) {
                this.refreshFiles();
            }
        });
    }

    public activate(startPath: string) {
        this.startPath = startPath;
        this.owner.selectFile();
        this.owner.activeIteration(this);
    }
    
    public refreshFiles() {
        // Reload files list
        $.ajax('~/api/v1/iterations/' + this.id(), {
            type: 'get'
        }).done(data => {
            this.folders.removeAll();
            for (var i = 0; i < data.length; i++) {
                this.folders.push(new FolderViewModel({
                    name: data[i].name,
                    files: data[i].files.map(function(f) { return new FileViewModel(_owner, f); })
                }));
            }
            if (this.startPath) {
                this.owner.selectFile(this.startPath);
                this.startPath = null;
            }
        }).fail(xhr => {
            switch (xhr.status) {
                case 404:
                    sybus.get('navigate').publish('');
                    break;
                case 403:
                    sybus.get('navigate').publish('');
                    break;
            }
        });
    }

    public publish() {
        $.ajax('~/api/v1/iterations/' + this.id(), {
            type: 'put',
            data: { published: true }
        }).done(data => {
            this.published(true)
        }).fail(xhr => {
            switch (xhr.status) {
                case 404:
                    alert('todo: tell user no such iteration');
                    break;
                case 403:
                    alert("todo: tell user they aren't the author of this review");
                    break;
            }
        });
    }

    public unpublish() {
        $.ajax('~/api/v1/iterations/' + this.id(), {
            type: 'put',
            data: { published: false }
        }).done(data => {
                this.published(false);
        }).fail(xhr => {
            switch (xhr.status) {
                case 404:
                    alert('todo: tell user no such iteration');
                    break;
                case 403:
                    alert("todo: tell user they aren't the author of this review");
                    break;
            }
        });
    }

    public addFiles() {
        sybus.get('exec').publish('reviews.upload');
    }

    public remove() {
        if (confirm('Are you sure you want to delete this iteration? This cannot be undone. If you just want to hide it from users, you can unpublish it')) {
            $.ajax('~/api/v1/iterations/' + this.id(), {
                type: 'delete'
            }).done(data => {
                // Calculate next order based on current numbers
                var oldOrder = this.order();
                this.owner.iterations.remove(self);
                if (this.owner.iterations().length > 0) {
                    var next = oldOrder;
                    if (next > this.owner.iterations().length - 1) {
                        next = this.owner.iterations().length - 1;
                    }
                    var iter = this.owner.iterations()[next];
                    sybus.get('navigate').publish('reviews/' + this.owner.id() + '/iterations/' + iter.id());
                } else {
                    sybus.get('navigate').publish('reviews/' + this.owner.id());
                }
            }).fail(xhr => {
                switch (xhr.status) {
                    case 404:
                        alert('todo: tell user no such review');
                        sybus.get('navigate').publish('');
                        break;
                    case 403:
                        alert("todo: tell user they aren't on this review");
                        sybus.get('navigate').publish('');
                        break;
                }
            });
        }
    }
}

export class ViewReviewViewModel extends sy.ViewModelBase {
    public id = ko.observable(0);
    public description = ko.observable('');
    public iterations = ko.observableArray([]);
    public participants = ko.observableArray([]);
    public owner = ko.observable();
    public title = ko.observable('');
    public author = ko.observable();
    public activeFile = ko.observable();
    public activeIteration = ko.observable();

    constructor () {
        super();

        this.iterations.subscribe(() => {
            // Reorder
            for (var i = 0; i < this.iterations().length; i++) {
                this.iterations()[i].order(i);
            }
        });
    }

    public load() {
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
                    for (var i = 0; i < data.iterations.length; i++) { 
                        this.iterations.push(new IterationViewModel(self, data.iterations[i])); 
                    }
                    if (this.iterations().length > 0) {
                        if (startIter) {
                            this.selectIteration(startIter, init.startPath);
                        } else {
                            this.iterations()[0].activate(init.startPath);
                        }
                    }
                }
            }
        });
    }
}