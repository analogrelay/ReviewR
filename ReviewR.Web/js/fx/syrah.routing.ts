/// <reference path="..\ref\Backbone.lite.d.ts" />
/// <reference path="..\ref\Modernizr.d.ts" />
export class Router {
    private router = new Backbone.Router();
    private currentRoute;
    constructor () {
        Backbone.history || (Backbone.history = new Backbone.History());
    }

    public refresh() {
        this.router.refresh();
    }

    public map(name: string, url: string, handler: Function) {
        this.router.route(name, url, handler);
    }

    public start(root: string) {
        Backbone.history.start({
            root: root,
            pushState: Modernizr.pushState
        });
    }

    public stop() {
        Backbone.history.stop();
    }

    public navigate(url: string) {
        this.router.navigate(url, { trigger: true });
    }
}