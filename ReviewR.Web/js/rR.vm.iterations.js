/// <reference path="rR.app.js" />
/// <reference path="rR.utils.js" />
/// <reference path="rR.models.js" />

// rR.vm.reviews.js
// Review management
(function (window, undefined) {
    "use strict";

    // Modals
    var create = (function () {
        var self = rR.models.dialog();
        ko.validation.validatableModel(self);
        return self;
    })();

    rR.publish('vm.iterations', {
        create: create
    });
})(window);