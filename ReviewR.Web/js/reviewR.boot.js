/// <reference path="reviewR.js" />

// reviewR.boot.js
// Startup code

if (!window.rR) {
    throw 'reviewR.js must be imported before reviewR.boot.js';
}

(function (rR) {
    "use strict";

    // To avoid users being able to call methods using url hacking, we have a list of known controllers:
    var _root = '/';

    // Public methods
    function start(root, env) {
        _root = root;

        var homePage = rR.models.page({
            view: 'Home.Index',
            model: {}
        });

        var viewModel = rR.models.system({
            environment: env,
            activePage: homePage
        });

        ko.applyBindings(viewModel, document.body);
    }

    $.extend(rR, {
        start: start
    });
})(window.rR);