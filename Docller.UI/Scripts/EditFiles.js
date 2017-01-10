/// <reference path="jquery-1.6.2-vsdoc.js" />
/// <reference path="jquery.validate.js" />
/// <reference path="jquery-ui-1.8.11.js" />
/// <reference path="jquery.unobtrusive-ajax.js" />
docller.editGrid = function () {
    this.grid = null;
    this.statusDropdowns = new Array();
    this.statusCache = {};
};
docller.editGrid.prototype = (function () {
    var Const = {
        AlertArea: "#editFiles-Msg",
        AlertDiv: "#editFile-Alert",
        UpdatedFiles: "#updatedFiles",
        Form: "#editFilesForm"
    };

    function autocompleteRenderer(instance, td, row, col, prop, value, cellProperties) {
        Handsontable.AutocompleteCell.renderer.apply(this, arguments);
        td.title = 'Type to show the list of options';
        var cell = $(td);
        //cell.width("15%");
    }
    
    function getStatusId(statusId) {
        for (var i = 0; i < docller.statusData.length; i++) {
            if (docller.statusData[i].statusId == statusId) {
                return docller.statusData[i].statusText;
            }
        }
        return "";
    }


    

    function cellRenderer(instance, td, row, col, prop, value, cellProperties) {
        var cell = $(td);
        //cell.addClass("editgrid-head");
       /*if (prop == "fileName") {
            cell.width("35%");
        } else if (prop == "title") {
            cell.width("45%");
        } else if (prop == "revision") {
            cell.width("5%");
        }*/
        
        if (col == 0) {
            var stringValue = Handsontable.helper.stringify(value);
            var file = instance.getData()[row];
            $(td).empty().html("<div id=\"" + file.fileInternalName + "\">" + stringValue + "</div>");

        } else {
            Handsontable.TextCell.renderer.apply(this, arguments);

        }
        
    }
    function ensureValidFileNames(files) {
        if (files.length > 0) {
            showErrorFiles(files, "File name is required", "Please provide all file names");
            return false;
        }
        return true;
    }
    
    function showErrorFiles(files, fileErrorMsg, summaryMsg) {
        for (var i = 0; i < files.length; i++) {
            $("#" + files[i].fileInternalName).append("&nbsp;&nbsp;<span class=\"label label-important\">" + fileErrorMsg + "</span>");
        }

        if (files.length > 0) {
            var alertHtml = "<div class=\"alert alert-error\" id=\"" + Const.AlertDiv + "\">" + summaryMsg + "</div>";
            $(Const.AlertArea).html(alertHtml);
           
        }
        
    }

    function transmitFiles(files) {
        var filesToTransmit = "";
        for (var i = 0; i < files.length; i++) {
            filesToTransmit = filesToTransmit + files[i].fileInternalName + "|";
        }

        $(Const.UpdatedFiles).val(filesToTransmit);
        $(Const.Form).trigger("submit");
    }

    function saveInternal(transmit) {
        var table = this.grid.data('handsontable');
        var data = table.getData();
        var invalidFiles = new Array();
        var index = 0;
        for (var i = 0; i < data.length; i++) {
            var file = data[i];

            if ($.trim(file.fileName).length == 0) {
                invalidFiles[index] = file;
                index++;
            } else {
                //data[i].statusId = this.statusCache[file.status];
            }
        }

        if (ensureValidFileNames(invalidFiles)) {
            $(Const.AlertArea).empty();

            $.ajax({
                url: "/Project/UpdateFiles/",
                type: "POST",
                cache: false,
                data: JSON.stringify(data),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                beforeSend: function () {
                    $.blockUI({ message: '<img src="/images/busy.gif" /> Please wait, saving details...' });
                },
                complete: function () {
                    $.unblockUI();
                },
                error: function (request, textStatus, errorThrown) {
                    alert(errorThrown);
                },
                success: function (result, textStatus, request) {
                    if (result.success == true) {
                        if (transmit == true) {
                            transmitFiles.call(this, data);
                        } else {
                            //redirect to the same folder
                            window.location.href = "/Project/FileRegister/" + docller.projectId + "/" + docller.folderId;
                        }
                    } else {
                        showErrorFiles.call(this, result.files,"Existing file","Please provide unique file names");
                    }
                }
            });
        }
    }
    
    return {
        
        transmit:function() {
            saveInternal.call(this, true);
        },
        save: function () {
            saveInternal.call(this, false);
        },
        
        create: function () {
            this.grid = $("#editFiles-grid");
            var $this = this;
            this.grid.handsontable({
                    data: docller.editFilesData,
                    startRows: 1,
                    startCols: 1,
                    scrollH: "none",
                    stretchH:"none",
                    colWidths: [200,300,50],
                    fillHandle: "vertical",
                    colHeaders: ["File/Document #","Title","Revision"],
                    columns: [
                                { data: "fileName", type: { renderer: cellRenderer } },
                                { data: "title", type: { renderer: cellRenderer } },
                                { data: "revision", type: { renderer: cellRenderer } }
                                //{ data: "status", type: { renderer: autocompleteRenderer } }
                    ],
                    cells: function (row, col, prop) {
                        var cellProperties = {};
                        /*if (col === 0) {
                            cellProperties.readOnly = true; //make cell read-only if it is first row or the text reads 'readOnly'
                        */
                       
                        return cellProperties;
                    }
                    /*autoComplete: [
                                      {
                                          match: function (row, col, data) {
                                              if (col == 3) {
                                                  return true;
                                              }
                                              return false;
                                          },
                                          source: function () {
                                              var statusSource = new Array();
                                              for (var i = 0; i < docller.statusData.length; i++) {
                                                  var status = docller.statusData[i];
                                                  
                                                      statusSource[i] = status.statusText;
                                                  if (status.statusId > 0) {
                                                      $this.statusCache[status.statusText] = status.statusId;
                                                  }
                                              }
                                              return statusSource;
                                          },
                                          strict: true
                                      }
                    ]*/
            });
            $("#saveButton").click(jQuery.proxy(this.save, this));
            $("#saveAndTransmittButton").click(jQuery.proxy(this.transmit, this));
        }
    };
})();
$(document).ready(function () {
    //var grid = new docller.editFiles();
    var grid = new docller.editGrid();
    //docller.editFilesData.splice(0, 0, { "fileName": "File/Document #", "title": "Title", "revision": "Revision", "status": "Status" });
    grid.create();

});


