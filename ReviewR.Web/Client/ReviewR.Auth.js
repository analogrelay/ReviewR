/// <reference path="reviewR.js" />
if (!window.rR) {
    window.rR = {};
}
if (!window.rR.auth) {
    window.rR.auth = {};
}

(function () {
    "use strict";

    function login() {
        alert('login!');
    }

    $.extend(rR.auth, {
        login: login
    });
})();