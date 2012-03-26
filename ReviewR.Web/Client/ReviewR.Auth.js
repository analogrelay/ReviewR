/// <reference path="reviewR.js" />
if (!window.rR) {
    window.rR = {};
}
if (!window.rR.auth) {
    window.rR.auth = {};
}

(function () {
    "use strict";

    function startLogin() {
        alert('startLogin!');
    }

    $.extend(rR.auth, {
        startLogin: startLogin
    });
})();