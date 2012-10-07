var syrah;
(function (syrah) {
    (function (dom) {
        function querySelector(selector) {
            if(document.querySelector) {
                return document.querySelector(selector);
            } else {
                if($) {
                    var $results = $(selector);
                    if($results.length > 0) {
                        return $results[0];
                    }
                }
            }
            throw new Error('querySelector requires HTML5 or jQuery.');
        }
        dom.querySelector = querySelector;
        function initDialog(element, onHide) {
            $(element).on('hidden', onHide);
            $(element).modal({
                show: false
            });
        }
        dom.initDialog = initDialog;
        function showDialog(element) {
            $(element).modal('show');
        }
        dom.showDialog = showDialog;
        function hideDialog(element) {
            $(element).modal('hide');
        }
        dom.hideDialog = hideDialog;
    })(syrah.dom || (syrah.dom = {}));
    var dom = syrah.dom;

})(syrah || (syrah = {}));

//@ sourceMappingURL=syrah.dom.js.map
