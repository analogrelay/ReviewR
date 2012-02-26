/// <reference path="../Scripts/jquery-1.7.1.js" />
/// <reference path="../Scripts/bootstrap.js" />

(function ($) {
    $(function () {
        var dialogs = [];

        var getValidationSummaryErrors = function ($form) {
            // We verify if we created it beforehand
            var errorSummary = $form.find('.validation-summary-errors, .validation-summary-valid');
            if (!errorSummary.length) {
                errorSummary = $('<div class="validation-summary-errors"><ul></ul></div>')
                .prependTo($form);
            }

            return errorSummary;
        };

        var displayErrors = function (form, errors) {
            var errorSummary = getValidationSummaryErrors(form)
            .removeClass('validation-summary-valid')
            .addClass('validation-summary-errors');

            var items = $.map(errors, function (error) {
                return '<li class="alert alert-error">' + error + '</li>';
            }).join('');

            var ul = errorSummary
            .find('ul')
            .empty()
            .append(items);
        };

        var dialogSubmitHandler = function (e) {
            var $form = $(this);

            // We check if jQuery.validator exists on the form
            if (!$form.valid || $form.valid()) {
                $.post($form.attr('action'), $form.serializeArray())
                .done(function (json) {
                    json = json || {};

                    // In case of success, we redirect to the provided URL or the same page.
                    if (json.success) {
                        location = json.redirect || location.href;
                    } else if (json.errors) {
                        displayErrors($form, json.errors);
                    }
                })
                .error(function () {
                    displayErrors($form, ['An unknown error happened.']);
                });
            }

            // Prevent the normal behavior since we opened the dialog
            e.preventDefault();
        };

        var loadDialog = function ($link, url) {
            /// <param name="$link" type="jQuery" />
            var separator = url.indexOf('?') >= 0 ? '&' : '?';

            // Cache empty jQuery just in case we get called again
            var $dialog = $();
            dialogs[$link.data('dialog-key')] = $dialog;

            // Load the dialog
            $.get(url + separator + 'content=1')
             .done(function (content) {
                 $dialog = $('<div class="fade modal">' + content + '</div>')
                    .hide()
                    .appendTo(document.body)
                    .filter('div')
                    .find('hgroup')
                        .addClass('modal-header')
                        .end()
                    .find('form')
                        .find('fieldset')
                            .addClass('modal-body')
                            .end()
                        .submit(dialogSubmitHandler)
                        .find('.modal-bar')
                            .addClass('modal-footer')
                            .find('input')
                                .addClass('pull-right')
                                .end()
                            .end()
                    .end()
                    .modal();
                 dialogs[$link.data('dialog-key')] = $dialog;
             });
        }

        var loadAndShowDialog = function ($link) {
            /// <param name="$link" type="jQuery" />

            // Check the data for a cached copy
            var $dialog = dialogs[$link.data('dialog-key')]
            if (!$dialog) {
                // No cache, load it
                loadDialog($link, $link.attr('href'));
            }
            else {
                // Cache found! Show it!
                $dialog.modal('show');
            }
        }

        $('.dialog').each(function () {
            $(this).click(function(e) {
                loadAndShowDialog($(this));
                e.preventDefault();
            });
        });
    });
})(jQuery);