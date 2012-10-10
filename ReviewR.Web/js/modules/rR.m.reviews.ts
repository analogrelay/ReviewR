/// <reference path="..\ref\knockout.d.ts" />
/// <reference path="..\ref\jquery.timeago.d.ts" />

import sy = module('../fx/syrah');

import models = module('../rR.models');

export enum FileChangeType {
    Added,
    Modified,
    Removed
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

export class DiffLineViewModel extends sy.ViewModelBase {
}

export class ViewReviewViewModel extends sy.ViewModelBase {
}