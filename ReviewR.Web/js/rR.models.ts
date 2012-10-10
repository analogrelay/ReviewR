/// <reference path="ref\knockout.d.ts" />
export interface UserReferenceInit {
    id?: number;
    email?: string;
    emailHash?: string;
    displayName?: string;
}

export class UserReference {
    public id = ko.observable(init.id || 0);
    public email = ko.observable(init.email || '');
    public emailHash = ko.observable(init.emailHash || '');
    public displayName = ko.observable(init.displayName || '');
    public gravatarUrl: Knockout.Observable;

    constructor ();
    constructor (init: UserReferenceInit);
    constructor (init?: UserReferenceInit) {
        this.gravatarUrl = ko.computed(() =>
            'http://www.gravatar.com/avatar/' + this.emailHash() + '?s=16');
    }
}

export interface UserInit extends UserReferenceInit {
    roles?: string[];
    loggedIn?: bool;
}

export class User extends UserReference {
    public token: string;
    public isAdmin: Knockout.Observable;
    public roles = ko.observableArray(init.roles || []);
    public loggedIn = ko.observable(init.loggedIn || false);

    constructor ();
    constructor (init: UserInit);
    constructor (init?: UserInit) {
        super(init);

        this.isAdmin = ko.computed(() : bool =>
            this.roles().indexOf('Admin') > -1);
    }
}

export interface ReviewInit {
    id?: number;
    title?: string;
    authorName?: string;
    authorEmail?: string;
    authorEmailHash?: string;
}

export class Review {
    public authorGravatarUrl: Knockout.Observable;
    public url: Knockout.Observable;
    public id = ko.observable(init.id || 0);
    public title = ko.observable(init.title || '');
    public authorName = ko.observable(init.authorName || '');
    public authorEmail = ko.observable(init.authorEmail || '');
    public authorEmailHash = ko.observable(init.authorEmailHash || '');

    constructor ();
    constructor (init: ReviewInit);
    constructor (init?: ReviewInit) {
        this.authorGravatarUrl = ko.computed(() =>
            'http://www.gravatar.com/avatar/' + this.authorEmailHash() + '?s=16');
        this.url = ko.computed(() =>
            'reviews/' + this.id());
    }
}