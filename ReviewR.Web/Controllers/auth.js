/// <reference path="..\Client\reviewR.js" />
if (!rR.c.auth) {
    rR.c.auth = {};
}

(function () {
    var c = new rR.ControllerBuilder('auth', rR.c.auth);

    c.action('login', function () {
        c.view();
    });
})();