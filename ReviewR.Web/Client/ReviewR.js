/// <reference path="../Scripts/jquery-1.7.1.js" />
/// <reference path="../Scripts/jquery.timeago.js" />
/// <reference path="../Scripts/bootstrap.js" />
/// <reference path="../Scripts/Kudu.DiffViewer.js" />

var reviewR = {};

(function ($, reviewR) {
    var attachers = {
        'confirm': function(val) {
            $(this).on('click', function(evt) {
                if (!evt.isDefaultPrevented()) {
                    if (!confirm(val)) {
                        evt.preventDefault();
                    }
                }
            });
        },
        'method': function(val) {
            $(this).on('click', function (evt) {
                if (!evt.isDefaultPrevented()) {
                    var $form = $('<form />');
                    $form.attr('action', $(this).attr('href'))
                    $form.attr('method', val)
                    $form.appendTo('body')
                    $form[0].submit();
                    evt.preventDefault();
                }
            });
        }
    };

    $(function () {
        reviewR.diffViewer = $('#viewer').diffViewer({
            templates: { diff: $('#diffViewer_files') },
            readonly: false
        });

        $('abbr.timeago').timeago();
        $('.attach').each(function () {
            var $elem = $(this);
            $.each(attachers, function (key, value) {
                var dval = $elem.data(key);
                if (dval) {
                    value.apply($elem, [ dval ]);
                }
            });
        });
    });

    reviewR.receiveDiffData = function (data) {
        var diff = $.parseJSON(data);
        reviewR.diffViewer.refresh(diff);
    };
})(jQuery, reviewR);