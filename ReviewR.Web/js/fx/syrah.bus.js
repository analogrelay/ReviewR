var busses = {
};
function create(name) {
    busses[name] = new Sink();
}
exports.create = create;
function get(name) {
    if(!busses.hasOwnProperty(name)) {
        throw new Error('No such message bus: ' + name);
    }
    return busses[name];
}
exports.get = get;
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
    Sink.prototype.register = function (name) {
    };
    return Sink;
})();

//@ sourceMappingURL=syrah.bus.js.map
