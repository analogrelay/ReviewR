/// <reference path="Backbone.lite.js" />
/// <reference path="rR.js" />
/// <reference path="rR.bus.js" />

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
    router.route('reviews/:id', 'review', function (id) {
        rR.app.currentParams = {
            id: id
        };
        viewModel.activePage('reviews.view');
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
        // Select all 'a's with out data-skip, and with an href starting "http" or "//"
        $(document).on('click', "a:not([data-skip],[href^='http'],[href^='//'])", function (evt) {
            evt.preventDefault();
            router.navigate($(this).attr('href'), { trigger: true });
        });
    }

    rR.bus.sink('login', ['user']).subscribe(function (user) {
        rR.bus.closeDialog.publish();
        viewModel.currentUser()._.update(user);
        viewModel.currentUser().loggedIn(true);

        // Rerun the current route
        history.loadUrl();
    });

    rR.bus.sink('logout').subscribe(function () {
        rR.bus.closeDialog.publish();
        viewModel.currentUser()._.reset();
        history.loadUrl();
    });
    
    rR.bus.sink('closeDialog').subscribe(function () {
        viewModel.activeDialog('');
    });

    rR.bus.sink('showDialog', ['id']).subscribe(function (id) {
        viewModel.activeDialog(id);
    });

    rR.bus.sink('navigate', ['url']).subscribe(function (url) {
        rR.bus.closeDialog.publish();
        router.navigate(url, { trigger: true });
    });

    rR.publish('app', {
        start: start,
        viewModel: viewModel,
        currentParams: {}
    });
})(window);