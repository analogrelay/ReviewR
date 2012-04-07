/// <reference path="rR.js" />

// rR.app.js
// Core application code

(function (window, undefined) {
    "use strict";

    var _root = '/';
    var viewModel = rR.models.application({
        loginDialog: 'auth.login',
        registerDialog: 'auth.register'
    });

    // Set up route table
    crossroads.addRoute('/', function () {
        viewModel.activePage('home.index');
    });

    // Public methods
    function start(init) {
        _root = init.root;
        viewModel.environment(init.env);

        if (init.env === 'Development') {
            rR.utils.activateDevMode();
        }
        if (init.user) {
            viewModel.currentUser()._.update(init.user);
            viewModel.currentUser().loggedIn(true);
        }

        //setup hasher
        function parseHash(newHash, oldHash) {
            crossroads.parse(newHash);
        }
        hasher.initialized.add(parseHash); //parse initial hash
        hasher.changed.add(parseHash); //parse hash changes
        hasher.init(); //start listening for history change

        ko.applyBindings(viewModel, document.getElementById('root'));
    }

    function login(user) {
        viewModel.activeDialog('');
        viewModel.currentUser()._.update(user);
        viewModel.currentUser().loggedIn(true);
    }

    function dismissDialog() {
        viewModel.activeDialog('');
    }

    rR.publish('app', {
        start: start,
        dismissDialog: dismissDialog,
        login: login,
        viewModel: viewModel
    });
})(window);