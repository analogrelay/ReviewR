/// <reference path="namespace.js" />
/// <reference path="syrah.plugins.js" />
(function (undefined) {
    "use strict";
    function DomAbstractionLayer(currentAccessor) {
        var self = this;
        if (currentAccessor) {
            syrah.plugins.Proxy.apply(self, [currentAccessor]);
        }

        self.querySelector = function (selector) {
            if (self.current && self.current().querySelector) {
                return self.current().querySelector(selector);
            } else {
                throw 'Override querySelector(selector)';
            }
        };

        self.initDialog = function (element, onHide) {
            if (self.current && self.current().initDialog) {
                return self.current().initDialog(element, onHide);
            } else {
                throw 'Override initDialog(element, onHide)';
            }
        };

        self.showDialog = function (element) {
            if (self.current && self.current().showDialog) {
                return self.current().showDialog(element);
            } else {
                throw 'Override showDialog(element)';
            }
        };

        self.closeDialog = function (element) {
            if (self.current && self.current().closeDialog) {
                return self.current().closeDialog(element);
            } else {
                throw 'Override closeDialog(element)';
            }
        };
    }
    syrah.plugins.add('dom', DomAbstractionLayer);

    syrah.plugins.dom.extend('DefaultDomAbstractionLayer', function () {
        var self = this;
        
        self.querySelector = function (selector) {
            return $(selector)[0];
        }
        self.initDialog = function (element, onHide) {
            $(element).on('hidden', onHide);
            $(element).modal({ show: false });
        }
        self.showDialog = function (element) {
            $(element).modal('show');
        }
        self.closeDialog = function (element) {
            $(element).modal('hide');
        }
    }, function () {
        return jQuery && jQuery.fn.modal;
    });
})();