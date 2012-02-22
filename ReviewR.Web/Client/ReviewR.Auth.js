/// <reference path="../Scripts/jquery-1.7.1.js" />
/// <reference path="../Scripts/bootstrap.js" />

(function ($) {
    $(function () {
        var dialogCacheKey = "dialog_cached";

        var dialogSubmitHandler = function () {
        };

        var loadDialog = function ($link, url) {
            /// <param name="$link" type="jQuery" />
            var separator = url.indexOf('?') >= 0 ? '&' : '?';

            // Cache empty jQuery just in case we get called again
            var $dialog = $();
            $link.data(dialogCacheKey, $dialog);

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
                 $link.data(dialogCacheKey, $dialog);
             });
        }

        var loadAndShowDialog = function ($link) {
            /// <param name="$link" type="jQuery" />

            // Check the data for a cached copy
            var $dialog = $link.data(dialogCacheKey);
            if (!$dialog) {
                // No cache, load it
                loadDialog($link, $link.attr('href'));
            }
            else {
                // Cache found! Show it!
                $dialog.modal('show');
            }
        }

        $('#master-nav a[data-dialog]').each(function () {
            $(this).click(function(e) {
                loadAndShowDialog($(this));
                e.preventDefault();
            });
        });
    });
})(jQuery);