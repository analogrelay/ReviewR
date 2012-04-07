/// <reference path="reviewR.js" />

// reviewR.app.js
// Core application code

if (!window.rR) {
    throw 'reviewR.js must be imported before reviewR.app.js';
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
    function start(init) {
        _root = init.root;
        _viewModel.environment(init.env);

        if (init.env === 'Development') {
            rR.utils.activateDevMode();
        }
        if (init.user) {
            _viewModel.currentUser()._.update(init.user);
            _viewModel.currentUser().loggedIn(true);
        }

        ko.applyBindings(_viewModel, document.getElementById('root'));
    }

    function login(user) {
        _viewModel.activeModal('');
        _viewModel.currentUser()._.update(user);
        _viewModel.currentUser().loggedIn(true);
    }

    function dismissModal() {
        _viewModel.activeModal('');
    }

    $.extend(rR, {
        app: {
            start: start,
            dismissModal: dismissModal,
            login: login,
            viewModel: _viewModel
        }
    });
    
})(window.rR);