"use strict";
var AppWeb = AppWeb || {};

AppWeb.BuildersCapital = function () {
    var $_inlineUpload = $('#doUpload'),
        $_upload = $('#uploadGo'),
        $_cancel = $('#uploadCancel'),
        
        init = function () {
            installEvents();
        },

        installEvents = function () {
            $_inlineUpload.removeClass('hide');

            $_inlineUpload.unbind('click').on('click', function (e) {
                doUpload();
            });

            $_upload.addClass('hide'); // use inline style button
            $_cancel.addClass('hide'); // no cancel button
        },


        doUpload = function () {
            // ensure a file is selected
            var pathname = $('#UploadFile').val();
            if (pathname == '') {
                AppWeb.ActionAlert.fail('Please select an Excel file to import.');
                return;
            }

            var formData = new FormData();
            // add the form fields defined im UploadFileModel
            formData.append('FileType', 'Json');
            formData.append('UploadFile', pathname);
            var importFileUpload = $("#fileToUpload").get(0);
            formData.append('fileToUpload', importFileUpload.files[0]); // the import file

            $.ajax({
                type: 'POST',
                url: '/Document/Upload',
                contentType: false, // Not to set any content header if FormData() is used
                processData: false, // Not to process data if FormData() is used
                data: formData,
                success: function (result) {
                },
                error: function (jqXHR, status, errorThrown) {
                }
            });
        }


    return {
        init: init,
    }

}();