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
            return self.requireCurrent('querySelector', 'Override querySelector(selector)')
                       .querySelector(selector);
        };

        self.initDialog = function (element, onHide) {
            if(self.currentSupports('initDialog')) {
                return self.current().initDialog(element, onHide);
            }
        };

        self.showDialog = function (element) {
            return self.requireCurrent('showDialog', 'Override showDialog(element)')
                       .showDialog(element);
        };

        self.closeDialog = function (element) {
            return self.requireCurrent('closeDialog', 'Override closeDialog(element)')
                       .closeDialog(element);
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