var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
define(["require", "exports", '../fx/syrah', '../rR.models'], function(require, exports, __sy__, __models__) {
    var sy = __sy__;

    var models = __models__;

    (function (FileChangeType) {
        FileChangeType._map = [];
        FileChangeType.Added = 0;
        FileChangeType.Modified = 1;
        FileChangeType.Removed = 2;
    })(exports.FileChangeType || (exports.FileChangeType = {}));

    (function (LineChangeType) {
        LineChangeType._map = [];
        LineChangeType.Same = 0;
        LineChangeType.Added = 1;
        LineChangeType.Removed = 2;
        LineChangeType.HunkHeader = 3;
    })(exports.LineChangeType || (exports.LineChangeType = {}));

    var CommentViewModel = (function (_super) {
        __extends(CommentViewModel, _super);
        function CommentViewModel(owner, line, init) {
            var _this = this;
                _super.call(this);
            this.owner = owner;
            this.line = line;
            this.id = ko.observable(init.id || 0);
            this.body = ko.observable(init.body || '');
            this.author = ko.observable();
            this.isAuthor = ko.observable(init.isAuthor || false);
            this.postedOn = ko.observable();
            if(init.postedOn) {
                this.postedOn(new Date(init.postedOn));
            }
            if(init.author) {
                this.author(new models.UserReference(init.author));
            }
            this.displayPostedOn = ko.computed(function () {
                return $.timeago(_this.postedOn());
            });
        }
        CommentViewModel.prototype.deleteComment = function () {
            $.ajax('~/api/v1/comments/' + this.id(), {
                type: 'delete'
            }).done(function () {
                this.line.comments.remove(self);
            }).fail(function (xhr) {
                switch(xhr.status) {
                    case 404: {
                        alert('no such comment!');
                        break;

                    }
                    case 403: {
                        alert('hey! you can\'t delete someone else\'s comment!');
                        break;

                    }
                }
            });
        };
        return CommentViewModel;
    })(sy.ViewModelBase);
    exports.CommentViewModel = CommentViewModel;    
    var DiffLineViewModel = (function (_super) {
        __extends(DiffLineViewModel, _super);
        function DiffLineViewModel(init, owner) {
                _super.call(this);
        }
        return DiffLineViewModel;
    })(sy.ViewModelBase);
    exports.DiffLineViewModel = DiffLineViewModel;    
    var ViewReviewViewModel = (function (_super) {
        __extends(ViewReviewViewModel, _super);
        function ViewReviewViewModel() {
            _super.apply(this, arguments);

        }
        return ViewReviewViewModel;
    })(sy.ViewModelBase);
    exports.ViewReviewViewModel = ViewReviewViewModel;    
})

//@ sourceMappingURL=rR.m.reviews.js.map
