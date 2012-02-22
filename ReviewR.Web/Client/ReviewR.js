/// <reference path="../Scripts/Kudu.DiffViewer.js" />

var reviewR = {};

(function ($, reviewR) {
    $(function () {
        reviewR.diffViewer = $('#viewer').diffViewer({
            templates: { diff: $('#diffViewer_files') },
            readonly: false
        });
    });

    reviewR.receiveDiffData = function (data) {
        var diff = $.parseJSON(data);
        reviewR.diffViewer.refresh(diff);
    };
})(jQuery, reviewR);