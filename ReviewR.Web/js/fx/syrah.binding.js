var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
var syren = require("./syrah.rendering")
var KnockoutViewHost = (function (_super) {
    __extends(KnockoutViewHost, _super);
    function KnockoutViewHost(host) {
        _super.call(this, host);
        this.host = host;
    }
    KnockoutViewHost.prototype.injectView = function (name, model) {
        ko.renderTemplate('v:' + name, model, {
        }, this.host);
    };
    return KnockoutViewHost;
})(syren.ViewHost);
exports.KnockoutViewHost = KnockoutViewHost;

//@ sourceMappingURL=syrah.binding.js.map
