/// <reference path="fx\syrah.ts" />
module syrah {
    export module bus {
        export class auth {
            public static setToken = new Sink();
            public static clearToken = new Sink();
        }
    }
}

module rR {
    import sy = syrah;
    
    export class App extends sy.App {
        public environment = ko.observable(sy.setting('environment') || '');
        public version = ko.observable(sy.setting('version') || '');
        // currentUser = new rR.models.User();
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

            sy.bus.auth.setToken.subscribe(this.onSetToken);
            sy.bus.auth.clearToken.subscribe(this.onClearToken);
        }

        // Top-level commands
        public logout() {
            sy.bus.exec.publish('auth.logout');
        }
        public login() {
            sy.bus.exec.publish('auth.login');
        }

        // Bus handlers
        private onSetToken(user) {

        }

        private onClearToken() {
        }
    }
}