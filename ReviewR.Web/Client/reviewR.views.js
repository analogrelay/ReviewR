if (!window.rR) {
    window.rR = {};
}

if (!window.rR.views) {
    window.rR.views = {};
}

(function () {
    "use strict";

    var _init = false;
    var _host = $();
    var _viewCache = {};
    var _stack = [];
    var _viewTypes = {};

    // Public functions
    function init($host) {
        /// <param name="$host" type="jQuery" />
        _host = $host;
        _host.empty();
    }

    function insert(name, viewLoader) {
        /// <param name="name" type="String" />
        /// <param name="viewLoader" type="Function" />
        /// <returns type="jQuery.Deferred" />
        var view = _viewCache[name];
        var promise;
        var def = $.Deferred();
        if (!view) {
            promise = viewLoader()
                .then(function ($view) {
                    view = {
                        init: false,
                        $content: $view
                    };
                    _viewCache[name] = view;
                });
        } else {
            promise = $.Deferred().resolve().promise();
        }
        promise.then(function () {
            // Figure out the view type
            var viewType = view.$content.data('view-type');
            var viewObj;
            if (!viewType || !_viewTypes.hasOwnProperty(viewType)) {
                viewObj = defaultAttacher(view.$content);
            } else {
                var vt = _viewTypes[viewType];
                if (!view.init) {
                    if (vt.init) {
                        vt.init(_host, view.$content);
                    }
                    view.init = true;
                }
                viewObj = vt.attach(_host, view.$content);
                _stack.push({
                    detach: function () {
                        vt.detach($host, view.$content);
                    }
                });
            }
            def.resolve(view.$content);
        });
        return def.promise();
    }

    function detachCurrent() {
        if (_stack.length === 0) {
            return;
        }
        var view = _stack.pop();
        view.detach(_host);
    }

    function defaultAttacher($view) {
        /// <param name="$view" type="jQuery" />

        // Remove the current view
        detachCurrent();

        _host.append($view);
        return {
            detach: function () {
                $view.detach();
            }
        };
    }

    function addType(name, obj) {
        if (_viewTypes.hasOwnProperty(name)) {
            throw "View Type '" + name + "' already registered...";
        }
        _viewTypes[name] = obj;
    }
    addType('modal', {
        init: function ($host, $v) {
            /// <param name="$host" type="jQuery" />
            /// <param name="$v" type="jQuery" />
            $host.append($v);
            $v.addClass('modal fade');
            $v.modal({ show: false });
        },
        attach: function ($host, $v) {
            /// <param name="$host" type="jQuery" />
            /// <param name="$v" type="jQuery" />
            // We don't remove the current view in our attacher, since the modal should appear in front of the current view
            $v.on('hidden', function () {
            });
            $v.modal('show');
        },
        detach: function ($host, $v) {
            /// <param name="$host" type="jQuery" />
            /// <param name="$v" type="jQuery" />
            $v.one('hidden', function () {
                $v.detach();
            });
            $v.modal('hide');
        }
    });

    $.extend(rR.views, {
        init: init,
        insert: insert,
        addType: addType
    });
})();