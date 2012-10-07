var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
define(["require", "exports", 'fx/syrah', 'fx/syrah.bus', 'rR.models'], function(require, exports, __sy__, __sybus__, __models__) {
    var sy = __sy__;

    var sybus = __sybus__;

    var models = __models__;

    sybus.create('auth.setToken');
    sybus.create('auth.clearToken');
    var _modules = [];
    exports.app;
    var App = (function (_super) {
        __extends(App, _super);
        function App(pageHost, dialogHost) {
            var _this = this;
                _super.call(this, pageHost, dialogHost);
            this.environment = ko.observable(sy.setting('environment') || '');
            this.version = ko.observable(sy.setting('version') || '');
            this.currentUser = ko.observable(new models.User());
            this.appBarVisible = ko.observable(false);
            this.isDataVolatile = ko.computed(function () {
                return _this.environment() === 'Production';
            });
            this.isDataBestEffort = ko.computed(function () {
                return _this.environment() === 'Preview';
            });
            sybus.get('auth.setToken').subscribe(this.onSetToken);
            sybus.get('auth.clearToken').subscribe(this.onClearToken);
        }
        App.prototype.logout = function () {
            sybus.get('exec').publish('auth.logout');
        };
        App.prototype.login = function () {
            sybus.get('exec').publish('auth.login');
        };
        App.prototype.onSetToken = function (user) {
        };
        App.prototype.onClearToken = function () {
        };
        return App;
    })(sy.App);
    exports.App = App;    
            function start(pageHost, dialogHost) {
        exports.app = new App(pageHost, dialogHost);
        _modules.forEach(function (mod) {
            exports.app.module(mod);
        });
        _modules = null;
        exports.app.start();
    }
    exports.start = start;
    function module(type) {
        var mod = new type();
        if(exports.app) {
            exports.app.module(mod);
        } else {
            _modules.push(mod);
        }
        return mod;
    }
    exports.module = module;
    $(function () {
        start();
    });
    $(document.body).on('click', 'a:not([data-link="exterior"],[href="#"],[href^="http:"],[href^="https:"],[href^="//"]),a[data-link="interior"]', function (evt) {
        evt.preventDefault();
        sybus.get('navigate').publish($(this).attr('href'));
    });
})

//@ sourceMappingURL=rR.app.js.map
