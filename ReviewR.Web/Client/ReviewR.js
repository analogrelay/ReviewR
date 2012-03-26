if (!window.rR) {
    window.rR = {};
}

(function () {
    "use strict";

    // Restore data from storage
    var authCookie = $.cookie('ReviewRAuth');

    // Parse cookie
    var authTicket = JSON.parse(authCookie);

    var currentUser = {
        id: ko.observable(0),
        name: ko.observable(''),
        roles: ko.observableArray([]),
        loggedIn: ko.observable(false)
    }
    currentUser.isAdmin = ko.computed(function () { return _.indexOf(currentUser.roles(), 'admin') > -1; });

    if (authTicket) {
        currentUser.id(authTicket.id);
        currentUser.name(authTicket.name);
        currentUser.roles(authTicket.roles);
        currentUser.loggedIn(true);
    }

    var viewModel = {
        currentUser: ko.observable(currentUser),
    };

    viewModel.startLogin = function () {
        rR.ensureModule('auth')
          .then(function () {
              rR.auth.startLogin();
          });
    }


    // Public methods
    function init() {
        ko.applyBindings(viewModel, document.body);
    }

    function loadModule(name) {
        var def = $.Deferred();
        $LAB.script('Client/reviewR.' + name + '.js').wait(function () { def.resolve(); });
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
})();