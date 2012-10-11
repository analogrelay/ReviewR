var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
var UserReference = (function () {
    function UserReference(init) {
        var _this = this;
        this.id = ko.observable(init.id || 0);
        this.email = ko.observable(init.email || '');
        this.emailHash = ko.observable(init.emailHash || '');
        this.displayName = ko.observable(init.displayName || '');
        this.gravatarUrl = ko.computed(function () {
            return 'http://www.gravatar.com/avatar/' + _this.emailHash() + '?s=16';
        });
    }
    return UserReference;
})();
exports.UserReference = UserReference;
var User = (function (_super) {
    __extends(User, _super);
    function User(init) {
        var _this = this;
        _super.call(this, init);
        this.roles = ko.observableArray(init.roles || []);
        this.loggedIn = ko.observable(init.loggedIn || false);
        this.isAdmin = ko.computed(function () {
            return _this.roles().indexOf('Admin') > -1;
        });
    }
    return User;
})(UserReference);
exports.User = User;
var Review = (function () {
    function Review(init) {
        var _this = this;
        this.id = ko.observable(init.id || 0);
        this.title = ko.observable(init.title || '');
        this.authorName = ko.observable(init.authorName || '');
        this.authorEmail = ko.observable(init.authorEmail || '');
        this.authorEmailHash = ko.observable(init.authorEmailHash || '');
        this.authorGravatarUrl = ko.computed(function () {
            return 'http://www.gravatar.com/avatar/' + _this.authorEmailHash() + '?s=16';
        });
        this.url = ko.computed(function () {
            return 'reviews/' + _this.id();
        });
    }
    return Review;
})();
exports.Review = Review;

//@ sourceMappingURL=rR.models.js.map
