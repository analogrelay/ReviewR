// Configure requirejs
requirejs.config({
    baseUrl: '/js',
    paths: {
        // Add module names here if paths need to be overridden
    },
    packages: [
        // Add an entry for every package used.
        { name: 'backbone.lite', main: 'backbone.lite' },
        { name: 'bootstrap', main: 'bootstrap' },
        { name: 'jquery', main: 'jquery-1.7.2' },
        { name: 'ko', main: 'knockout-2.1.0' },
        { name: 'signals', main: 'signals' },
        { name: 'underscore', main: 'underscore' },
        'syrah',
        'rR'
    ]
});

define('window', [], function () {
    return window;
});

require(['rR', 'jquery'], function (rR, $) {
    $(function () {
        rR.start();
    });
});