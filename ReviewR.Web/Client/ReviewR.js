/// <reference path="reviewR.router.js" />

if (!window.rR) {
    window.rR = {};
}

(function () {
    "use strict";

    // To avoid users being able to call methods using url hacking, we have a list of known controllers:
    var controllers = ['auth'];
    var _root = '/';

    // Routes
    function mainRouter(ctl, act, args) {
        if(controllers.indexOf(ctl) === -1) {
            throw 'No such controller: ' + ctl + ', make sure you add it to the known controllers list in reviewR.js';
        }
        rR.ensureModule(ctl)
          .then(function () {
              if (rR[ctl].hasOwnProperty(act)) {
                  rR[ctl][act].call(this, args);
              }
              else {
                  fallback();
              }
          });
    }

    //// Restore data from storage
    //var authCookie = $.cookie('ReviewRAuth');

    //// Parse cookie
    //var authTicket = JSON.parse(authCookie);

    var currentUser = {
        id: ko.observable(0),
        name: ko.observable(''),
        roles: ko.observableArray([]),
        loggedIn: ko.observable(false)
    }
    currentUser.isAdmin = ko.computed(function () { return _.indexOf(currentUser.roles(), 'admin') > -1; });

    //if (authTicket) {
    //    currentUser.id(authTicket.id);
    //    currentUser.name(authTicket.name);
    //    currentUser.roles(authTicket.roles);
    //    currentUser.loggedIn(true);
    //}

    var viewModel = {
        currentUser: ko.observable(currentUser),
    };

    viewModel.startLogin = function () {
        rR.router.navigate('auth/login', { trigger: true });
    }

    // Public methods
    function init(root) {
        _root = root;
        ko.applyBindings(viewModel, document.body);
        Backbone.history.start({ pushState: Modernizr.history, root: root });
    }

    function loadModule(name) {
        var def = $.Deferred();
        $LAB.script(_root + 'Client/reviewR.' + name + '.js').wait(function () { def.resolve(); });
        return def;
    }

    function ensureModule(name) {
        if (!rR[name]) {
            return loadModule(name);
        }
        // Nothing to do, just resolve the deferred before even sending it to the caller
        return $.Deferred().resolve().promise();
    }

    $.extend(rR, {
        init: init,
        ensureModule: ensureModule,
        loadModule: loadModule
    });

    // Routing
    rR.router = new (Backbone.Router.extend({
        routes: {
            ':ctl/:act': 'main',
            ':ctl/:act/*args': 'mainWithArgs',
        },
        main: mainRouter,
        mainWithArgs: mainRouter
    }));
})();