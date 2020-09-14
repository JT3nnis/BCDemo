"use strict";
var AppWeb = AppWeb || {};

AppWeb.File = function () {
    var $filenameSelector,

        initUpload = function (options) { 
            var $browseSelector = options && options.browseId ? $('#' + options.browseId) : $('#attachFile');

            var filenameId = options && options.filenameId ? options.filenameId : $browseSelector.attr('data-field');
            var uploadId = options && options.uploadId ? options.uploadId : $browseSelector.attr('data-upload');

            if (!filenameId || !uploadId) return;

            var $removeSelector = options && options.removeId ? $('#' + options.removeId) : $('#removeFile');

            $filenameSelector = $('#' + filenameId);
            var $uploadSelector = $('#' + uploadId);

            $browseSelector.unbind('click').on('click', function () {
                $uploadSelector.click(); // delegate to file select control
                $uploadSelector.change(); // force the change in case the file is the same
            });

            if ($removeSelector != undefined) {
                $removeSelector.unbind('click').on('click', function () {
                    $filenameSelector.val('');
                });
            }

            $uploadSelector.unbind('change').on('change', function () {
                var filename = $(this).val();
                if (filename != null) {
                    $filenameSelector.val(filename);
                    $filenameSelector.blur(); // allow validation to kick in without user interaction
                }
                else {
                    $filenameSelector.val('');
                }
            });
        },

        download = function (url) {
            //iOS devices do not support downloading. We have to inform user about this.
            if (/(iP)/g.test(navigator.userAgent)) {
                alert('Your device does not support files downloading. Please try desktop browser.');
                return false;
            }

            //If in Chrome or Safari - download via virtual link click
            if (AppWeb.Helpers.isChrome() || AppWeb.Helpers.isSafari()) {
                var link = document.createElement('a'); // Creating new link node.
                link.href = url;

                if (link.download !== undefined) {
                    //Set HTML5 download attribute. This will prevent file from opening if supported.
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

            // Force file download (whether supported by server) for all other browsers
            if (url.indexOf('?') === -1) {
                url += '?download';
            }

            window.open(url, '_self');
            return true;
        },

        clearFile = function () {
            $filenameSelector.val('');
        }

    return {
        initUpload: initUpload,
        download: download,
        clearFile: clearFile
    }
}();
