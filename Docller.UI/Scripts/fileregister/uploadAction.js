/// <reference path="jquery-1.6.2-vsdoc.js" />
/// <reference path="jquery.validate.js" />
/// <reference path="jquery-ui-1.8.11.js" />
/// <reference path="jquery.unobtrusive-ajax.js" />

//START Knockout support for fileupload
ko.bindingHandlers.fileStatusId = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var value = valueAccessor();
        var id = value();
        $(element).attr("id", id + "__fs");
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        // This will be called once when the binding is first applied to an element,
        // and again whenever the associated observable changes value.
        // Update the DOM element based on the supplied values here.
    }
};

ko.bindingHandlers.messageId = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var value = valueAccessor();
        var id = value();
        $(element).attr("id", id + "__msg");
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        // This will be called once when the binding is first applied to an element,
        // and again whenever the associated observable changes value.
        // Update the DOM element based on the supplied values here.
    }
};

docller.filesViewModel = function (files, fileUploader) {
    var $this = this;
    $this.uploader = fileUploader;
    $this.files = ko.mapping.fromJS(files, {
        'fileSize': {
            create: function (options) {
                return plupload.formatSize(options.data);
            }

        }

    });
    $this.removeFile = function (file) {
        var f = ko.mapping.toJS(file);
        $this.uploader.removeFile(f);

    };
};
//END Knockout support for fileupload
//Start of uploader
docller.fileUploader = function () {
    this.uploader = null;
    this.currentFileList = new Object();
    this.files = null;
    this.totalFiles = 0;
    this.skippedFiles = 0;
    this.uploadedFiles = new Array();
    this.uploadedAttachments = new Array();
}
docller.fileUploader.prototype = (function () {


    var Const = {
        Container: "#fileuploader",
        ContainerId: "fileuploader",
        FileList: "#fileuploader_filelist",
        FileListId: "fileuploader_filelist",
        DropArea: "#fileuploader_filelist",
        DropAreaId: "fileuploader_filelist",
        FileUploadOptions: "#fileuploader_options",
        AttachCADFiles: "#attachCADfiles",
        UploadAsNewVersion: "#uploadnewversion",
        BrowseButton: "#fileuploader_browse",
        BrowseButtonId: "fileuploader_browse",
        StartUploadButton: "#fileuploader_start",
        Cache: "plupload",
        DropAreaCss: "fileuploader_droparea",
        HasFiles: "HasFiles",
        StartButtonDisabledCssClass: "plupload_disabled",
        FailedCssClass: "fileupload_failed",
        DeleteCssClass: "plupload_delete",
        AlertArea: "#alertArea",
        VersionAlert: "#versionAlert",
        UploadForm: "#upload-form",
        Height: 351,
        TallHeight: 400,
        UploadingCssClass: "plupload_uploading",
        DoneCssClass: "fileupload_done",
        TotalNum: "#filelist_totalnum",
        FooterStatus: "#filelist_footer_status",
        TotalPercent: "#filelist_totalpercent",
        TotalSize: "#filelist_totalsize",
        ProgressBar: "#filelist_progressbar",
        EditFilesForm: "#editFilesForm",
        UploadedFilesHidden: "#uploadedFiles",
        UploadedAttachmentsHidden: "#uploadedAttachments"
    };


    function clearFileList() {
        var filelist = $(Const.FileList).html("");
    }
    function init(up, params) {
        resetFooterStatus.call(this);
        $(Const.BrowseButton).show();
        $(Const.StartUploadButton).show();
        var attachment = $(Const.AttachCADFiles);
        var uploadAsNewVersion = $(Const.UploadAsNewVersion);
        attachment.removeAttr("disabled");
        attachment.unbind("change").change(jQuery.proxy(refreshFileList, this));
        uploadAsNewVersion.removeAttr("disabled", '');
        uploadAsNewVersion.unbind("change").change(jQuery.proxy(ensureVersion, this));
        $(Const.FileUploadOptions).show();

        if (window.FileReader && !((params.runtime == "flash") || (params.runtime == "silverlight"))) {
            var dropArea = $(Const.DropArea);
            dropArea.append("<li class=\"plupload_droptext\">Drag files here</li>");
        }
    }
    function clearAlerts() {
        $(Const.VersionAlert).alert("close");
        var uploadForm = $(Const.UploadForm);
        uploadForm.height(Const.Height);
        $(Const.FileList).find(".file_msg").html("");

    }
    function ensureVersion() {
        var isChecked = $(Const.UploadAsNewVersion).is(":checked");
        if (isChecked || this.uploader.files.length <= 0) {
            clearAlerts.call(this);
        }

        //see now if we need to enable the button
        if (this.uploader.files.length <= 0) {
            disableStartButton.call(this);
            return;
        } else if (this.uploader.files.length > 0 && isChecked) {
            enableStartButton.call(this);
            return;
        }


        //Go through each file in queue and check it's against the server cache
        var pFiles = this.uploader.files;
        var numOfExsitingFiles = 0;
        for (var i = 0; i < pFiles.length; i++) {
            var file = getActualFile.call(this, pFiles[i]);
            if (file.isExistingFile) {
                numOfExsitingFiles++;
                //Insert the message 
                var id = pFiles[i].id;
                $("#" + id + "__msg").html("<span class=\"label label-important\">Existing file</span>");
            }
        }

        if (numOfExsitingFiles > 0) {
            $(Const.UploadForm).height(Const.TallHeight);
            var file = "file";
            if (numOfExsitingFiles > 1) {
                file += "s";
            }
            var alertMsg = numOfExsitingFiles + " existing " + file + " found. Upload " + file + " as a new version or remove the duplicate files";
            var alertHtml = "<div class=\"alert alert-error\" id=\"versionAlert\">" + alertMsg + "</div>";
            $(Const.AlertArea).html(alertHtml);
            disableStartButton.call(this);
        } else {
            clearAlerts.call(this);
            enableStartButton.call(this);
        }

    }

    function refreshFileList() {

        if (!this.uploader.files || this.uploader.files.length <= 0)
            return;
        var $this = this;
        var data = {
            projectId: docller.projectId,
            folderId: docller.folderId,
            attachCADFiles: $(Const.AttachCADFiles).is(":checked"),
            uploadAsNewVersion: $(Const.UploadAsNewVersion).is(":checked"),
            files: this.uploader.files
        };
        $.ajax({
            url: "/Project/GetMetaDataInfo/",
            type: "POST",
            cache: false,
            data: JSON.stringify(data),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            error: function (request, textStatus, errorThrown) {
                alert(errorThrown);
            },
            success: function (files, textStatus, request) {
                clearFileList.call($this);
                $this.files = files;
                $this.totalFiles = $this.uploader.files.length;
                reorderFiles.call($this);
                var model = new docller.filesViewModel(files, $this);
                ko.applyBindings(model, document.getElementById(Const.FileListId));
                ensureVersion.call($this);
            }
        });

    }

    function reorderFiles() {
        var index = 0;
        for (var i = 0; i < this.files.length; i++) {
            var file = this.files[i];
            this.uploader.files[index] = { id: file.uniqueIdentifier, name: file.fileName, status: 0, percent: 0, size: file.fileSize, status: plupload.QUEUED };
            index++;
            if (file.attachments && file.attachments.length > 0) {
                for (var j = 0; j < file.attachments.length; j++) {
                    var attachment = file.attachments[j];
                    this.uploader.files[index] = { id: attachment.uniqueIdentifier, name: attachment.fileName, status: 0, percent: 0, size: attachment.fileSize, status: plupload.QUEUED };
                    index++;
                }
            }
        }
    }
    function filesAdded(up, files) {
        var $this = this;
        //Remove Duplicates
        $.each(files, function (i, file) {
            var key = file.name.toLowerCase();
            if ($this.currentFileList[key] != null) {
                up.removeFile(file);
                return;
            } else {
                $this.currentFileList[key] = file;
            }

        });
        //enable start upload button
        refreshFileList.call(this);

        up.refresh(); // Reposition Flash/Silverlight

    }

    function startUpload() {
        $("#versionAlert").alert("close");
        $(Const.AttachCADFiles).attr("disabled", '');
        $(Const.AttachCADFiles).unbind("change");
        $(Const.UploadAsNewVersion).attr("disabled", '');
        $(Const.UploadAsNewVersion).unbind("change");
        $(Const.BrowseButton).hide();
        disableStartButton.call(this);
        $(Const.StartUploadButton).hide();
        uploadFooterStatus.call(this);
        this.uploader.start();
    }

    function resetFooterStatus() {
        clearAlerts.call(this);
        $(Const.FooterStatus).hide();
        $(Const.TotalNum).hide();
    }

    function uploadFooterStatus(forceShow) {
        var footer = $(Const.FooterStatus);
        var totlaNum = $(Const.TotalNum);
        if (!forceShow) {
            if (!footer.is(":visible")) {
                return;
            }
        }

        footer.show();
        totlaNum.show();
        var queued = this.uploader.total;
        $(Const.TotalNum).html("Uploaded " + queued.uploaded + "/" + this.totalFiles + " files");
        var percent = queued.percent;

        if (isNaN(percent)) {
            //if for some reson we get NaN we fall
            //back on manual percent calculation
            percent = Math.floor((queued.uploaded / this.totalFiles) * 100);

        }
        $(Const.TotalPercent).html(percent + "%");
        $(Const.ProgressBar).width(percent + "%");

        $(Const.TotalSize).html(plupload.formatSize(queued.size));

        if (queued.failed > 0) {
            $(Const.UploadForm).height(Const.TallHeight);
            var file = "file";
            if (queued.failed > 1) {
                file += "s";
            }
            var alertMsg = "Failed to upload " + queued.failed + " " + file;
            //files will only be skipped is case of an error of parent file
            if (this.skippedFiles > 0) {
                file = "file";
                if (this.skippedFiles > 1) {
                    file += "s";
                }
                alertMsg += " and skipped " + this.skippedFiles + " " + file;
            }
            var alertHtml = "<div class=\"alert alert-error\" id=\"versionAlert\">" + alertMsg + "</div>";
            $(Const.AlertArea).html(alertHtml);
        }
    }

    function getActualFile(pFile) {
        for (var i = 0; i < this.files.length; i++) {
            var file = this.files[i];
            if (pFile.id == file.uniqueIdentifier) {
                return file;
            }

            if (file.attachments && file.attachments.length > 0) {
                for (var j = 0; j < file.attachments.length; j++) {
                    var attachment = file.attachments[j];
                    if (pFile.id == attachment.uniqueIdentifier) {
                        return attachment;
                    }
                }
            }
        }
    }
    function uploadProgress(up, file) {
        if (file.percent < 100) {
            $('#' + file.id + "__fs").html(file.percent + "%");
        }
        uploadFooterStatus.call(this, true);

    }

    function onError(up, err) {
        if (err.file) {
            $('#' + err.file.id + "__fs").html("0%");
            $('#' + err.file.id).removeClass(Const.DeleteCssClass)//;.addClass(Const.FailedCssClass);
            var file = getActualFile.call(this, err.file);

            if (file.parentFile) {
                $("#" + file.parentFile).html("<a href=\"#\" style=\"display: block;\"  title=\"" + err.message + "\" class=\"" + Const.FailedCssClass + "\"></a>");
            } else {
                $('#' + err.file.id).find("a:first").attr("title", err.message).addClass(Const.FailedCssClass);

                //see if need to remove the attachment;
                if (file.attachments && file.attachments.length > 0) {

                    //figure out the index of current file
                    var index = -1;
                    for (var i = 0; i < this.uploader.files.length; i++) {
                        if (this.uploader.files[i].id == err.file.id) {
                            index = i;
                            break;
                        }
                    }

                    if (index > -1) {
                        //next file will be the attachment
                        var nextIndex = index + 1;
                        if (this.uploader.files.length > nextIndex) {
                            //right now we are only counting on 1 attachment file

                            if (this.uploader.files[nextIndex].id == file.attachments[0].uniqueIdentifier) {
                                //We need to remove the attachment from the queue to be uploaded
                                var attachmentToRemove = this.uploader.files[nextIndex];
                                this.uploader.removeFile(attachmentToRemove);
                                this.skippedFiles++;
                                //Insert the warning message the file was skipped
                                $("#" + attachmentToRemove.id + "__msg").html("<span class=\"label label-inverse\">Attachment upload skipped</span>");
                            }
                        }

                    }

                }


            }




        } else {
            alert(err.message);
        }
        uploadFooterStatus.call(this);
        up.refresh(); // Reposition Flash/Silverlight
    }

    function fileUploaded(up, pFile) {
        $('#' + pFile.id + "__fs").html("100%");
        $('#' + pFile.id).removeClass(Const.UploadingCssClass); //.addClass(Const.DoneCssClass);
        var file = getActualFile.call(this, pFile);

        if (file.parentFile) {
            $("#" + file.parentFile).html("<a href=\"#\" style=\"display: block;\" class=\"" + Const.DoneCssClass + "\"></a>");
            this.uploadedAttachments[this.uploadedAttachments.length] = file;
        } else {
            $('#' + pFile.id).find("a:first").addClass(Const.DoneCssClass);
            this.uploadedFiles[this.uploadedFiles.length] = file;
        }
        uploadFooterStatus.call(this);

    }

    function beforeUpload(up, pFile) {
        $('#' + pFile.id).find("a").removeAttr("title").unbind("click");
        $('#' + pFile.id).removeClass(Const.DeleteCssClass).addClass(Const.UploadingCssClass);
        var file = getActualFile.call(this, pFile);
        var parentFile = "";
        if (file.parentFile) {
            parentFile = file.parentFile;
        }

        var param = {
            fileName: pFile.name,
            fileId: file.fileId,
            fileInternalName: file.fileInternalName,
            parentFile: parentFile,
            projectId: file.project.projectId,
            folderId: file.folder.folderId,
            fullPath: file.folder.fullPath,
            baseFileName: file.baseFileName,
            docNumber: file.docNumber,
            fileSize: pFile.size
        };
        
        if (file.isExistingFile == true) {
            param.isExisting = "1";
        }

        if ($(Const.UploadAsNewVersion).is(":checked")) {
            param.uploadAsNewVersion = "1";
        }

        up.settings.multipart_params = param;

    }

    function uploadComplete(up, files) {
        var queued = this.uploader.total;
        if (queued.failed > 0) {
            $(Const.FooterStatus).hide();
        }
        //alert(this.uploadedAttachments.length);
        //alert(this.uploadedFiles.length);
        var uploadedFilesInternalName = "";
        for (var i = 0; i < this.uploadedFiles.length; i++) {
            uploadedFilesInternalName = uploadedFilesInternalName + this.uploadedFiles[i].fileInternalName + "|";
        }

        $(Const.UploadedFilesHidden).val(uploadedFilesInternalName);
        $(Const.EditFilesForm).trigger("submit");
    }

    function enableStartButton() {
        var button = $(Const.StartUploadButton);
        if (button.hasClass(Const.StartButtonDisabledCssClass)) {
            button.removeClass(Const.StartButtonDisabledCssClass);
        }
        button.unbind("click").click(jQuery.proxy(startUpload, this));
    }
    function disableStartButton() {
        var button = $(Const.StartUploadButton);

        if (!button.hasClass(Const.StartButtonDisabledCssClass)) {
            button.addClass(Const.StartButtonDisabledCssClass);
        }
        button.unbind("click");
    }

    function deleteFile(fileId) {
        var f = this.uploader.getFile(fileId);
        var key = f.name.toLowerCase();
        this.uploader.removeFile(f);
        uploadFooterStatus.call(this);
        $("#" + fileId).remove();
        this.currentFileList[key] = null;
        delete this.currentFileList[key];

    }


    return {

        create: function () {
            this.uploader = new plupload.Uploader({
                //UI Elements
                browse_button: Const.BrowseButtonId,
                container: Const.ContainerId,
                drop_element: Const.DropAreaId,
                runtimes: 'html5,silverlight,flash,html4',
                url: '/Project/UploadFile/' + docller.projectId + "/" + docller.folderId,
                max_file_size: '2000mb',
                max_file_count: 3,
                chunk_size: '1mb',
                filters: [
        			                    { title: "PDF & CAD Files", extensions: "pdf,dwg" },
        			                    { title: "Zip files", extensions: "zip" },
                                        { title: "Image files", extensions: "jpg,gif,png,bmp" },
                                        { title: "All files", extensions: "*" }
        		                    ],
                // Flash settings
                flash_swf_url: '/Client/plupload.flash.swf',

                // Silverlight settings
                silverlight_xap_url: '/Client/plupload.silverlight.xap'


            });

            this.uploader.bind('Init', jQuery.proxy(init, this));

            this.uploader.init();

            this.uploader.bind('FilesAdded', jQuery.proxy(filesAdded, this));

            this.uploader.bind('UploadProgress', jQuery.proxy(uploadProgress, this));

            this.uploader.bind('Error', jQuery.proxy(onError, this));

            this.uploader.bind('BeforeUpload', jQuery.proxy(beforeUpload, this));

            this.uploader.bind('FileUploaded', jQuery.proxy(fileUploaded, this));

            this.uploader.bind('UploadComplete', jQuery.proxy(uploadComplete, this));
            ///http://stackoverflow.com/questions/5471141/why-does-add-files-button-in-plupload-not-fire-in-latest-chrome-or-ff-on-os-x
            $(Const.Container).data(Const.Cache, this.uploader);
        },

        destroy: function () {
            this.uploader = $(Const.Container).data(Const.Cache);
            if (this.uploader) {
                ko.cleanNode(document.getElementById(Const.FileListId));
                clearFileList.call(this);
                this.uploader.destroy();
            }
        },

        removeFile: function (file) {
            if (this.uploader) {
                var $this = this;
                deleteFile.call($this, file.uniqueIdentifier);
                if (file.attachments != null) {
                    $.each(file.attachments, function (i, attachment) {
                        deleteFile.call($this, attachment.uniqueIdentifier);
                    });
                }
                ensureVersion.call(this);
            }

        }

    };


})();

//end of uploader

//Start Splitter support
var SplitterPanes = {
    Left: 0,
    Right: 1
};


//Extend jQuery to add a funtion to resize the Splitter
$.fn.resizeSplitter = function (options) {

    // setup default options 
    var config = {
        height: 0,
        resizeToContent: false,
        splitter: null,
        expand: false,
        collapse: false,
        offSetHeight: 0,
        splitterPane: SplitterPanes.Left
    };

    if (options) {
        $.extend(config, options);
    }

    //Get the splitter if not passed in
    if (config.splitter == null) {
        config.splitter = $("#container");
    }

    if (config.resizeToContent == true) {
        var leftPaneHeight = config.splitter.find(".k-pane")[SplitterPanes.Left].scrollHeight;
        var rightPaneHeight = config.splitter.find(".k-pane")[SplitterPanes.Right].scrollHeight;

        if (leftPaneHeight > rightPaneHeight) {
            config.height = leftPaneHeight;
        } else {
            config.height = rightPaneHeight;
        }

        config.height += config.offSetHeight;

    } else if (config.height <= 0) {
        var splitterScorllHeight = config.splitter.find(".k-pane")[config.splitterPane].scrollHeight;

        if (config.expand == true) {
            config.height = (splitterScorllHeight + this.height());
            this.data("increasedHeight", this.height());
        } else if (config.collapse == true) {
            var increasedHeight = 0;
            if (this.data("increasedHeight")) {
                increasedHeight = this.data("increasedHeight");
            }

            config.height = (splitterScorllHeight - increasedHeight);

        }
    }

    if (config.height > 0) {
        config.splitter.height(config.height);
        config.splitter.data("kendoSplitter").trigger("resize");
    }
    return this;
}
//End Splitter support

$(function () {

    var initSplitter = function () {
        var splitter = $("#container");
        if (splitter.length > 0) {
            splitter.kendoSplitter({
                panes: [
                     { collapsible: true, size: "18%", max: "400px" },
                     { collapsible: true }
                   ]
            });
            splitter.resizeSplitter({
                resizeToContent: true,
                offSetHeight: 20,
                splitter: splitter
            });
        }
    };


    var initFilesNavbar = function () {

        $("#nav-upload").click(function (e) {
            e.preventDefault();
            $('#upload-form').dialog({
                modal: true,
                width: 600,
                height: 400,
                title: "Upload Files",
                autoOpen: true,
                resizable: false,
                open: function (event, ui) {
                    var uploader = new docller.fileUploader();
                    uploader.create();
                    //fileUploader();
                },
                close: function (event, ui) {
                    var uploader = new docller.fileUploader();
                    uploader.destroy();
                }
            });

        });
    };


    $(document).ready(function () {
        //initSplitter();
        initFilesNavbar();

    });

});