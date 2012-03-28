if (!rR.c.home) {
    rR.c.home = {};
}

(function () {
    var c = new rR.ControllerBuilder('home', rR.c.home);
    
    c.action('index', function () {
        c.view();
    });
})();