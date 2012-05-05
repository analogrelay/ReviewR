/// <reference path="../rR.app.js" />
/// <reference path="../rR.models.js" />

(function (sy, rR) {
    var users = rR.module('users', function () {
        this.route('profile', 'users/:id', function (id) {
            alert('Sorry, not yet implemented...');
            history.back();
        });
    });
})(syrah, rR);