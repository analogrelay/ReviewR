/// <reference path="../ref/jquery.d.ts" />
/// <reference path="../ref/jquery.bootstrap.d.ts" />
export function querySelector(selector: string): HTMLElement {
    if (document.querySelector) {
        return <HTMLElement>document.querySelector(selector);
    } else if ($) {
        var $results = $(selector);
        if ($results.length > 0) {
            return $results[0];
        }
    }

    throw new Error('querySelector requires HTML5 or jQuery.');
}

export function initDialog(element: Element, onHide: (evt: JQueryEventObject) => void ) {
    $(element).on('hidden', onHide);
    $(element).modal({ show: false });
}

export function showDialog(element: Element) {
    $(element).modal('show');
}

export function hideDialog(element: Element) {
    $(element).modal('hide');
}