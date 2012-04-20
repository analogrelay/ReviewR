/// <reference path="namespace.js" />
(function (undefined) {
    "use strict";
    namespace.define('syrah.bus', function (ns) {
        ns.register = function(name, args) {
            /// <signature>
            ///     <param name="name" type="String">The name of the sink to create</param>
            /// </signature>
            /// <signature>
            ///     <param name="name" type="String">The name of the sink to create</param>
            ///     <param name="args" type="Array">Names of arguments proved to the publish method (which are passed through to all subscribers)</param>
            /// </signature>
            if (typeof args === 'string') {
                allowOnly = args;
                args = [];
            }
            var sink = new syrah.bus.Sink(args);
            namespace.define('syrah.bus.' + name, sink);
            
            return sink;
        }

        ns.Sink = function(args) {
            var self = this;
            var sig = new signals.Signal();

            self.publish = function(params) {
                sig.dispatch.apply(sig, Array.prototype.slice.apply(arguments));
            }

            self.subscribe = function(handler, context, priority) {
                return sig.add(handler, context, priority);
            }
        }
    });
})();