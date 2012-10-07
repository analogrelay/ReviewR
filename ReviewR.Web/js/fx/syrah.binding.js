var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
var syrah;
(function (syrah) {
    (function (binding) {
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
        })(syrah.rendering.ViewHost);
        binding.KnockoutViewHost = KnockoutViewHost;        
    })(syrah.binding || (syrah.binding = {}));
    var binding = syrah.binding;

})(syrah || (syrah = {}));

//@ sourceMappingURL=syrah.binding.js.map
