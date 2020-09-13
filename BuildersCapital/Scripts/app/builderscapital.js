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
            var foundDocument = _docData.find(x => x.Id == id);

            var url = '/Document/Download?id=' + id;
            $.ajax({
                type: 'POST',
                url: url,
                contentType: false, // Not to set any content header if FormData() is used
                processData: false, // Not to process data if FormData() is used
                success: function (result) {
                    if (result.error == 0) {
                        var url = result.data;
                        // create a html5 link tag to download the file by browser
                        var link = document.createElement('a'); // Creating new link node.
                        link.type = "application/zip";
                        link.href = url;
                        if (url !== undefined) {
                            //Set HTML5 download attribute. This will prevent file from opening if supported.
                            url = url.replace(/\\/g, '/');
                            var fileName = url.substring(url.lastIndexOf('/') + 1, url.length);
                            link.download = fileName;
                        }

                        //Dispatching click event.
                        if (document.createEvent) {
                            var e = document.createEvent('MouseEvents');
                            e.initEvent('click', true, true);
                            link.dispatchEvent(e);
                            return true;
                        }
                    }
                    else {
                    }
                },
                error: function (jqXHR, status, errorThrown) {
                    alert('error');
                }
            });
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