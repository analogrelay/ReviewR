/// <reference path="rR.js" />
(function (window) {
    "use strict";

    function sink(name, args, allowOnly) {
        /// <signature>
        ///     <param name="name" type="String">The name of the sink to create</param>
        ///     <returns type="sinkType">The created sink. The sink is also stored in rR.bus[name]</returns>
        /// </signature>
        /// <signature>
        ///     <param name="name" type="String">The name of the sink to create</param>
        ///     <param name="args" type="Array">Names of arguments proved to the publish method (which are passed through to all subscribers)</param>
        ///     <returns type="sinkType">The created sink. The sink is also stored in rR.bus[name]</returns>
        /// </signature>
        /// <signature>
        ///     <param name="name" type="String">The name of the sink to create</param>
        ///     <param name="args" type="Array">Arguments proved to the publish method (which are passed through to all subscribers)</param>
        ///     <param name="allowOnly" type="String">
        ///         A restriction to place on the publicly available sink. If 'publish', then any code which
        ///         accesses this sink via rR.bus will only be able to publish. If 'subscribe', then they will
        ///         only be able to subscribe. The sink returned by this function has full functionality.
        ///     </param>
        ///     <returns type="sinkType">The created sink. The sink is also stored in rR.bus[name]</returns>
        /// </signature>
        /// <signature>
        ///     <param name="name" type="String">The name of the sink to create</param>
        ///     <param name="allowOnly" type="String">
        ///         A restriction to place on the publicly available sink. If 'publish', then any code which
        ///         accesses this sink via rR.bus will only be able to publish. If 'subscribe', then they will
        ///         only be able to subscribe. The sink returned by this function has full functionality.
        ///     </param>
        ///     <returns type="sinkType">The created sink. The sink is also stored in rR.bus[name]</returns>
        /// </signature>
        if (typeof args === 'string') {
            allowOnly = args;
            args = [];
        }

        if (rR.bus.hasOwnProperty(name)) {
            return rR.bus[name];
        }
        var sink = sinkType(args);
        var pub = sink;
        if (allowOnly === 'publish') {
            pub = restrictSubscribe(pub);
        } else if (allowOnly === 'subscribe') {
            pub = restrictPublish(pub);
        }
        rR.bus[name] = pub;

        return sink;
    }

    function sinkType(args) {
        var sig = new signals.Signal();

        function _publish(params) {
            sig.dispatch.apply(sig, params);
        }

        function subscribe(handler, context, priority) {
            return sinkBinding(sig.add(handler, context, priority));
        }

        var publish = (args && args.length > 0) ? new Function(args, "this._publish(Array.prototype.slice.call(arguments))") : function () { _publish(Array.prototype.slice.call(arguments)); }

        return {
            publish: publish,
            subscribe: subscribe,
            _publish: _publish
        };
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

    rR.publish('bus', {
        sink: sink
    });
})(window);