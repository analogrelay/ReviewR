/// <reference path="Backbone.lite.js" />
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
    
    var history = Backbone.history = new Backbone.History();
    var router = new Backbone.Router({});

    router.route('', 'home', function () {
        if (viewModel.currentUser().loggedIn()) {
            viewModel.activePage('home.dashboard');
        } else {
            viewModel.activePage('home.index');
        }
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

        //setup history
        history.start({
            root: _root,
            hashChange: true,
            pushState: true
        });
        ko.applyBindings(viewModel, document.getElementById('root'));

        // Take over all interior links
        $("a").each(function (a) {
            var href = $(a).attr('href');
            if (href && !href.match(/^http.*$/)) {
                $(a).on('click', function (evt) {
                    evt.preventDefault();
                    hasher.setHash(href);
                });
            }
        });
    }

    function login(user) {
        viewModel.activeDialog('');
        viewModel.currentUser()._.update(user);
        viewModel.currentUser().loggedIn(true);
        
        // Rerun the current route
        history.loadUrl();
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