/// <reference path="..\ref\knockout.d.ts" />
/// <reference path="..\ref\knockout.validation.d.ts" />

import sy = module('../fx/syrah');
import sybus = module('../fx/syrah.bus');
import models = module('../rR.models');
import rR = module('../rR.app');

export class DashboardViewModel extends sy.ViewModelBase {
    public createdReviews = ko.observableArray();
    public assignedReviews = ko.observableArray();
    public loading = ko.observable(false);
    
    public showNewReview() {
        sybus.get('exec').publish('home.createReview');
    }

    public load() {
        this.loading(true);
        $.get('~/api/v1/my/reviews')
		    .done(function (data) {
			    if (data.created) {
				    this.createdReviews.removeAll();
				    for (var i = 0; i < data.created.length; i++) {
					    this.createdReviews.push(new models.Review(data.created[i]));
				    }
			    }
			    if (data.assigned) {
				    this.assignedReviews.removeAll();
				    for (var i = 0; i < data.assigned.length; i++) {
					    this.assignedReviews.push(new models.Review(data.assigned[i]));
				    }
			    }
		    })
		    .always(function () { this.loading(false); });
    }
}

export class CreateReviewViewModel extends sy.DialogViewModelBase {
    public title = ko.observable('').required('Title is required');
    public description = ko.observable('');
    public customError = ko.observable('');

    public createReview() {
        this.validate();
        if (this.isValid()) {
            $.ajax('~/api/v1/reviews', {
                type: 'post',
                data: { title: this.title(), description: this.description() },
                statusCode: {
                    401: function () {
                        this.customError("You aren't logged in! How did that happen?");
                    },
                    400: function () {
                        this.customError('Whoops, there were some errors :(');
                    },
                    500: function () {
                        this.customError('Uurp... something bad happened on the server.');
                    },
                    201: function (data) {
                        if (!data.id) {
                            this.customError('Uurp... something bad happened on the server.');
                        } else {
                            sybus.get('navigate').publish('/reviews/' + data.id);
                        }
                    }
                }
            });
        }
    }
}

export class HomeModule extends sy.Module {
    constructor () {
        super('home');

        this.page('dashboard', DashboardViewModel);

    	this.action('createReview', function () {
    	    this.showDialog('newReview');
    	});

    	this.dialog('newReview', CreateReviewViewModel);

    	this.route('Home', /^$/, function () {
            if (rR.app.currentUser().loggedIn()) {
    	        this.openPage('dashboard');
            } else {
                this.closePage();
    	    }
        });
    }
}
rR.module(HomeModule);