"use strict";
var AppWeb = AppWeb || {};

AppWeb.BuildersCapital = function () {
    var $_inlineUpload = $('#doUpload'),
        $_upload = $('#uploadGo'),
        $_cancel = $('#uploadCancel'),
        $_grid = $('#appDataGrid'),
        _docData = [],
        
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
                    if (result.error == 0) {
                        _docData = result.data;
                        var docGrid = createGrid(_docData);
                        $_grid.html(docGrid);
                    }
                    else {
                        alert(result.message);
                    }
                },
                error: function (jqXHR, status, errorThrown) {
                    alert('error');
                }
            });
        },

        download = function (id) {
            // search data source for id
            const foundDocument = _docData.find(document => document.Id === id);
            // retrieve blob
            // download blob
            //var xhr = new XMLHttpRequest();
            //xhr.responseType = "arraybuffer";

            //xhr.onload = function () {
            //    if (this.status === 200) {
            //        var blob = new Blob(foundDocument.DocBlob, { type: "application/vnd.openxmlformats-officedocument.wordprocessingml.document" });
            //        var objectUrl = URL.createObjectURL(blob);
            //        window.open(objectUrl);
            //    }
            //};
            //xhr.send();

            var blob = new Blob(foundDocument.DocBlob, { type: "application/zip" });
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            var fileName = foundDocument.Id;
            link.download = fileName;
            link.click();

            //TODO: DO DOWNLOAD
            //dataItem.DocBlob
        },

        createGrid = function (data) {
            var html = '';
            html += initTable();
            for (var i = 0; i < data.length; i++) {
                html += createRow(data[i]);
            }
            html += claseTable();

            return html;
        },

        initTable = function () {
            return '<table class="doctype" cellpadding=0 cellspacing=0><tr><th>PropertyId</th><th>DocType</th><th>FileName</th><th>DocBlob</th></tr>';
        },

        createRow = function (data) {
            var cell1 = '<td class="doccell"><span>' + data.PropertyId + '</span></td>';
            var cell2 = '<td class="doccell"><span>' + data.DocType + '</span></td>';
            var cell3 = '<td class="doccell"><span>' + data.FileName + '</span></td>';
            // var cell4 = `${data.Id}`
            var cell4 = '<td class="doccell"><button onclick="AppWeb.BuildersCapital.download(\'' + data.Id + '\')">Download</button></td>';
            return '<tr class="docrow">' + cell1 + cell2 + cell3 + cell4 + '</tr>';
        },

        claseTable = function () {
            return '</table>'
        }


    return {
        init: init,
        download: download,
    }

}();