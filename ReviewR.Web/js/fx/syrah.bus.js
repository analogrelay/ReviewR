/// <reference path="namespace.js" />
(function (undefined) {
    "use strict";
    namespace.define('syrah.bus', function (ns) {
        ns.register = function(name, args, allowOnly) {
            /// <signature>
            ///     <param name="name" type="String">The name of the sink to create</param>
            /// </signature>
            /// <signature>
            ///     <param name="name" type="String">The name of the sink to create</param>
            ///     <param name="args" type="Array">Names of arguments proved to the publish method (which are passed through to all subscribers)</param>
            /// </signature>
            /// <signature>
            ///     <param name="name" type="String">The name of the sink to create</param>
            ///     <param name="args" type="Array">Arguments proved to the publish method (which are passed through to all subscribers)</param>
            ///     <param name="allowOnly" type="String">
            ///         A restriction to place on the publicly available sink. If 'publish', then any code which
            ///         accesses this sink via syrah.bus will only be able to publish. If 'subscribe', then they will
            ///         only be able to subscribe. The sink returned by this function has full functionality.
            ///     </param>
            /// </signature>
            /// <signature>
            ///     <param name="name" type="String">The name of the sink to create</param>
            ///     <param name="allowOnly" type="String">
            ///         A restriction to place on the publicly available sink. If 'publish', then any code which
            ///         accesses this sink via syrah.bus will only be able to publish. If 'subscribe', then they will
            ///         only be able to subscribe. The sink returned by this function has full functionality.
            ///     </param>
            /// </signature>
            if (typeof args === 'string') {
                allowOnly = args;
                args = [];
            }
            var sink = new syrah.bus.Sink(args);
            var pub = sink;
            if (allowOnly === 'publish') {
                pub = restrictSubscribe(pub);
            } else if (allowOnly === 'subscribe') {
                pub = restrictPublish(pub);
            }

            namespace.define('syrah.bus.' + name, pub);
            
            return sink;
        }

        ns.Sink = function(args) {
            var self = this;
            var sig = new signals.Signal();

            self._publish = function(params) {
                sig.dispatch.apply(sig, params);
            }

            self.subscribe = function(handler, context, priority) {
                return sinkBinding(sig.add(handler, context, priority));
            }

            self.publish = (args && args.length > 0) ? new Function(args, "this._publish(Array.prototype.slice.call(arguments))") : function () { _publish(Array.prototype.slice.call(arguments)); }
        }

        function restrictSubscribe(sink) {
            return {
                publish: sink.publish
            };
        }

        function restrictPublish(sink) {
            return {
                subscribe: sink.subscribe
            };
        }

        function sinkBinding(signalBinding) {
            return {
                detach: function () { signalBinding.detach(); }
            };
        }
    });
})();