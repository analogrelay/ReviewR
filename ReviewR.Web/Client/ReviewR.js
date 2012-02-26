/// <reference path="../Scripts/jquery-1.7.1.js" />
/// <reference path="../Scripts/jquery.timeago.js" />
/// <reference path="../Scripts/bootstrap.js" />
/// <reference path="../Scripts/Kudu.DiffViewer.js" />

var reviewR = {};

(function ($, reviewR) {
    $(function () {
        reviewR.diffViewer = $('#viewer').diffViewer({
            templates: { diff: $('#diffViewer_files') },
            readonly: false
        });

        $('abbr.timeago').timeago();
    });

    reviewR.receiveDiffData = function (data) {
        var diff = $.parseJSON(data);
        reviewR.diffViewer.refresh(diff);
    };
})(jQuery, reviewR);