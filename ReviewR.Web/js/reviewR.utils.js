// reviewR.utils.js
// Utilities
(function (window, undefined) {
    "use strict";

    var _devMode = false;
    var _root = '/';

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

    function getModel(id) {
        return resolveNestedName(rR, 'm.' + id);
    }

    function getView(id) {
        return document.getElementById(getViewId(id));
    }

    function getViewId(id) {
        return 'v:' + id;
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

    function setRoot(root) {
        _root = root;
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

    function resolveUrl(url) {
        /// <param name="url" type="String" />
        if (url[0] === '~' && url[1] === '/') {
            return _root + url.substr(2);
        }
        return _root + url;
    }

    window.rR = $.extend(window.rR || {}, {
        utils: {
            resolveNestedName: resolveNestedName,
            getModel: getModel,
            signalModal: signalModal,
            fail: fail,
            assert: assert,
            activateDevMode: activateDevMode,
            getView: getView,
            getViewId: getViewId,
            setRoot: setRoot,
            resolveUrl: resolveUrl
        }
    });
})(window);