/// <reference path="rR.app.js" />
/// <reference path="rR.utils.js" />
/// <reference path="rR.models.js" />

// rR.vm.auth.js
// Authentication and login code
(function (window, undefined) {
    "use strict";

    // Modals
    var index = (function () {
        var self = rR.models.page({});
        return self;
    })();

    rR.publish('vm.home', {
        index: index
    });
})(window);