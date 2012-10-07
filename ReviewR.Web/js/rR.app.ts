/// <reference path="rR.models.ts" />
import sy = module('fx/syrah');
import sybus = module('fx/syrah.bus');

import models = module('rR.models');

sybus.create('auth.setToken');
sybus.create('auth.clearToken');

var _modules: sy.Module[] = [];

export var app: App;

export class App extends sy.App {
    public environment = ko.observable(sy.setting('environment') || '');
    public version = ko.observable(sy.setting('version') || '');
    public currentUser = ko.observable(new models.User());
    public appBarVisible = ko.observable(false);

    // Computed Properties
    public isDataVolatile: Knockout.Observable;
    public isDataBestEffort: Knockout.Observable;

    constructor ();
    constructor (pageHost: HTMLElement);
    constructor (pageHost: HTMLElement, dialogHost: HTMLElement);
    constructor (pageHost?: HTMLElement, dialogHost?: HTMLElement) {
        super(pageHost, dialogHost);

        this.isDataVolatile = ko.computed(() =>
            this.environment() === 'Production');
        this.isDataBestEffort = ko.computed(() =>
            this.environment() === 'Preview');

        sybus.get('auth.setToken').subscribe(this.onSetToken);
        sybus.get('auth.clearToken').subscribe(this.onClearToken);
    }

    // Top-level commands
    public logout() {
        sybus.get('exec').publish('auth.logout');
    }
    public login() {
        sybus.get('exec').publish('auth.login');
    }

    // Bus handlers
    private onSetToken(user) {

    }

    private onClearToken() {
    }
}

export function start();
export function start(pageHost: HTMLElement);
export function start(pageHost?: HTMLElement, dialogHost?: HTMLElement) {
    app = new App(pageHost, dialogHost);

    _modules.forEach(mod => {
        app.module(mod);
    });
    _modules = null;

    app.start();
}

export function module(type: new() => sy.Module) {
    var mod = new type();

    if (app) {
        // Inject it right away
        app.module(mod);
    } else {
        // Save it until the app starts
        _modules.push(mod);
    }
    return mod;
}

$(function () {
    start();
});

// Hijack interior links
$(document.body).on('click', 'a:not([data-link="exterior"],[href="#"],[href^="http:"],[href^="https:"],[href^="//"]),a[data-link="interior"]', function (evt) {
    // Send the URL to the router
    evt.preventDefault();
    sybus.get('navigate').publish($(this).attr('href'));
});