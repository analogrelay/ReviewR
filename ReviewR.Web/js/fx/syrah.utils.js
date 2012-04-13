/// <reference path="namespace.js" />
(function (querySelector, undefined) {
    "use strict";
    namespace.define('syrah.utils', function (ns) {
        var _assert = false;

        ns.getFunctionParameterNames = function(fn) {
            /// <param name="fn" type="Function" />
            /// <returns type="Array" />
            var fstr = fn.toString();
            return fstr.match(/\(.*?\)/)[0].replace(/[()]/gi,'').replace(/\s/gi,'').split(',');
        }​

        ns.enableAsserts = function() {
            _assert = true;
        }

        ns.fail = function(msg) {
            if (_assert) {
                if (confirm('Assert failed: ' + msg + '\nPress OK to throw, Cancel to continue')) {
                    throw 'Assert failed: ' + msg;
                }
            }
        }

        ns.assert = function(val, msg) {
            if (!val) {
                fail(msg);
            }
        }
    });
})();