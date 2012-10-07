/// <reference path="../../Scripts/signals.js" />
/// <reference path="syrah.dom.ts" />
/// <reference path="syrah.binding.ts" />
/// <reference path="syrah.routing.ts" />
module syrah {
    export function setting(key : string) {
        return document.documentElement.getAttribute('data-' + key);
    }

    export function createViewHost(...selectors: string[]) {
        var found: HTMLElement = null;
        for (var selector in selectors) {
            found = syrah.dom.querySelector(selector);
        }
        if (!found) {
            throw new Error('View host not found');
        }
        return new syrah.binding.KnockoutViewHost(found);
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
        public router = new syrah.routing.Router();
        private modules: Module[];
        private actions = {};
        private running = false;


        private environment: string;
        private rootUrl: string;
        private pageHost: syrah.rendering.ViewHost;
        private dialogHost: syrah.rendering.ViewHost;

        constructor ();
        constructor (pageHost: HTMLElement);
        constructor (pageHost: HTMLElement, dialogHost: HTMLElement);
        constructor (pageHost?: HTMLElement, dialogHost?: HTMLElement) {
            this.environment = setting('environment');
            this.rootUrl = setting('root');
            this.pageHost = unwrapViewHost(pageHost) || createViewHost('#syrah-page-host');
            this.dialogHost = unwrapViewHost(dialogHost) || createViewHost('#syrah-dialog-host');

            if ($ && $.ajaxPrefilter) {
                $.ajaxPrefilter(function (options) {
                    options.url = this.resolveUrl(options.url)
                });
            }
        }

        public route(name: string, url: string, handler: (...) => void ) {
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

        public action(name: string, handler: () => void ) {
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

        public openPage(view : syrah.rendering.View, model : any) {
            this.pageHost.setView(view, model);
        }

        public showDialog(view: syrah.rendering.View, model: any) {
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

        // TODO: syrah.bus?
    }

    export class Module {
        public attach(app: App) {
        }
    }
}