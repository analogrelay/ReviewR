var syrah;
(function (syrah) {
    (function (bus) {
        bus.navigate = new bus.Sink();
        bus.exec = new bus.Sink();
        bus.dismiss = new bus.Sink();
    })(syrah.bus || (syrah.bus = {}));
    var bus = syrah.bus;

})(syrah || (syrah = {}));

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
    var DialogViewModel = (function () {
        function DialogViewModel() { }
        DialogViewModel.prototype.close = function () {
            syrah.bus.dismiss.publish();
        };
        return DialogViewModel;
    })();
    syrah.DialogViewModel = DialogViewModel;    
    var App = (function () {
        function App(pageHost, dialogHost) {
            this.router = new syrah.routing.Router();
            this.actions = {
            };
            this.running = false;
            this.env = setting('environment');
            this.rootUrl = setting('root');
            this.pageHost = unwrapViewHost(pageHost) || createViewHost('#syrah-page-host');
            this.dialogHost = unwrapViewHost(dialogHost) || createViewHost('#syrah-dialog-host');
            if($ && $.ajaxPrefilter) {
                $.ajaxPrefilter(function (options) {
                    options.url = this.resolveUrl(options.url);
                });
            }
            syrah.bus.navigate.subscribe(this.onNavigate);
            syrah.bus.exec.subscribe(this.onExec);
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
        App.prototype.onNavigate = function (url) {
            this.closeDialog();
            this.router.navigate(url);
        };
        App.prototype.onExec = function (action) {
            var args = [];
            for (var _i = 0; _i < (arguments.length - 1); _i++) {
                args[_i] = arguments[_i + 1];
            }
            var act = this.actions[action];
            if(!act) {
                throw new Error('No such action: ' + action);
            }
            act.apply(this, args);
        };
        return App;
    })();
    syrah.App = App;    
    var Module = (function () {
        function Module(name) {
            this.name = name;
            this.attached = new signals.Signal();
            this.pages = {
            };
            this.dialogs = {
            };
        }
        Module.prototype.attach = function (app) {
            var _this = this;
            this.app = app;
            this.routes.forEach(function (route) {
                _this.app.route(route.name, route.url, route.handler);
            });
            this.actions.forEach(function (action) {
                _this.app.action(action.name, action.handler);
            });
            this.attached.dispatch();
            return this;
        };
        Module.prototype.closePage = function () {
            this.app.closePage();
        };
        Module.prototype.openPage = function (pageId, model) {
            var view = this.getRequiredView(this.pages, pageId);
            this.app.openPage(view, model);
        };
        Module.prototype.showDialog = function (dialogId, model) {
            var view = this.getRequiredView(this.dialogs, dialogId);
            this.app.showDialog(view, model);
        };
        Module.prototype.closeDialog = function () {
            this.app.closeDialog();
        };
        Module.prototype.route = function (name, url, handler) {
            this.routes.push({
                name: this.name + '.' + name,
                url: url,
                handler: handler
            });
            return this;
        };
        Module.prototype.action = function (name, handler) {
            this.actions.push({
                name: name,
                handler: handler
            });
            return this;
        };
        Module.prototype.page = function (pageId, modelConstructor, options) {
            this.addView(this.pages, pageId, modelConstructor, options);
        };
        Module.prototype.dialog = function (dialogId, modelConstructor, options) {
            this.addView(this.dialogs, dialogId, modelConstructor, options);
        };
        Module.prototype.addView = function (collection, viewId, modelConstructor, options) {
            if(collection.hasOwnProperty(viewId)) {
                throw new Error('A view named ' + viewId + ' has already been defined by this module');
            }
            options = options || {
            };
            var templateId = options.templateId;
            if(typeof (templateId) === 'undefined') {
                templateId = viewId;
            }
            var view = new syrah.rendering.View(templateId, modelConstructor, options);
            collection[viewId] = view;
            return view;
        };
        Module.prototype.getRequiredView = function (collection, viewId) {
            if(!collection.hasOwnProperty(viewId)) {
                throw new Error('No such view: ' + viewId);
            }
            return collection[viewId];
        };
        return Module;
    })();
    syrah.Module = Module;    
})(syrah || (syrah = {}));

//@ sourceMappingURL=syrah.js.map
