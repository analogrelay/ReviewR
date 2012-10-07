declare module Backbone {
    interface RouterNavigateOptions {
        trigger?: bool;
        replace?: bool;
    }

    interface HistoryStartOptions {
        root?: string;
        silent?: bool;
        pushState?: bool;
    }

    export declare class Router {
        public routes: any;

        public initialize();

        public route(route: RegExp, name: string);
        public route(route: string, name: string);
        public route(route: RegExp, name: string, callback : (...) => void);
        public route(route: string, name: string, callback : (...) => void);

        public navigate(fragment: string);
        public navigate(fragment: string, options: RouterNavigateOptions);

        public refresh();
    }

    export declare class History {
        public start(options: HistoryStartOptions);
        public stop();
    }

    export declare var history: History;
}