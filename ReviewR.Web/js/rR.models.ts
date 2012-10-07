/// <reference path="ref\knockout.d.ts" />
interface UserReferenceInit {
    id?: Number;
    email?: string;
    emailHash?: string;
    displayName?: string;
}

export class UserReference {
    public id: Knockout.Observable;
    public email: Knockout.Observable;
    public emailHash: Knockout.Observable;
    public displayName: Knockout.Observable;
    public gravatarUrl: Knockout.Observable;

    constructor ();
    constructor (init: UserReferenceInit);
    constructor (init?: UserReferenceInit) {
        this.id = ko.observable(init.id || 0);
        this.email = ko.observable(init.email || '');
        this.emailHash = ko.observable(init.emailHash || '');
        this.displayName = ko.observable(init.displayName || '');
        this.gravatarUrl = ko.computed(() =>
            'http://www.gravatar.com/avatar/' + this.emailHash() + '?s=16');
    }
}

interface UserInit extends UserReferenceInit {
    roles?: string[];
    loggedIn?: bool;
}

export class User extends UserReference {
    public token: string;
    public isAdmin: Knockout.Observable;
    public roles: Knockout.ObservableArray;
    public loggedIn: Knockout.Observable;

    constructor ();
    constructor (init: UserInit);
    constructor (init?: UserInit) {
        super(init);

        this.roles = ko.observableArray(init.roles || []);
        this.loggedIn = ko.observable(init.loggedIn || false);
        this.isAdmin = ko.computed(() =>
            this.roles().indexOf('Admin') > -1);
    }
}

interface ReviewInit {
    id?: Number;
    title?: string;
    authorName?: string;
    authorEmail?: string;
    authorEmailHash?: string;
}

export class Review {
    public id: Knockout.Observable;
    public title: Knockout.Observable;
    public authorName: Knockout.Observable;
    public authorEmail: Knockout.Observable;
    public authorEmailHash: Knockout.Observable;
    public authorGravatarUrl: Knockout.Observable;
    public url: Knockout.Observable;

    constructor ();
    constructor (init: ReviewInit);
    constructor (init?: ReviewInit) {
        this.id = ko.observable(init.id || 0);
        this.title = ko.observable(init.title || '');
        this.authorName = ko.observable(init.authorName || '');
        this.authorEmail = ko.observable(init.authorEmail || '');
        this.authorEmailHash = ko.observable(init.authorEmailHash || '');
        this.authorGravatarUrl = ko.computed(() =>
            'http://www.gravatar.com/avatar/' + this.authorEmailHash() + '?s=16');
        this.url = ko.computed(() =>
            'reviews/' + this.id());
    }
}