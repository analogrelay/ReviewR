var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
define(["require", "exports", 'syrah.dom'], function(require, exports, __sydom__) {
    var sydom = __sydom__;

    var View = (function () {
        function View(templateId, modelConstructor, options) {
            this.templateId = templateId;
            this.modelConstructor = modelConstructor;
            this.options = options;
            this.injected = new signals.Signal();
            this.removed = new signals.Signal();
        }
        return View;
    })();
    exports.View = View;    
    var Page = (function (_super) {
        __extends(Page, _super);
        function Page() {
            _super.apply(this, arguments);

            this.obscured = new signals.Signal();
            this.revealed = new signals.Signal();
        }
        return Page;
    })(View);
    exports.Page = Page;    
    var ViewHost = (function () {
        function ViewHost(host) {
            this.host = host;
        }
        ViewHost.prototype.injectView = function (name, model) {
        };
        ViewHost.prototype.obscure = function () {
            if(this.currentView instanceof Page) {
                (this.currentView).obscured.dispatch();
            }
        };
        ViewHost.prototype.reveal = function () {
            if(this.currentView instanceof Page) {
                (this.currentView).revealed.dispatch();
            }
        };
        ViewHost.prototype.clearView = function () {
            this.host.innerHTML = '';
            var old = this.currentView;
            this.currentView = null;
            if(old) {
                old.removed.dispatch();
            }
        };
        ViewHost.prototype.setView = function (view, model) {
            this.clearView();
            if(!model) {
                model = new view.modelConstructor();
            }
            this.injectView(view.templateId, model);
            this.currentView = view;
            this.currentView.injected.dispatch(this.currentView, model);
            if(model.load && typeof (model.load) === "function") {
                (model.load)();
            }
        };
        ViewHost.prototype.initDialog = function () {
            var _this = this;
            sydom.initDialog(this.host, function () {
                _this.clearView();
            });
        };
        ViewHost.prototype.showDialog = function (view, model) {
            if(view && model) {
                this.setView(view, model);
            }
            sydom.showDialog(this.host);
        };
        ViewHost.prototype.closeDialog = function () {
            sydom.hideDialog(this.host);
        };
        return ViewHost;
    })();
    exports.ViewHost = ViewHost;    
})

//@ sourceMappingURL=syrah.rendering.js.map
