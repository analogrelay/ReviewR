var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
define(["require", "exports", '../fx/syrah', '../fx/syrah.bus', '../rR.models', '../rR.app'], function(require, exports, __sy__, __sybus__, __models__, __rR__) {
    var sy = __sy__;

    var sybus = __sybus__;

    var models = __models__;

    var rR = __rR__;

    var DashboardViewModel = (function (_super) {
        __extends(DashboardViewModel, _super);
        function DashboardViewModel() {
            _super.apply(this, arguments);

            this.createdReviews = ko.observableArray();
            this.assignedReviews = ko.observableArray();
            this.loading = ko.observable(false);
        }
        DashboardViewModel.prototype.showNewReview = function () {
            sybus.get('exec').publish('home.createReview');
        };
        DashboardViewModel.prototype.load = function () {
            this.loading(true);
            $.get('~/api/v1/my/reviews').done(function (data) {
                if(data.created) {
                    this.createdReviews.removeAll();
                    for(var i = 0; i < data.created.length; i++) {
                        this.createdReviews.push(new models.Review(data.created[i]));
                    }
                }
                if(data.assigned) {
                    this.assignedReviews.removeAll();
                    for(var i = 0; i < data.assigned.length; i++) {
                        this.assignedReviews.push(new models.Review(data.assigned[i]));
                    }
                }
            }).always(function () {
                this.loading(false);
            });
        };
        return DashboardViewModel;
    })(sy.ViewModelBase);
    exports.DashboardViewModel = DashboardViewModel;    
    var CreateReviewViewModel = (function (_super) {
        __extends(CreateReviewViewModel, _super);
        function CreateReviewViewModel() {
            _super.apply(this, arguments);

            this.title = ko.observable('').required('Title is required');
            this.description = ko.observable('');
            this.customError = ko.observable('');
        }
        CreateReviewViewModel.prototype.createReview = function () {
            this.validate();
            if(this.isValid()) {
                $.ajax('~/api/v1/reviews', {
                    type: 'post',
                    data: {
                        title: this.title(),
                        description: this.description()
                    },
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
                            if(!data.id) {
                                this.customError('Uurp... something bad happened on the server.');
                            } else {
                                sybus.get('navigate').publish('/reviews/' + data.id);
                            }
                        }
                    }
                });
            }
        };
        return CreateReviewViewModel;
    })(sy.DialogViewModelBase);
    exports.CreateReviewViewModel = CreateReviewViewModel;    
    var HomeModule = (function (_super) {
        __extends(HomeModule, _super);
        function HomeModule() {
                _super.call(this, 'home');
            this.page('dashboard', DashboardViewModel);
            this.action('createReview', function () {
                this.showDialog('newReview');
            });
            this.dialog('newReview', CreateReviewViewModel);
            this.route('Home', /^$/, function () {
                if(rR.app.currentUser().loggedIn()) {
                    this.openPage('dashboard');
                } else {
                    this.closePage();
                }
            });
        }
        return HomeModule;
    })(sy.Module);
    exports.HomeModule = HomeModule;    
    rR.module(HomeModule);
})

//@ sourceMappingURL=rR.m.home.js.map
