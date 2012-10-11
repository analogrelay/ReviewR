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
exports.querySelector = querySelector;
function initDialog(element, onHide) {
    $(element).on('hidden', onHide);
    $(element).modal({
        show: false
    });
}
exports.initDialog = initDialog;
function showDialog(element) {
    $(element).modal('show');
}
exports.showDialog = showDialog;
function hideDialog(element) {
    $(element).modal('hide');
}
exports.hideDialog = hideDialog;

//@ sourceMappingURL=syrah.dom.js.map
