var syrah;
(function (syrah) {
    (function (routing) {
        var Router = (function () {
            function Router() {
                this.router = new Backbone.Router();
                Backbone.history || (Backbone.history = new Backbone.History());
            }
            Router.prototype.refresh = function () {
                this.router.refresh();
            };
            Router.prototype.map = function (name, url, handler) {
                this.router.route(name, url, handler);
            };
            Router.prototype.start = function (root) {
                Backbone.history.start({
                    root: root,
                    pushState: Modernizr.pushState
                });
            };
            Router.prototype.stop = function () {
                Backbone.history.stop();
            };
            Router.prototype.navigate = function (url) {
                this.router.navigate(url, {
                    trigger: true
                });
            };
            return Router;
        })();
        routing.Router = Router;        
    })(syrah.routing || (syrah.routing = {}));
    var routing = syrah.routing;

})(syrah || (syrah = {}));

//@ sourceMappingURL=syrah.routing.js.map
