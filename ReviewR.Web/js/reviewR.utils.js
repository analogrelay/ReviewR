// reviewR.utils.js
// Utilities

if (!window.rR) {
    window.rR = {}
}

(function (rR) {
    "use strict";

    var _devMode = false;

    function resolveNestedName(root, name) {
        /// <param name="root" type="Object" />
        /// <param name="name" type="String" />
        /// <returns type="Object" />
        var names = name.split('.');
        var cur = root;
        for (var i = 0; i < names.length; i++) {
            cur = cur[names[i]];
        }
        return cur;
    }

    function getModal(modalId) {
        return resolveNestedName(rR, 'm.' + modalId);
    }

    function signalModal(modalId, signal) {
        var model = resolveNestedName(rR, 'm.' + modalId)
        if (model && model[signal]) {
            model[signal]();
        }
    }

    function activateDevMode() {
        _devMode = true;
    }

    function fail(msg) {
        if (_devMode) {
            if (confirm('Assert failed: ' + msg + '\nPress OK to throw, Cancel to abort')) {
                throw 'Assert failed: ' + msg;
            } else {
                if (window.stop) {
                    window.stop();
                } else {
                    document.execCommand('Stop');
                }
            }
        }
    }

    function assert(val, msg) {
        if (!val) {
            fail(msg);
        }
    }

    $.extend(rR, {
        utils: {
            resolveNestedName: resolveNestedName,
            getModal: getModal,
            signalModal: signalModal,
            fail: fail,
            assert: assert,
            activateDevMode: activateDevMode
        }
    });
})(window.rR);