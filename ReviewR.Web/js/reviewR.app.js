/// <reference path="reviewR.js" />

// reviewR.boot.js
// Startup code

if (!window.rR) {
    throw 'reviewR.js must be imported before reviewR.boot.js';
}

(function (rR) {
    "use strict";

    var _root = '/';
    var _viewModel = rR.models.application({
        loginModal: 'auth.login',
        registerModal: 'auth.register',
        activePage: rR.models.page({
            view: 'home.index',
            model: {}
        })
    });

    // Public methods
    function start(root, env) {
        _root = root;
        _viewModel.environment(env);

        if (env === 'Development') {
            rR.utils.activateDevMode();
        }

        ko.applyBindings(_viewModel, document.getElementById('root'));
    }

    function dismissModal() {
        _viewModel.activeModal('');
    }

    $.extend(rR, {
        app: {
            start: start,
            dismissModal: dismissModal
        }
    });
})(window.rR);