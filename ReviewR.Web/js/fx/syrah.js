var syrah;
(function (syrah) {
    function setting(key) {
        return document.documentElement.getAttribute('data-' + key);
    }
    syrah.setting = setting;
    function createViewHost() {
        var selectors = [];
        for (var _i = 0; _i < (arguments.length - 0); _i++) {
            selectors[_i] = arguments[_i + 0];
        }
        var found = null;
        for(var selector in selectors) {
            found = syrah.dom.querySelector(selector);
        }
        if(!found) {
            throw new Error('View host not found');
        }
        return new syrah.binding.KnockoutViewHost(found);
    }
    syrah.createViewHost = createViewHost;
            function unwrapViewHost(funcOrObj) {
        if(typeof funcOrObj === "function") {
            return new funcOrObj();
        } else {
            return funcOrObj;
        }
    }
    syrah.unwrapViewHost = unwrapViewHost;
    var App = (function () {
        function App(pageHost, dialogHost) {
            this.router = new syrah.routing.Router();
            this.actions = {
            };
            this.running = false;
            this.environment = setting('environment');
            this.rootUrl = setting('root');
            this.pageHost = unwrapViewHost(pageHost) || createViewHost('#syrah-page-host');
            this.dialogHost = unwrapViewHost(dialogHost) || createViewHost('#syrah-dialog-host');
            if($ && $.ajaxPrefilter) {
                $.ajaxPrefilter(function (options) {
                    options.url = this.resolveUrl(options.url);
                });
            }
        }
        App.prototype.route = function (name, url, handler) {
            this.router.map(name, url, handler);
        };
        App.prototype.resolveUrl = function (appRelativeUrl, fullUrl) {
            var url = appRelativeUrl;
            if(url[0] === '~' && url[1] === '/') {
                url = this.rootUrl + appRelativeUrl.substr(2);
                if(fullUrl) {
                    url = location.protocol + '//' + location.host + url;
                }
            }
            return url;
        };
        App.prototype.action = function (name, handler) {
            this.actions[name] = handler;
        };
        App.prototype.module = function (mod) {
            if(this.running) {
                mod.attach(this);
            } else {
                this.modules.push(mod);
            }
        };
        App.prototype.start = function () {
            var _this = this;
            this.running = true;
            this.modules.forEach(function (mod) {
                mod.attach(_this);
            });
            ko.applyBindings(self, document.body);
            this.router.start(this.rootUrl);
        };
        App.prototype.closePage = function () {
            this.pageHost.clearView();
        };
        App.prototype.openPage = function (view, model) {
            this.pageHost.setView(view, model);
        };
        App.prototype.showDialog = function (view, model) {
            this.pageHost.obscure();
            this.dialogHost.showDialog(view, model);
        };
        App.prototype.refresh = function () {
            this.router.refresh();
        };
        App.prototype.closeDialog = function () {
            this.dialogHost.closeDialog();
            this.pageHost.reveal();
        };
        return App;
    })();
    syrah.App = App;    
    var Module = (function () {
        function Module() { }
        Module.prototype.attach = function (app) {
        };
        return Module;
    })();
    syrah.Module = Module;    
})(syrah || (syrah = {}));

//@ sourceMappingURL=syrah.js.map
