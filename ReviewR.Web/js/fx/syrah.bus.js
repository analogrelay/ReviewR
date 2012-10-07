var syrah;
(function (syrah) {
    (function (bus) {
        var Sink = (function () {
            function Sink() {
                this.sig = new signals.Signal();
            }
            Sink.prototype.publish = function () {
                var params = [];
                for (var _i = 0; _i < (arguments.length - 0); _i++) {
                    params[_i] = arguments[_i + 0];
                }
                (this.sig.dispatch).apply(this.sig, (Array.prototype.slice).apply(arguments));
            };
            Sink.prototype.subscribe = function (handler, context, priority) {
                return this.sig.add(handler, context, priority);
            };
            return Sink;
        })();
        bus.Sink = Sink;        
    })(syrah.bus || (syrah.bus = {}));
    var bus = syrah.bus;

})(syrah || (syrah = {}));

//@ sourceMappingURL=syrah.bus.js.map
