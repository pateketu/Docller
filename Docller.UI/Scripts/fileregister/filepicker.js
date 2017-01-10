///<reference path="../Docller.js"/>
///<reference path="../Docller.js"/>
///<reference path="../jquery/jquery-1.6.2-vsdoc.js"/>
///<reference path="../plugins/jquery.validate-vsdoc.js"/>
///<reference path="../plugins/jquery.blockUI.js"/>
///<reference path="../bootstrap/bootstrap.js"/>
///<reference path="../plugins/knockout-2.1.0.js"/>
docller.filePicker = function (target, url) {
    this.$modal = $(target);
    this.pickerUrl = url;
    this.files = new Object();
    var inititaldata = { files: [] };
    this.$modal.trigger("docller.filePicker.onload", [inititaldata]);
    this.hasFiles = false;
    if (inititaldata.files.length > 0) {
        this.hasFiles = true;
        for (var i = 0; i < inititaldata.files.length; i++) {
            var file = inititaldata.files[i];
            this.files[file.fileId] = file;
        }
    }
};

docller.filePicker.prototype = (function () {

    return {
        show: function () {

            var $this = this;
            
            this.$modal.on("change", "#chkAllFiles", function () {
                var allcheckBoxes = $this.$modal.find("tbody").find("input:checkbox");
                
                var $chkBox = $(this);
                
                if ($chkBox.is(':checked')) {
                    allcheckBoxes.attr("checked", "checked");
                } else {
                    allcheckBoxes.removeAttr("checked");
                }
                allcheckBoxes.trigger("change");
            });

            this.$modal.on("change", "tbody input:checkbox", function () {
                var $thisCheckbox = $(this);
                var id = $thisCheckbox.data("fileid");

                var file = {
                    fileId: id,
                    fileName: $thisCheckbox.data("filename"),
                    title: $thisCheckbox.data("filetitle"),
                    revision: $thisCheckbox.data("filerev")
                };

                
                if ($thisCheckbox.is(':checked')) {
                    $this.addFile(file);
                    $thisCheckbox.data("file", file);
                    $thisCheckbox.parent().parent().addClass("alert alert-success");
                } else {
                    $this.removeFile($thisCheckbox.data("file"));
                    $thisCheckbox.parent().parent().removeClass("alert alert-success");
                }
            });

            if (this.hasFiles) {
                this.$modal.find("#selectfiles").text("Update files...");
            }
            
            this.$modal.find("#selectfiles").on("click", function (e) {
                var files = new Array();
                
                for (f in $this.files) {
                    files[files.length] = $this.files[f];
                }
                $this.$modal.trigger("docller.filePicker.filesSelected", [files]);
                $this.$modal.modal('hide');
                
            });


            this.$modal.on("click", "#fileArea a", function (e) {
                e.preventDefault();
                var $a = $(e.target);
                var href = $a.attr("href");
                if (href.length > 1) {
                    $this.refreshFiles(href);
                }
            });

            this.$modal.modal({ remote: this.pickerUrl });


        },
        hide:function() {
            this.$modal.off("click", "#fileArea a");
            this.$modal.find("#selectfiles").off("click");
            this.$modal.off("change", "tbody input:checkbox");
        },
        addFile: function (file) {
            if (this.files[file.fileId] == null) {
                this.files[file.fileId] = file;
            }
            
        },
        removeFile: function (file) {
            this.files[file.fileId] = null;
            delete this.files[file.fileId];
        },
        refreshFiles: function (href) {
            var $this = this;
              this.$modal.block({ message: '<img src="/images/busy.gif" /> Please wait, getting files...' });
                $.get(href, function (data) {
               
                    $this.$modal.unblock();
                    var fileArea = $("#fileArea");
                    fileArea.html(data);
                    var allCheckbox = fileArea.find("tbody").find("input:checkbox");
                    for (var i = 0; i < allCheckbox.length; i++) {
                        var chk = $(allCheckbox[i]);
                        if ($this.files[chk.data("fileid")] != null) {
                            chk.prop("checked", true);
                            chk.trigger("change");
                        }
                    }

                    window.location.href = "#filepickertop";
                
            }).fail(function () {
                alert("Errors occured");
                $this.$modal.unblock();
            });
        }
    };
})();


$(document).on('click', '[data-toggle="filePicker"]', function (e) {
    var $this = $(this);
    var target = $this.data("target");

    var picker = new docller.filePicker(target, $this.attr("href"));
    e.preventDefault();
    picker.show();
    $(target).on("hidden", function (args) {
        $(document).off("docller.filePicker.folderSelected");
        picker.hide();
    });
    $(document).on("docller.filePicker.folderSelected", function (event, url) {
        picker.refreshFiles(url);
    });
    

    
});


