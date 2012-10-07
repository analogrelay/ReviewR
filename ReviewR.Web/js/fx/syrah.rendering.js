var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
var syrah;
(function (syrah) {
    (function (rendering) {
        var View = (function () {
            function View(templateId, modalConstructor, options) {
                this.templateId = templateId;
                this.modalConstructor = modalConstructor;
                this.options = options;
                this.injected = new signals.Signal();
                this.removed = new signals.Signal();
            }
            return View;
        })();
        rendering.View = View;        
        var Page = (function (_super) {
            __extends(Page, _super);
            function Page() {
                _super.apply(this, arguments);

                this.obscured = new signals.Signal();
                this.revealed = new signals.Signal();
            }
            return Page;
        })(View);
        rendering.Page = Page;        
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
                    model = view.modalConstructor();
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
                syrah.dom.initDialog(this.host, function () {
                    _this.clearView();
                });
            };
            ViewHost.prototype.showDialog = function (view, model) {
                if(view && model) {
                    this.setView(view, model);
                }
                syrah.dom.showDialog(this.host);
            };
            ViewHost.prototype.closeDialog = function () {
                syrah.dom.hideDialog(this.host);
            };
            return ViewHost;
        })();
        rendering.ViewHost = ViewHost;        
    })(syrah.rendering || (syrah.rendering = {}));
    var rendering = syrah.rendering;

})(syrah || (syrah = {}));

//@ sourceMappingURL=syrah.rendering.js.map
