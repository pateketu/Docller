///<reference path="../Docller.js"/>
///<reference path="../Docller.js"/>
///<reference path="../jquery/jquery-1.6.2-vsdoc.js"/>
///<reference path="../plugins/jquery.validate-vsdoc.js"/>
///<reference path="../plugins/jquery.blockUI.js"/>
///<reference path="../bootstrap/bootstrap.js"/>
///<reference path="../plugins/plupload.full.js"/>
docller.fileRegister = {};
docller.fileRegister.utils = {};
docller.fileRegister.utils.formatAttachmentVersion = function(versionNum) {
    return "V" + versionNum;
};
docller.fileRegister.utils.setThView = function () {
    //var $container = $(".fileRegisterThumbnailView");
    //only fix up the heights if not in mobile view
    //otherwise it looks ugly in mobile view
    if (docller.utils.currentDevice() != docller.device.MOBILE) {
        var thViews = $(".thPreview");
        var tallest = 0;
        thViews.css("min-height", "");
        thViews.each(function() {
            var thisHeight = $(this).outerHeight(true);
            if (thisHeight > tallest) {
                tallest = thisHeight;
            }
        });
        thViews.css("min-height", tallest);
    }
};

docller.fileList = function (checkboxes) {
    this.checkboxes = checkboxes;
};

docller.fileList.prototype = (function () {
    return {
        getSelectedItems: function () {
            return this.checkboxes.filter(":checked");
        }
    };
})();

docller.newFolderAction = function () {
    this.saveButton = $("#btnSaveNewFolder");
    this.form = $("#createFolderForm");
    
};

docller.newFolderAction.prototype = (function () {
   
    return {
        init: function () {
            
            this.saveButton.click(function (e) {
                $("#dupfolderWarning").hide();
                var val = $("#folderName").val();
                if ($.trim(val).length == 0) {
                    $("#folderReqMsg").show();
                    e.preventDefault();
                }
            });
            $("#folderName").keypress(function() {
                $("#folderReqMsg").hide();
            });
            window.$newFolderAction = this;
        },
        refresh: function (itemList) {
            
        },
        onStart: function() {
            $("#newFolder").block({ message: '<img src="/images/busy.gif" /> Please wait, creating folder...' });
        },
        onComplete: function (data) {
            $("#newFolder").unblock();
            if (data.success == true) {
                window.location.href = data.url;
            } else {
                if (data.duplicatefolderMessage) {
                    var lbl = $("#dupfolderWarning");
                    lbl.text(data.duplicatefolderMessage);
                    lbl.show();
                }
            }
            
        },
        onError:function() {
            alert("error");
        }
    };

})();

docller.shareFilesAction = function () {
    this.selectedFiles = new Array();
    this.button = $("#nav-sharefiles");
};

docller.shareFilesAction.prototype = (function () {

    return {
        init: function () {
            var $this = this;
            $("#shareFilesModal").on("click", "#btnShareFiles", function(e) {
                $("#shareFilesErrors").html("");
                var val = $("#emails").val();
                if ($.trim(val).length == 0) {
                    $("#emailsReqMsg").show();
                    e.preventDefault();
                }

                var fileIds = "";
                $.each($this.selectedFiles, function (index, value) {
                    fileIds += $(value).data("fileid") + ",";
                });
                $("#fileIds").val(fileIds);
            });
            $("#shareFilesModal").on("keypress", "#emails", function(e) {
                $("#emailsReqMsg").hide();
            });
            window.$shareFilesAction = this;
        },
        refresh: function (itemList) {
            this.selectedFiles = itemList.getSelectedItems();
            this.button.toggleClass("hidden", this.selectedFiles.length == 0);
            
        },
        onStart: function () {
           $("#shareFilesModal").block({ message: '<img src="/images/busy.gif" /> Please wait...' });
            
        },
        onComplete: function (data) {
            $("#shareFilesModal").unblock();
            if (data.success == true) {
                $("#shareFilesContainer").html("<div class='alert alert-success'>Successfully shared files...</div>");
            } else {
                for (var i = 0; i < data.errors.length; i++) {
                    $("#shareFilesErrors").append("<div class='label label-important'>" + data.errors[i] + "</div><br/>");
                }
            }
            
        },
        onError: function () {
            $("#shareFilesModal").unblock();
            alert("error");
        }
    };

})();


docller.editAction = function () {
    this.selectedFiles = new Array();
    this.button = $("#nav-edit");
    this.fileEditOptions = $("#nav-fileEditOptions");
    this.editFilesForm = $("#editFilesForm");
};

docller.editAction.prototype = (function () {

    return {
        init: function () {
            var $this = this;
            this.button.click(function() {
                var uploadedFiles = "";
                $.each($this.selectedFiles, function (index, value) {
                    uploadedFiles += $(value).data("internalname") + "|";
                });
                $("#uploadedFiles").val(uploadedFiles);
                $this.editFilesForm.trigger("submit");

            });
        },
        refresh: function (itemList) {
            this.selectedFiles = itemList.getSelectedItems();
            this.button.toggleClass("hidden", this.selectedFiles.length == 0);
            this.fileEditOptions.toggleClass("hidden", this.selectedFiles.length == 0);
        }
    };
})();

docller.deleteAction = function () {
    this.selectedFiles = new Array();
    this.button = $("#nav-delete");
    this.fileDeleteButton = $(".fileDelete");
    this.refreshOnClose = false;
};



docller.deleteAction.prototype = (function () {
    function onDelete() {
        var fileIds = "";
        $.each(this.selectedFiles, function (index, value) {
            fileIds += "fileIds=" + $(value).data("fileid") + "&";
        });
        deleteFiles.call(this, fileIds,"Are you sure want to delete " + this.selectedFiles.length + " selected file(s)?");
    }
    
    function onSingleFileDelete(event) {
        var target = $(event.currentTarget);
        var fileId = target.data("fileid");
        deleteFiles.call(this, "fileIds=" + fileId, "Are you want to delete the file?");
    }
    
    function onDeleteVersion(event) {
        var target = $(event.currentTarget);
        var fileId = target.data("fileid");
        var revisionNumber = target.data("revisionnumber");
        
        if (confirm("Are you sure you want to delete the revision? All transmittals for the revision will also be deleted")) {
            var url = "/Project/DeleteFileVersion/" + docller.projectId;
            var fileVersionData = "fileId=" + fileId + "&revisionNumber=" + revisionNumber;
            var $that = this;
            $.post(url, fileVersionData)
                 .done(function (partialView) {
                     $("#historyModal").find(".modal-body").html(partialView);
                     $that.refreshOnClose = true;
                 })
                 .fail(function (xhr, textStatus, errorThrown) {
                     alert(errorThrown);
                 });
        }
    }

    function onDeleteAttachmentVersion(event) {
        var target = $(event.currentTarget);
        var fileId = target.data("fileid");
        var rev = target.data("revisionnumber");
        var fileData = "fileId=" + fileId + "&revisionNumber=" + rev;
        deleteAttachment.call(this, fileData, "Are you sure you want to delete the attachment revision?");
    }
    
    function onDeleteAttachment(event) {
        var target = $(event.currentTarget);
        var fileId = target.data("fileid");
        var fileData = "fileId=" + fileId;
        deleteAttachment.call(this, fileData, "Are you sure you want to delete the attachment and all its previous revisions?");
    }
    
    function deleteAttachment(fileAttachmentData, msg) {
        if (confirm(msg)) {
            var url = "/Project/DeleteAttachment/" + docller.projectId;
            var $that = this;
            $.post(url, fileAttachmentData)
                 .done(function(viewModel) {
                     $(document).trigger("docller.fileRegisterActions.onAttachmentUpdated", [viewModel]);
                     
                     if (viewModel.fileName == null || viewModel.fileName.length == 0) {
                         
                         //this means that we have deleted the attachment and we need to refresh the file register screen
                         //on close of modal
                         $that.refreshOnClose = true;
                     } else {
                         $that.refreshOnClose = false;
                     }
                 })
                 .fail(function (xhr, textStatus, errorThrown) {
                     alert(errorThrown);
                 });
        }
    }
    function deleteFiles(fileIds, msg) {
        if (confirm(msg + " All versions and transmittals will also be deleted")) {
            var url = "/Project/DeleteFiles/" + docller.projectId;
            $.blockUI({ message: '<img src="/images/busy.gif" /> Please wait, deleting files...' });
            $.post(url, fileIds)
                 .done(function () {
                     $.unblockUI();
                     window.location.replace(window.location.href.replace("#", ""));
                 })
                 .fail(function (xhr, textStatus, errorThrown) {
                     $.unblockUI();
                     alert(errorThrown);
                 });
        }
    }
    return {
        init: function() {
            this.button.click(jQuery.proxy(onDelete, this));
            this.fileDeleteButton.click(jQuery.proxy(onSingleFileDelete, this));
            var $that = this;
            $(document).on("click", ".deleteVersion",
                jQuery.proxy(onDeleteVersion, $that));
            $(document).on("click", ".deleteAttachment",
                jQuery.proxy(onDeleteAttachment, $that));
            $(document).on("click", ".deleteAttachmentVersion",
                jQuery.proxy(onDeleteAttachmentVersion, $that));

            $("#attachmentModal").on("hide", function () {
                if ($that.refreshOnClose) {
                    window.location.replace(window.location.href.replace("#",""));
                }
            });
            
            $("#historyModal").on("hide", function () {
                if ($that.refreshOnClose) {
                    window.location.replace(window.location.href.replace("#", ""));
                }
            });
        },
        refresh: function(itemList) {
            this.selectedFiles = itemList.getSelectedItems();
            this.button.toggleClass("hidden", this.selectedFiles.length == 0);
        }
    };

})();

docller.downloadAction = function() {
    this.selectedFiles = new Array();
    this.button = $("#nav-download");
    this.fileDownloadButtons = $(".fileDownload");
};

docller.downloadAction.prototype = (function () {

    function onDownload() {
       
       var fileIds = "";
        $.each(this.selectedFiles, function(index, value) {
            fileIds += "fileIds=" + $(value).data("fileid") + "&";
        });
        download.call(this, fileIds);
    }
    
    function onFileDownload(event) {
        
        var target = $(event.currentTarget);
        var fileId = target.data("fileid");
        download.call(this, "fileIds=" + fileId);
    }
    
    function download(fileIds) {
        $.fileDownloader('/Download/DownloadFiles', fileIds);
    }
    function downloadVersion(event) {
        var target = $(event.currentTarget);
        var data = "fileId=" + target.data("fileid") + "&version=" + target.data("revisionnumber");
        $.fileDownloader('/Download/DownloadVersion', data);
    }

    return {
        init: function () {
            this.button.click(jQuery.proxy(onDownload, this));
            this.fileDownloadButtons.click(jQuery.proxy(onFileDownload, this));
            var $that = this;
            $(document).on("click", ".downloadVersion", 
                jQuery.proxy(downloadVersion, $that));
        },
        refresh:function (itemList) {
            this.selectedFiles = itemList.getSelectedItems();
            this.button.toggleClass("hidden", this.selectedFiles.length == 0);
        }
    };
    
})();


docller.fileActionsFactory = function(list) {
    this.allActions = new Array();
    this.fileList = list;
};

docller.fileActionsFactory.prototype = (function () {
    return {
        init: function() {

            this.allActions[0] = new docller.downloadAction();
            this.allActions[1] = new docller.newFolderAction();
            this.allActions[2] = new docller.deleteAction();
            this.allActions[3] = new docller.shareFilesAction();
            this.allActions[4] = new docller.editAction();

            for (var i = 0; i < this.allActions.length; i++) {
                if (this.allActions[i].init) {
                    this.allActions[i].init();
                }
                
            }

        },
        refreshAll: function() {
            for (var i = 0; i < this.allActions.length; i++) {
                if (this.allActions[i].refresh) {

                    this.allActions[i].refresh(this.fileList);
                }
            }
        }
    };
})();

/*
docller.fileHistoryModel = {};

docller.fileHistory = function() {
    this.fileHistoryModel = null;
};
docller.fileHistory.prototype = (function () {
    return {
      bindModel: function(data) {
          docller.fileHistoryModel = ko.mapping.fromJS(data);
      }  
    };

})();
*/

docller.fileAttachmentModel = {};

docller.fileAttachments = function () {
    this.attachmentViewModel = null;
    //this.uploader = null;
    //this.refreshOnClose = false;
};
docller.fileAttachments.prototype = (function () {
    
    return {
        bindModel: function (data) {
            docller.fileAttachmentModel = ko.mapping.fromJS(data);
            ko.applyBindings(docller.fileAttachmentModel, document.getElementById("attachmentContainer"));
            $(document).off("docller.fileRegisterActions.onAttachmentUpdated");
            $(document).on("docller.fileRegisterActions.onAttachmentUpdated", function (evt, viewModel) {
                ko.mapping.fromJS(viewModel, {}, docller.fileAttachmentModel);
                /*$("#attachmentFileList").html("");
                $("#btnSelectAttachment").removeClass("disabled");
                $("#btnUploadAttachment").addClass("disabled");*/
            });
            /*
            if (this.uploader != null) {
                this.uploader.destroy();
                this.uploader = null;
            }
            var $this = this;
            this.uploader = new plupload.Uploader({
                runtimes: 'html5,silverlight,flash,html4',
                browse_button: 'btnSelectAttachment',
                container: 'attachmentUpload',
                dragdrop: false,
                url: '/Project/UploadAttachment/' + docller.projectId + "/" + docller.folderId,
                filters: [
                                       { title: "PDF & CAD Files", extensions: "pdf,dwg" },
                                       { title: "Zip files", extensions: "zip" },
                                       { title: "Image files", extensions: "jpg,gif,png,bmp" },
                                       { title: "All files", extensions: "*" }
                ],
                max_file_size: '2000mb',
                chunk_size: '1mb',
                multi_selection:false,
                // Flash settings
                flash_swf_url: '/Client/plupload.flash.swf',

                // Silverlight settings
                silverlight_xap_url: '/Client/plupload.silverlight.xap'
            });
            
            $('#btnUploadAttachment').click(function (e) {
                $this.uploader.start();
                e.preventDefault();
            });

            this.uploader.init();
            
            this.uploader.bind('FilesAdded', function (up, files) {

                
                $('#attachmentFileList').append(
                        '<div id="' + files[0].id + '">' +
                        files[0].name + ' (' + plupload.formatSize(files[0].size) + ') <strong></strong>' +
                    '</div>'); 

                $("#btnSelectAttachment").addClass('disabled');
                $("#btnUploadAttachment").removeClass("disabled");
                
                up.refresh(); // Reposition Flash/Silverlight
            });

            this.uploader.bind('UploadProgress', function (up, file) {
                $('#' + file.id + " strong").html(" --- " + file.percent + "%");

            });
            
            this.uploader.bind('Error', function (up, err) {
                $('#attachmentFileList').html("<div class='label label-important dc-whitecolor field-validation-error'>Error: " + err.code +
                    ", Message: " + err.message +
                    (err.file ? ", File: " + err.file.name : "") +
                    "</div>"
                );

                up.refresh(); // Reposition Flash/Silverlight
            });
            
            this.uploader.bind('FileUploaded', function (up, file, object) {
                $('#' + file.id + " strong").html(" --- 100%");
                if (object.response.length > 0) {
                    var viewModel;
                    try {
                        viewModel = eval(object.response); 
                    } catch(e) {
                        viewModel = eval("(" + object.response + ")");
                    } 
                    
                    $(document).trigger("docller.fileRegisterActions.onAttachmentUpdated", [viewModel]);
                }
               
                $this.refreshOnClose = true;
            });
            this.uploader.bind('BeforeUpload', function (up, file) {
                up.settings.multipart_params = { parentFile: $("#btnUploadAttachment").data("fileid"), size:file.size};
            });
            

            $("#attachmentModal").on("hide", function () {
                if ($this.refreshOnClose) {
                    window.location.replace(window.location.href.replace("#", ""));
                }
            })
            */;
        }
        
    };
})();

docller.singleFileUploader = function() {
    
};

docller.singleFileUploader.prototype = (function () {

    return {
        setUp: function (buttonId, actionUrl, onFileAdded, onBeforeUpload) {
            var $this = this;
            var uploader = new plupload.Uploader({
                    runtimes: 'html5,silverlight,flash,html4',
                    browse_button: buttonId,
                    container: 'fileVersionList',
                    dragdrop: false,
                    url: actionUrl,
                    filters: [
                                           { title: "PDF & CAD Files", extensions: "pdf,dwg" },
                                           { title: "Zip files", extensions: "zip" },
                                           { title: "Image files", extensions: "jpg,gif,png,bmp" },
                                           { title: "All files", extensions: "*" }
                    ],
                    max_file_size: '2000mb',
                    chunk_size: '1mb',
                    multi_selection:false,
                    // Flash settings
                    flash_swf_url: '/Client/plupload.flash.swf',

                    // Silverlight settings
                    silverlight_xap_url: '/Client/plupload.silverlight.xap'
                });
            
            uploader.init();
            if (onFileAdded) {
                uploader.bind('FilesAdded', onFileAdded);
            } else {
                uploader.bind('FilesAdded', function(up, files) {

                    $.blockUI({ message: '<img src="/images/busy.gif" /> Uploading ' + files[0].name });
                    uploader.start();


                    up.refresh(); // Reposition Flash/Silverlight
                });
            }
            uploader.bind('FileUploaded', function (up, file, object) {
                $.unblockUI();
                window.location.replace(window.location.href.replace("#", ""));
                
            });
            uploader.bind('UploadProgress', function (up, file) {
                $(".blockMsg").html("Uploading " + file.name + " --- " + file.percent + "%");

            });
            if (onBeforeUpload) {
                uploader.bind('BeforeUpload', onBeforeUpload);
            } else {
                uploader.bind('BeforeUpload', function(up, file) {
                    up.settings.multipart_params = { fileId: $("#" + buttonId).data("fileid"), fileSize: file.size };
                });
            }
            uploader.bind('Error', function (up, err) {
                $('.blockMsg').html("<div><div  class='label label-important dc-whitecolor field-validation-error'>Error: " + err.code +
                    ", Message: " + err.message +
                    (err.file ? ", File: " + err.file.name : "") +
                    "</div><br/><a class='btn btn-info' href='" + window.location.href.replace("#", "") + "'>Go Back</a></div>"
                );

                up.refresh(); // Reposition Flash/Silverlight
            });
        }

    };
})();

$(document).ready(function () {
    //console.log(docller.utils.currentDevice());
    var allcheckBoxes = $(".chkFile");

    var fileList = new docller.fileList(allcheckBoxes);
    var allActions = new docller.fileActionsFactory(fileList);
    allActions.init();

    $("#chkAllFiles").change(function () {
        var $this = $(this);
        if ($this.is(':checked')) {
            allcheckBoxes.attr("checked", "checked");
        } else {
            allcheckBoxes.removeAttr("checked");
        }
        allActions.refreshAll();
    });

    $(".badge-version").tooltip();
    $(".thViewFileName").popover({trigger:"hover"});
    $(".filetype-icon-small").popover();
    allcheckBoxes.change(function () {
        allActions.refreshAll();
    });
    var previews = new docller.filePreviews();
    previews.init();

    $(".uploadVersion").each(function () {

        var uploader = new docller.singleFileUploader();
        uploader.setUp($(this).prop("id"));
    });


    $(".uploadComment").each(function () {
        var uploader = new docller.singleFileUploader();
        var $file = $(this);

        uploader.setUp($file.prop("id"), "/Project/UploadComment/" + docller.projectId + "/" + docller.folderId,
            function(up, files) {
                var commentModal = $("#attachCommentModal");
                var uploadButton = commentModal.find(".btn-primary");
                uploadButton.on("click", function(evt) {

                    $("#attachCommentModal").modal("hide");
                    evt.preventDefault();

                    $.blockUI({ message: '<img src="/images/busy.gif" /> Uploading ' + files[0].name });
                    up.start();
                    up.refresh(); // Reposition Flash/Silverlight

                });
                $.ajax({
                    type: "POST",
                    url: "/Project/GetCommentMetaInfo/" + docller.projectId + "/" + docller.folderId,
                    data: "fileId=" + $file.data("fileid") + "&name=" + files[0].name,
                    success: function(result) {
                        if (result.success) {
                            commentModal.find(".alert").hide();
                            commentModal.find(".well").html("Selected file: " + files[0].name);
                            commentModal.find(".well").show();
                            commentModal.find("textarea").show();
                            uploadButton.show();
                            commentModal.modal();
                        } else if (result.existingMarkUpFileByAnotherUser) {
                            commentModal.find(".alert").html(files[0].name + " is an existing markup file uploaded by other user, please rename the file and try again");
                            commentModal.find(".alert").show();
                            commentModal.find(".well").hide();
                            commentModal.find("textarea").hide();
                            uploadButton.hide();
                            commentModal.modal();
                        } else {
                            alert("Errors, please try again later");
                        }
                        
                        //alert(result.success);
                    },
                    error: function() {
                        alert("Errors occured, Please try again later");
                    }
                });

            }, function (up, file) {
                var commentModal = $("#attachCommentModal");
                
                up.settings.multipart_params = {
                    fileId: $file.data("fileid"),
                    fileSize: file.size,
                    comments: commentModal.find("textarea").val()
                };
            });
    });

    $(document).on("click", '[data-toggle="directdownload"]', function() {
        var $this = $(this);
        $.fileDownloader('/Download/DownloadFiles', "fileIds=" + $this.data("fileid"));
    });

    var renameFolderModal = $("#renameFolder");
    renameFolderModal.on("shown", function () {
        
        $("#txtfolderRename").val("");
        $("#renameFolderError").hide();
        $("#folderRenameReqMsg").hide();

        $("#txtfolderRename").keypress(function () {
            $("#folderRenameReqMsg").hide();
        });
    });
    $("#btnRenameFolder").click(function(e) {
        var val = $("#txtfolderRename").val();
        if ($.trim(val).length == 0) {
            $("#folderRenameReqMsg").show();
            e.preventDefault();
        } else {
            renameFolderModal.block({ message: '<img src="/images/busy.gif" /> Please wait...' });
            $.ajax({
                type: "POST",
                url: "/Project/RenameFolder/" + docller.projectId + "/" + docller.folderId,
                data: "folderName=" + val,
                success: function (result) {
                    renameFolderModal.unblock();
                    if (result.success) {
                        window.location.replace(window.location.href.replace("#", ""));
                    }else if (result.existingFolder) {
                        var lbl = $("#renameFolderError");
                        lbl.text(val + " is an exisiting folder, Please choose a different name");
                        lbl.show();
                    }
                },
                error: function () {
                    renameFolderModal.unblock();
                    alert("Errors occured in renaming folder, Please try again later");
                },
                dataType: "json"
            });
        }

    });

    var minWidth = 0;
    
    $("#folderArea").resizable({
        autoHide: false,
        handles: { 'e': '#resizeHandle' },
        minWidth: minWidth,
        maxWidth:600,
        resize: function (e, ui) {

            var parentWidth = ui.element.parent().width();
            var remainingSpace = parentWidth - ui.element.outerWidth();

            if (remainingSpace < minWidth) {
                ui.element.width((parentWidth - minWidth) / parentWidth * 100 + "%");
                remainingSpace = minWidth;
            }

            var divTwo = ui.element.next(),
                divTwoWidth = (remainingSpace - (divTwo.outerWidth() - divTwo.width())) / parentWidth * 100 + "%";
            divTwo.width(divTwoWidth);
        },
        stop: function (e, ui) {
            var parentWidth = ui.element.parent().width();
            ui.element.css({
                width: ui.element.width() / parentWidth * 100 + "%"
            });
        }
    });
    $("#btn-folderView").on("click", function() {
        
        var folderArea = $("#folderArea");
        folderArea.toggleClass("out");
        if (folderArea.hasClass("hidden-phone hidden-tablet")) {
            folderArea.removeClass("hidden-phone hidden-tablet", 400);
            folderArea.collapse({ toggle: false });
        } else {
            if (folderArea.hasClass("in")) {
                folderArea.collapse("hide");
                //folderArea.data("isCollapsed", true);
            } else {
                folderArea.collapse("show");
            }
        }
        $("#fileContainer").toggleClass("hidden");
    });
    $(window).on('resize', function (e) {

        if (e.target == window) {
            //only do if the actual window is resized 
            //reisze fires even when splitter is resized
            docller.fileRegister.utils.setThView();
            $(".zoomablepreview").remove(); //Remove any zoomanble preview row
            $("#previewArea").removeClass("stick").empty();
            if (docller.utils.currentDevice() == docller.device.DESKTOP) {
                var folderArea = $("#folderArea");
                folderArea.removeClass("out");
                folderArea.removeAttr("style");

                $("#fileContainer").removeClass("hidden");
                if (!folderArea.hasClass("in")) {
                    folderArea.addClass("in");
                }
                if (!folderArea.hasClass("hidden-phone hidden-tablet")) {
                    folderArea.addClass("hidden-phone hidden-tablet");
                }
            }
        }
    });


});
//Doing in window load as resizing the divs with images does not give
//correct results until all images are loaded
$(window).load(function () {
    docller.fileRegister.utils.setThView();

});
