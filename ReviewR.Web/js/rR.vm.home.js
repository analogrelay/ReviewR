/// <reference path="rR.app.js" />
/// <reference path="rR.utils.js" />
/// <reference path="rR.models.js" />

// rR.vm.home.js
// Home page
(function (window, undefined) {
    "use strict";

    // Models
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
                            self.createdReviews.removeAll();
                            for (var i = 0; i < data.created.length; i++) {
                                self.createdReviews.push(rR.models.review(data.created[i]));
                            }
                        }
                        if (data.assigned) {
                            self.assignedReviews.removeAll();
                            for (var i = 0; i < data.assigned.length; i++) {
                                self.assignedReviews.push(rR.models.review(data.assigned[i]));
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