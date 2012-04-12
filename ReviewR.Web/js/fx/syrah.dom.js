/// <reference path="namespace.js" />
(function (undefined) {
    "use strict";
    namespace.define('syrah.dom', function (ns) {
        ns.querySelector = function (selector) {
            return (_provider || initProvider()).querySelector(selector);
        }
        ns.initDialog = function (element, onHide) {
            return (_provider || initProvider()).initDialog(element, onHide);
        }
        ns.closeDialog = function (element) {
            return (_provider || initProvider()).closeDialog(element);
        }
        ns.showDialog = function (element) {
            return (_provider || initProvider()).showDialog(element);
        }

        var _provider;

        ns.setProvider = function (provider) {
            /// <param name="provider" type="syrah.dom.abstractions.DomAbstractionLayer" />
            _provider = provider;
        }

        function initProvider() {
            if (!_provider) {
                var a = syrah.dom.abstractions;
                for (var key in a) {
                    if (a.hasOwnProperty(key) && a.isAvailable && a.isAvailable()) {
                    }
                }
            }
            return _provider;
        }
    });

    namespace.define('syrah.dom.abstractions', function (ns) {
        ns.DomAbstractionLayer = function () {
            this.querySelector = function (selector) {
                throw 'Override querySelector(selector)'
            };

            this.initDialog = function (element, onHide) {
                // Don't throw, this doesn't have to be implemented if the underlying system doesn't need it
            };

            this.showDialog = function (element) {
                throw 'Override showDialog(element)';
            };
            
            this.closeDialog = function (element) {
                throw 'Override closeDialog(element)';
            };
        }

        ns.DefaultDomAbstractionLayer = function ($) {
            ns.DomAbstractionLayer.apply(this);
            this.querySelector = function (selector) {
                return $(selector)[0];
            }
            this.initDialog = function (element, onHide) {
                $(element).on('hidden', onHide);
                $(element).modal({ show: false });
            }
            this.showDialog = function (element) {
                $(element).modal('show');
            }
            this.closeDialog = function (element) {
                $(element).modal('hide');
            }
        }
        ns.DefaultDomAbstractionLayer.isAvailable = function () {
            return jQuery && jQuery.fn.modal;
        }
    });
})();