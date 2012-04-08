/// <reference path="rR.app.js" />
/// <reference path="rR.utils.js" />
/// <reference path="rR.models.js" />

// rR.vm.home.js
// Home page
(function (window, undefined) {
    "use strict";

    // Modals
    function review(init) {
        init = init || {};
        var self = {};

        self.id = ko.observable(init.id);
        self.title = ko.observable(init.title);
        self.authorName = ko.observable(init.authorName);
        self.authorEmail = ko.observable(init.authorEmail);
        self.authorEmailHash = ko.observable(init.authorEmailHash);

        return self;
    }

    var index = (function () {
        var self = rR.models.page();
        return self;
    })();

    var dashboard = (function () {
        var self = rR.models.page();

        self.createdReviews = ko.observableArray([])
        self.assignedReviews = ko.observableArray([])
        self.loading = ko.observable(false);

        self.refresh = function () {
            self.loading(true);
            $.ajax({
                url: '~/api/reviews',
                type: 'get',
                statusCode: {
                    200: function (data) {
                        if (data.created) {
                            for (var i = 0; i < data.created.length; i++) {
                                self.createdReviews.push(review(data.created[i]));
                            }
                        }
                        if (data.assigned) {
                            for (var i = 0; i < data.assigned.length; i++) {
                                self.assignedReviews.push(review(data.assigned[i]));
                            }
                        }
                    }
                },
                complete: function () { self.loading(false); }
            });
        }

        self.showNewReview = function () {
            rR.bus.showDialog.publish('reviews.create');
        }

        self.opened.add(self.refresh);

        return self;
    })();

    rR.publish('vm.home', {
        index: index,
        dashboard: dashboard
    });
})(window);