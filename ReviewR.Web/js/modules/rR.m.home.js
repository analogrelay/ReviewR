/// <reference path="../rR.app.js" />
/// <reference path="../rR.models.js" />
/// <reference path="../fx/syrah.ajax.js" />
/// <reference path="../fx/syrah.js" />

// Home page
(function (sy, rR, undefined) {
    "use strict";

    // Models
    function DashboardViewModel() {
        var self = this;
        
        self.createdReviews = ko.observableArray([])
    	self.assignedReviews = ko.observableArray([])
    	self.loading = ko.observable(false);

    	self.showNewReview = function () {
    	};

    	self.refresh = function () {
    		self.loading(true);
    		sy.ajax.get('~/api/v1/my/reviews')
				.done(function (data) {
					if (data.created) {
						self.createdReviews.removeAll();
						for (var i = 0; i < data.created.length; i++) {
							self.createdReviews.push(new rR.models.Review(data.created[i]));
						}
					}
					if (data.assigned) {
						self.assignedReviews.removeAll();
						for (var i = 0; i < data.assigned.length; i++) {
							self.assignedReviews.push(new rR.models.Review(data.assigned[i]));
						}
					}
				})
				.always(function () { self.loading(false); });
    	}
    	return self;
    };

    var home = rR.module('home', function () {
    	var self = this;
    	self.page('dashboard', DashboardViewModel)
			.injected.add(function (view, model) {
				model.refresh();
			});

    	self.route('Home', '', function () {
    	    if (rR.app.currentUser().serverVerified()) {
    	        self.openPage('dashboard');
            }
        });
    });
})(syrah, rR);