"use strict";
var AppWeb = AppWeb || {};

AppWeb.DocGrid = function () {
    var createGrid = function (data) {
            var html = '';
            html += initTable();
            for (var i = 0; i < data.length; i++) {
                html += createRow(data[i]);
            }
            html += claseTable();

            return html;
        },

        initTable = function () {
            return '<table class="doctype" cellpadding=0 cellspacing=0>';
        },

        createRow = function (data) {
            var cell1 = '<td class="doccell"><span>' + data.PropertyId + '</span></td>';
            var cell2 = '<td class="doccell"><span>' + data.DocType + '</span></td>';
            var cell3 = '<td class="doccell"><span>' + data.FileName + '</span></td>';
            var cell4 = '<td class="doccell"><button onclick="download("' + data.Id + '")</button></td>';
            return '<tr class="docrow">' + cell1 + cell2 + cell3 + cell4 + '</tr>';
        },

        claseTable = function () {
            return '</table>'
        }

    return {
        createGrid: createGrid
    }
}
