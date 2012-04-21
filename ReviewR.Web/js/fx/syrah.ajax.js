(function (sy, undefined) {
    "use strict";

    namespace.define('syrah.ajax', function (ns) {
        var _rootUrl;
        var _authToken;
        var _refreshToken;
        var _refreshUrl;

        function createSyXHR(def) {
            /// <param name="def" type="jQuery.Deferred" />
            var _statusHandlers = {};
            def.on = function (statusCode, handler) {
                if(!_statusHandlers[statusCode]) {
                    _statusHandlers[statusCode] = [];
                }
                _statusHandlers[statusCode].push(handler);
            }
            def.raiseStatus = function(statusCode, settings, data, textStatus, xhr) {
                var handlers = _statusHandlers[statusCode];
                if(handlers) {
                    for(var i = 0; i < handlers.length; i++) {
                        handlers[i].apply(settings, [data, textStatus, xhr]);
                    }
                }
            }
            return def;
        }
        
        function _doAjax(deferred, url, options) {
            var xhr = $.ajax(url, options);
            xhr.done(function () {
                deferred.resolveWith(this, Array.prototype.slice.apply(arguments));
            });
            xhr.fail(function () {
                deferred.rejectWith(this, Array.prototype.slice.apply(arguments));
            });
            return xhr;
        }

        ns.init = function (root) {
            _rootUrl = root;
        }

        ns.setAuth = function (authToken, refreshToken, refreshUrl) {
            _authToken = authToken;
            _refreshToken = refreshToken;
            _refreshUrl = refreshUrl;
        }

        ns.refreshAuth = function () {
            return $.post(_refreshUrl)
                .done(function (data) {
                    sy.bus.auth.setToken(data.user, data.token);
                });
        }

        ns.invoke = function (url, options) {
            var def = createSyXHR($.Deferred());
            var xhr = _doAjax(def, url, options);
            if (_authToken && _refreshToken && _refreshUrl) {
                xhr.statusCode({
                    403: function () {
                        if (!this.retrying) {
                            _authToken = null;
                            // Refresh the auth token
                            syrah.ajax.refreshAuth().done(function () {
                                if (_authToken) {
                                    this.retrying = true;
                                    _doAjax(def, url, this);
                                }
                            });
                        }
                    }
                });
            }
            return $.extend(def.promise(), {
                on: def.on
            });
        };

        ns.get = function (url, options) {
            if (typeof url === 'object') {
                options = url;
                url = undefined;
            }
            if (options === undefined) {
                options = {};
            }
            options.type = 'get';
            return sy.ajax.invoke(url, options);
        }

        ns.post = function (url, options) {
            if (typeof url === 'object') {
                options = url;
                url = undefined;
            }
            if (options === undefined) {
                options = {};
            }
            options.type = 'post';
            return sy.ajax.invoke(url, options);
        }

        self.resolveUrl = function (vpath) {
            /// <param name="vpath" type="String" />
            if (vpath[0] === '~' && vpath[1] === '/') {
                return _rootUrl + vpath.substr(2);
            }
            return vpath;
        };

        // Prefilter jquery ajax requests
        $.ajaxPrefilter(function (options) {
            options.url = self.resolveUrl(options.url);
            if (_authToken) {
                if (!options.headers) { options.headers = {}; }
                options.headers['Authorization'] = 'Basic ' + _authToken;
            }
        });
    });
})(syrah);