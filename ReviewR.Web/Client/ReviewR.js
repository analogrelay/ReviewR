/// <reference path="reviewR.views.js" />

if (!window.rR) {
    window.rR = {};
}

(function () {
    "use strict";

    // To avoid users being able to call methods using url hacking, we have a list of known controllers:
    var _root = '/';
    var _host = $();

    // Routes
    function mainRouter(ctl, act, args) {
        ctl = ctl || 'home';
        act = act || 'index';

        rR.ensureController(ctl)
          .then(function () {
              if (rR.c[ctl].hasOwnProperty(act)) {
                  rR.c[ctl][act].call(this, args);
              }
              else {
                  fallback();
              }
          });
    }

    //// Restore data from storage
    //var authCookie = $.cookie('ReviewRAuth');

    //// Parse cookie
    //var authTicket = JSON.parse(authCookie);

    var currentUser = {
        id: ko.observable(0),
        email: ko.observable(''),
        displayName: ko.observable(''),
        roles: ko.observableArray([]),
        loggedIn: ko.observable(false)
    }
    currentUser.isAdmin = ko.computed(function () {
        return _.indexOf(currentUser.roles(), 'admin') > -1;
    });

    var system = {
        devMode: ko.observable(true)
    };

    //if (authTicket) {
    //    currentUser.id(authTicket.id);
    //    currentUser.name(authTicket.name);
    //    currentUser.roles(authTicket.roles);
    //    currentUser.loggedIn(true);
    //}

    var viewModel = {
        currentUser: ko.observable(currentUser),
        system: ko.observable(system)
    };

    viewModel.startLogin = function () {
        rR.router.navigate('auth/login', { trigger: true });
    }

    // Public methods
    function init(root, $host) {
        _root = root;
        _host = $host;
        ko.applyBindings(viewModel, document.body);
        rR.views.init($('#host'));
        Backbone.history.start({ pushState: Modernizr.history, root: root });

        $(document).on('click', 'a', function () {
            var href = $(this).attr('href');
            if (href[0] == '/') {
                rR.router.navigate(href, { trigger: true });
                return false;
            }
        });
    }

    function loadObject(path) {
        var def = $.Deferred();
        $LAB.script(_root + path).wait(function () {
            def.resolve();
        });
        return def;
    }

    function ensureModule(name) {
        return ensureObject(rR[name], 'Client/reviewR.' + name + '.js');
    }

    function ensureController(name) {
        return ensureObject(rR.c[name], 'Controllers/' + name + '.js');
    }

    function ensureObject(expr, path) {
        if (!expr) {
            return loadObject(path);
        }
        // Nothing to do, just resolve the deferred before even sending it to the caller
        return $.Deferred().resolve().promise();
    }
    
    function injectTemplate(path, model) {
        rR.views.insert(path, function () {
            return fetchHtml(path);
        })
            .then(function($t) {
                if(model) {
                    ko.applyBindings(model, $t);
                }
            });
    }

    function fetchHtml(path) {
        var def = $.Deferred();
        $.ajax({
            url: _root + path,
            type: 'get',
            dataType: 'html',
            success: function (html, textStatus, xhr) {
                var $container = $('<div></div>');
                $container.html(html);
                def.resolve($container.find('#root'));
            }
        });
        return def.promise();
    }

    // Routing
    var router = new (Backbone.Router.extend({
        routes: {
            '': 'home',
            ':ctl/:act': 'main',
            ':ctl/:act/*args': 'mainWithArgs',
        },
        home: mainRouter,
        main: mainRouter,
        mainWithArgs: mainRouter
    }));

    // Controllers
    function ControllerBuilder(name, obj) {
        var exports = this;
        var _controllerName = name;
        var _actionName = '';
        var _obj = obj;

        exports.view = function (viewName, model) {
            viewName = viewName || _actionName;
            if(typeof(name) === 'object') {
                model = viewName;
                viewName = _actionName;
            }

            return rR.injectTemplate('Views/' + _controllerName + '/' + viewName, model);
        }

        exports.action = function(actionName, impl) {
            _obj[actionName] = function() {
                var old = _actionName;
                _actionName = actionName;
                impl.apply(obj, arguments);
                _actionName = old;
            }
        }
    };

    $.extend(rR, {
        init: init,
        ensureModule: ensureModule,
        ensureObject: ensureObject,
        ensureController: ensureController,
        injectTemplate: injectTemplate,
        c: {},
        ControllerBuilder: ControllerBuilder
    });
})();