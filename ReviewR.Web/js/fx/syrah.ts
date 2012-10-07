/// <reference path="../../Scripts/signals.js" />
/// <reference path="syrah.dom.ts" />
/// <reference path="syrah.binding.ts" />
/// <reference path="syrah.routing.ts" />
/// <reference path="syrah.bus.ts" />

import dom = module('syrah.dom');
import binding = module('syrah.binding');
import bus = module('syrah.bus');
import routing = module('syrah.routing');
import rendering = module('syrah.rendering');

bus.create('navigate');
bus.create('exec');
bus.create('dismiss');

export function setting(key: string) {
    return document.documentElement.getAttribute('data-' + key);
}

export function createViewHost(...selectors: string[]) {
    var found: HTMLElement = null;
    for (var selector in selectors) {
        found = dom.querySelector(selector);
    }
    if (!found) {
        throw new Error('View host not found');
    }
    return new binding.KnockoutViewHost(found);
}

export function unwrapViewHost(func: () => any): any;
export function unwrapViewHost(obj: any);
export function unwrapViewHost(funcOrObj: any) {
    if (typeof funcOrObj === "function") {
        return new funcOrObj();
    } else {
        return funcOrObj;
    }
}

export class App {
    public router = new routing.Router();
    private modules: Module[];
    private actions = {};
    private running = false;

    private env: string;
    private rootUrl: string;
    private pageHost: rendering.ViewHost;
    private dialogHost: rendering.ViewHost;

    constructor ();
    constructor (pageHost: HTMLElement);
    constructor (pageHost: HTMLElement, dialogHost: HTMLElement);
    constructor (pageHost?: HTMLElement, dialogHost?: HTMLElement) {
        this.env = setting('environment');
        this.rootUrl = setting('root');
        this.pageHost = unwrapViewHost(pageHost) || createViewHost('#syrah-page-host');
        this.dialogHost = unwrapViewHost(dialogHost) || createViewHost('#syrah-dialog-host');

        if ($ && $.ajaxPrefilter) {
            $.ajaxPrefilter(function (options) {
                options.url = this.resolveUrl(options.url)
            });
        }

        // Attach to the service bus
        bus.get('navigate').subscribe(this.onNavigate);
        bus.get('exec').subscribe(this.onExec);
    }

    public route(name: string, url: string, handler: Function) {
        this.router.map(name, url, handler);
    }

    public resolveUrl(appRelativeUrl: string);
    public resolveUrl(appRelativeUrl: string, fullUrl: bool);
    public resolveUrl(appRelativeUrl: string, fullUrl?: bool) {
        var url = appRelativeUrl;
        if (url[0] === '~' && url[1] === '/') {
            url = this.rootUrl + appRelativeUrl.substr(2);

            if (fullUrl) {
                url = location.protocol + '//' + location.host + url;
            }
        }
        return url;
    }

    public action(name: string, handler: Function) {
        this.actions[name] = handler;
    }

    public module(mod: Module) {
        if (this.running) {
            mod.attach(this);
        } else {
            this.modules.push(mod);
        }
    }

    public start() {
        this.running = true;

        // Attach modules
        this.modules.forEach((mod) => {
            mod.attach(this);
        });

        // Bind the rest of the page
        ko.applyBindings(self, document.body);

        // Start the router
        this.router.start(this.rootUrl);
    }

    public closePage() {
        this.pageHost.clearView();
    }

    public openPage(view: rendering.View, model: any) {
        this.pageHost.setView(view, model);
    }

    public showDialog(view: rendering.View, model: any) {
        this.pageHost.obscure();
        this.dialogHost.showDialog(view, model);
    }

    public refresh() {
        this.router.refresh();
    }

    public closeDialog() {
        this.dialogHost.closeDialog();
        this.pageHost.reveal();
    }

    private onNavigate(url: string) {
        this.closeDialog();
        this.router.navigate(url);
    }

    private onExec(action: string, ...args: any[]) {
        var act = <Function>this.actions[action];
        if (!act) {
            throw new Error('No such action: ' + action);
        }
        act.apply(this, args);
    }
}

export interface Route {
    name: string;
    url: string;
    handler: Function;
}

export interface Action {
    name: string;
    handler: Function;
}

export class Module {
    private app: App;
    private routes: Route[];
    private actions: Action[];

    public attached = new signals.Signal();
    public pages = {};
    public dialogs = {};

    constructor (private name: string) {
    }

    public attach(app: App) {
        // Capture the app
        this.app = app;

        // Attach routes
        this.routes.forEach(route => {
            this.app.route(route.name, route.url, route.handler);
        });

        // Attach actions
        this.actions.forEach(action => {
            this.app.action(action.name, action.handler);
        });
        this.attached.dispatch();
        return this;
    }

    public closePage() {
        this.app.closePage();
    }

    public openPage(pageId: string);
    public openPage(pageId: string, model: any);
    public openPage(pageId: string, model?: any) {
        var view = this.getRequiredView(this.pages, pageId);
        this.app.openPage(view, model);
    }

    public showDialog(dialogId: string);
    public showDialog(dialogId: string, model: any);
    public showDialog(dialogId: string, model?: any) {
        var view = this.getRequiredView(this.dialogs, dialogId);
        this.app.showDialog(view, model);
    }

    public closeDialog() {
        this.app.closeDialog();
    }

    public route(name: string, url: string, handler: Function) {
        this.routes.push({
            name: this.name + '.' + name,
            url: url,
            handler: handler
        });
        return this;
    }

    public action(name: string, handler: Function) {
        this.actions.push({
            name: name,
            handler: handler
        });
        return this;
    }

    public page(pageId: string, modelConstructor: new () => any);
    public page(pageId: string, modelConstructor: new () => any, options: rendering.ViewOptions);
    public page(pageId: string, modelConstructor: new () => any, options?: rendering.ViewOptions) {
        this.addView(this.pages, pageId, modelConstructor, options);
    }

    public dialog(dialogId: string, modelConstructor: new () => any);
    public dialog(dialogId: string, modelConstructor: new () => any, options: rendering.ViewOptions);
    public dialog(dialogId: string, modelConstructor: new () => any, options?: rendering.ViewOptions) {
        this.addView(this.dialogs, dialogId, modelConstructor, options);
    }

    private addView(collection: Object,
        viewId: string,
        modelConstructor: new () => any);
    private addView(collection: Object,
        viewId: string,
        modelConstructor: new () => any,
        options: rendering.ViewOptions);
    private addView(collection: Object,
        viewId: string,
        modelConstructor: new () => any,
        options?: rendering.ViewOptions) {
        if (collection.hasOwnProperty(viewId)) {
            throw new Error('A view named ' + viewId + ' has already been defined by this module');
        }
        options = options || {};
        var templateId = options.templateId;
        if (typeof (templateId) === 'undefined') {
            templateId = viewId;
        }
        var view = new rendering.View(templateId, modelConstructor, options);
        collection[viewId] = view;
        return view;
    }

    private getRequiredView(collection: Object, viewId: string) {
        if (!collection.hasOwnProperty(viewId)) {
            throw new Error('No such view: ' + viewId);
        }
        return <rendering.View>collection[viewId];
    }
}

export class ViewModelBase {
    public customError: Knockout.Observable;
    public isValid: Knockout.Observable;
    public errorMessage: Knockout.Observable;
    public hasMessage: Knockout.Observable;

    constructor () {
        this.customError = ko.observable('');

        this.isValid = ko.computed(() => {
            var v = true;
            for (var key in self) {
                if (this.hasOwnProperty(key) &&
                    ko.isObservable(this[key]) &&
                    this[key].validating &&
                    this[key].validating()) {
                    v &= self[key].isValid();
                }
            }
            return v;
        });

        this.errorMessage = ko.computed(() =>
            this.isValid() ? this.customError() : 'Whoops, there were some errors :(');
        this.hasMessage = ko.computed(() =>
            !this.isValid() || (this.customError() && (this.customError().length > 0)));
    }

    public validate() {
        for (var key in this) {
            if (this.hasOwnProperty(key) && ko.isObservable(this[key]) && this[key].validating) {
                this[key].validating(true);
            }
        }
    }
}

export class DialogViewModelBase extends ViewModelBase {
    public close() {
        bus.get('dismiss').publish();
    }
}