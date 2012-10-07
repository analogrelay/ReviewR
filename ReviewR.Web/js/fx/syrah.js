var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
define(["require", "exports", 'syrah.dom', 'syrah.binding', 'syrah.bus', 'syrah.routing', 'syrah.rendering'], function(require, exports, __dom__, __binding__, __bus__, __routing__, __rendering__) {
    var dom = __dom__;

    var binding = __binding__;

    var bus = __bus__;

    var routing = __routing__;

    var rendering = __rendering__;

    bus.create('navigate');
    bus.create('exec');
    bus.create('dismiss');
    function setting(key) {
        return document.documentElement.getAttribute('data-' + key);
    }
    exports.setting = setting;
    function createViewHost() {
        var selectors = [];
        for (var _i = 0; _i < (arguments.length - 0); _i++) {
            selectors[_i] = arguments[_i + 0];
        }
        var found = null;
        for(var selector in selectors) {
            found = dom.querySelector(selector);
        }
        if(!found) {
            throw new Error('View host not found');
        }
        return new binding.KnockoutViewHost(found);
    }
    exports.createViewHost = createViewHost;
            function unwrapViewHost(funcOrObj) {
        if(typeof funcOrObj === "function") {
            return new funcOrObj();
        } else {
            return funcOrObj;
        }
    }
    exports.unwrapViewHost = unwrapViewHost;
    var App = (function () {
        function App(pageHost, dialogHost) {
            this.router = new routing.Router();
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
            bus.get('navigate').subscribe(this.onNavigate);
            bus.get('exec').subscribe(this.onExec);
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
    exports.App = App;    
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
            var view = new rendering.View(templateId, modelConstructor, options);
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
    exports.Module = Module;    
    var ViewModelBase = (function () {
        function ViewModelBase() {
            var _this = this;
            this.customError = ko.observable('');
            this.isValid = ko.computed(function () {
                var v = true;
                for(var key in self) {
                    if(_this.hasOwnProperty(key) && ko.isObservable(_this[key]) && _this[key].validating && _this[key].validating()) {
                        v &= self[key].isValid();
                    }
                }
                return v;
            });
            this.errorMessage = ko.computed(function () {
                return _this.isValid() ? _this.customError() : 'Whoops, there were some errors :(';
            });
            this.hasMessage = ko.computed(function () {
                return !_this.isValid() || (_this.customError() && (_this.customError().length > 0));
            });
        }
        ViewModelBase.prototype.validate = function () {
            for(var key in this) {
                if(this.hasOwnProperty(key) && ko.isObservable(this[key]) && this[key].validating) {
                    this[key].validating(true);
                }
            }
        };
        return ViewModelBase;
    })();
    exports.ViewModelBase = ViewModelBase;    
    var DialogViewModelBase = (function (_super) {
        __extends(DialogViewModelBase, _super);
        function DialogViewModelBase() {
            _super.apply(this, arguments);

        }
        DialogViewModelBase.prototype.close = function () {
            bus.get('dismiss').publish();
        };
        return DialogViewModelBase;
    })(ViewModelBase);
    exports.DialogViewModelBase = DialogViewModelBase;    
})

//@ sourceMappingURL=syrah.js.map
