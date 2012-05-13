define(function () {
    "use strict";
    var exports = {};

    // User View model
    exports.UserReference = function (init) {
        init = init || {};
        var self = this;

        self.id = ko.observable(init.id || 0);
        self.email = ko.observable(init.email || '');
        self.emailHash = ko.observable(init.emailHash || '');
        self.displayName = ko.observable(init.displayName || '');
        self.gravatarUrl = ko.computed(function () {
            return 'http://www.gravatar.com/avatar/' + self.emailHash() + '?s=16';
        });
    }

    exports.User = function (init) {
        init = init || {};
        var self = this;
        rR.models.UserReference.apply(this, [init]);

        // Fields
        self.token = '';
        self.roles = ko.observableArray(init.roles || []);
        self.loggedIn = ko.observable(init.loggedIn || false);
        self.isAdmin = ko.computed(function () {
            return $.inArray('Admin', self.roles()) > -1;
        });

        return self;
    }

    exports.Review = function (init) {
        init = init || {};
        var self = {};

        self.id = ko.observable(init.id);
        self.title = ko.observable(init.title);
        self.authorName = ko.observable(init.authorName);
        self.authorEmail = ko.observable(init.authorEmail);
        self.authorEmailHash = ko.observable(init.authorEmailHash);

        self.authorGravatarUrl = ko.computed(function () {
            return 'http://www.gravatar.com/avatar/' + self.authorEmailHash() + '?s=16';
        });

        self.url = ko.computed(function () { return 'reviews/' + self.id(); });

        return self;
    }
});