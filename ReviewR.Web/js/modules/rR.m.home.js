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
    	    sy.bus.exec.publish('home.createReview');
    	};

    	self.load = function () {
    		self.loading(true);
    		$.get('~/api/v1/my/reviews')
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

    function CreateReviewViewModel() {
        var self = this;
        sy.DialogViewModel.apply(this);
        
        self.title = ko.observable('').required('Title is required');
        self.description = ko.observable('');
        ko.validation.addValidation(self);
        
        self.createReview = function () {
            self.validate();
            if (self.isValid()) {
                $.ajax({
                    url: '~/api/v1/reviews',
                    type: 'post',
                    data: { title: self.title(), description: self.description() },
                    statusCode: {
                        401: function () {
                            self.customError("You aren't logged in! How did that happen?");
                        },
                        400: function () {
                            self.customError('Whoops, there were some errors :(');
                        },
                        500: function () {
                            self.customError('Uurp... something bad happened on the server.');
                        },
                        201: function (data) {
                            if (!data.id) {
                                self.customError('Uurp... something bad happened on the server.');
                            } else {
                                sy.bus.navigate.publish('/reviews/' + data.id);
                            }
                        }
                    }
                });
            }
        }
    }

    var home = rR.module('home', function () {
    	var self = this;
    	self.page('dashboard', DashboardViewModel);

    	self.action('createReview', function () {
    	    this.showDialog('newReview');
    	});

    	self.dialog('newReview', CreateReviewViewModel);

    	self.route('Home', /^$/, function () {
    	    if (rR.app.currentUser().loggedIn()) {
    	        this.openPage('dashboard');
            } else {
                this.closePage();
    	    }
        });
    });
})(syrah, rR);