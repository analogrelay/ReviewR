// rR.utils.js
// Utilities
(function (window, undefined) {
    "use strict";

    // Module management
    window.rR = {};
    window.rR.publish = function (name, val) {
        /// <param name="name" type="String">The name of the property to publish</param>
        /// <param name="val" type="Object">The value of the property</param>
        var parts = name.split('.');
        var cur = window.rR;
        for (var i = 0; i < parts.length; i++) {
            if (!cur[parts[i]]) {
                if (i === parts.length - 1) {
                    cur[parts[i]] = val;
                } else {
                    cur[parts[i]] = {}
                }
            }
            cur = cur[parts[i]];
        }
    }

    var _devMode = false;
    var _root = '/';

    function resolveNestedName(root, name) {
        if (!root) { throw 'root is required'; }
        if (!name) { throw 'name is required'; }
        /// <param name="root" type="Object" />
        /// <param name="name" type="String" />
        /// <returns type="Object" />
        var names = name.split('.');
        var cur = root;
        for (var i = 0; i < names.length && cur; i++) {
            cur = cur[names[i]];
        }
        return cur;
    }

    function getModel(id) {
        return resolveNestedName(rR, 'vm.' + id);
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
            if (confirm('Assert failed: ' + msg + '\nPress OK to throw, Cancel to continue')) {
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

    rR.publish('utils', {
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
    });
})(window);