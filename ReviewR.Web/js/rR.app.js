var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
var syrah;
(function (syrah) {
    (function (bus) {
        var auth = (function () {
            function auth() { }
            auth.setToken = new bus.Sink();
            auth.clearToken = new bus.Sink();
            return auth;
        })();
        bus.auth = auth;        
    })(syrah.bus || (syrah.bus = {}));
    var bus = syrah.bus;

})(syrah || (syrah = {}));

var rR;
(function (rR) {
    var sy = syrah;
    ; ;
    var App = (function (_super) {
        __extends(App, _super);
        function App(pageHost, dialogHost) {
            var _this = this;
                _super.call(this, pageHost, dialogHost);
            this.environment = ko.observable(sy.setting('environment') || '');
            this.version = ko.observable(sy.setting('version') || '');
            this.appBarVisible = ko.observable(false);
            this.isDataVolatile = ko.computed(function () {
                return _this.environment() === 'Production';
            });
            this.isDataBestEffort = ko.computed(function () {
                return _this.environment() === 'Preview';
            });
            sy.bus.auth.setToken.subscribe(this.onSetToken);
            sy.bus.auth.clearToken.subscribe(this.onClearToken);
        }
        App.prototype.logout = function () {
            sy.bus.exec.publish('auth.logout');
        };
        App.prototype.login = function () {
            sy.bus.exec.publish('auth.login');
        };
        App.prototype.onSetToken = function (user) {
        };
        App.prototype.onClearToken = function () {
        };
        return App;
    })(sy.App);
    rR.App = App;    
})(rR || (rR = {}));

//@ sourceMappingURL=rR.app.js.map
