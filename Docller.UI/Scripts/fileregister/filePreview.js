docller.filePreviews = function() {
};
docller.filePreviews.prototype = (function () {
    function cachePreviews(urls) {
        for (var i = 0; i < urls.length; i++) {
            var image = new Image();
            image.src = urls[i];
        }
    }

    return {
        init: function () {

            //Seadragon.Config.imagePath = "/Images/seadragon/";
            var previews = $(".preview");
            var imageUrls = new Array();
            previews.each(function (i) {
                var $this = $(this);
                var url = "/Project/FilePreview/" + docller.projectId + "/?fileId=" + $this.data("fileid") + "&previewType=PThumb&ptag=" + $this.data("previewtimestamp") + "&internalName=" + $this.data("internalname");
                imageUrls[i] = url;
            });

            var s = $("#previewArea");
            //var pos = s.position();
            $(window).scroll(function () {
                //var windowpos = $(window).scrollTop();
                s.empty();
                //if (pos && windowpos >= pos.top) {
                // //   s.addClass("stick");
                //} else {
                //   // s.removeClass("stick");
                //}
            });
            cachePreviews(imageUrls);
            
            $(".preview").mouseover(function () {
                var previewArea = $("#previewArea");
                previewArea.empty();
                var $this = $(this);
                //var position = $this.offset();
                
                var url = "/Project/FilePreview/" + docller.projectId + "/?fileId=" + $this.data("fileid") + "&previewType=PThumb&ptag=" + $this.data("previewtimestamp") + "&internalName=" + $this.data("internalname");
                var windowpos = $(window).scrollTop();
                
                previewArea.append("<img src='" + url + "'/>");
            });
            var $that = this;
            $(document).on("click", '[data-toggle="zoomablepreview"]', function (e) {
                var $this = $(this);
                
                var fileId = $this.data("fileid");
                var internalName = $this.data("internalname");
                var pTag = $this.data("previewtimestamp");
                var forcezoomable = $this.data("forcezoomable");
                if (docller.utils.currentDevice() == docller.device.DESKTOP || forcezoomable) {
                    $(".zoomablepreview").remove();
                    $that.interactivePreview(fileId, internalName, pTag);
                } else {
                    var zoomablePreviewId = "File_" + fileId + "_zoomablepreview";
                    var zoomablePreviewRow = $("#" + zoomablePreviewId);
                    
                    if (zoomablePreviewRow.length == 0) {
                        var url = "/Project/FilePreview/" + docller.projectId + "/?fileId=" + fileId + "&previewType=PThumb&ptag=" + pTag + "&internalName=" + internalName;

                        var overlayed = "<a data-toggle='zoomablepreview' data-forcezoomable='true' data-fileId='" + fileId + "' data-internalName='" + internalName + "' data-previewtimestamp='" + pTag + "' title='Tap to open a zoomable preview' class='btn btn-mini btn-inverse' href='#'><i class='icon-white icon-eye-open'></i> Interactive Preview</a>";
                        
                        $("<tr class='zoomablepreview' id='" + zoomablePreviewId + "'><td colspan='5' class='overlaid no-table-hover'><img src='" + url + "'/><div class='overlay'>" + overlayed + "</div></td>").insertAfter("#File_" + fileId + "_Row").hide().show("slow", "linear");

                    } else {
                        zoomablePreviewRow.toggle("slow");

                    }
                }
                
                    //if (!window.$seadragonViewer) {
                    //    window.$seadragonViewer = new Seadragon.Viewer("zoomContainer");

                    //    window.$seadragonViewer.addEventListener("resize",
                    //        function (viewer) {
                    //            if (viewer.isFullPage()) {
                    //                var style = viewer.elmt.firstChild.style;
                    //                style.backgroundColor = "#ffffff";
                    //            }
                    //        }, false);

                    //    window.$seadragonViewer.addEventListener("open", function(viewer) {
                    //       // viewer.elmt.firstChild.innerText = "Please Wait Loading ...";
                    //    });
                    //}
                    //window.$seadragonViewer.openDzi("/Project/ZoomablePreview/" + docller.projectId + "/?fileId=" + fileId + "&ptag=" + pTag + "&internalName=" + internalName + "&tile=");
                
                e.preventDefault();
            });

        },
        interactivePreview: function (fileId, internalName, pTag) {
            var dziUrl = "/Project/ZoomablePreview/" + docller.projectId + "?fileId=" + fileId + "&ptag=" + pTag + "&internalName=" + internalName + "&tile=";
            var zoomModal = $("#zoomModal");

            zoomModal.addClass(docller.utils.currentDevice() == docller.device.DESKTOP ? "widemodal" : "modal");
            $("#downloadPreviewed").data("fileid", fileId);

            if (window.$openseadragonViewer) {
                window.$openseadragonViewer.close();
            }
            $.ajax({
                type: "GET",
                url: dziUrl,
                dataType: "xml",
                success: function (dzi) {
                    var xml = $(dzi);
                    var $image = xml.find('Image');
                    var $size = xml.find('Size');
                    //console.log(image);
                    var $dziSource = {
                        Image: {
                            xmlns: $image.attr('xmlns'),
                            Url: dziUrl,
                            Format: $image.attr('Format'),
                            Overlap: $image.attr('Overlap'),
                            TileSize: $image.attr('TileSize'),
                            Size: {
                                Height: $size.attr('Height'),
                                Width: $size.attr('Width')
                            }
                        }
                    };

                    if (!window.$openseadragonViewer) {
                        window.$openseadragonViewer = OpenSeadragon({
                            id: "zoomContainer",
                            prefixUrl: "/images/openseadragon/",
                            tileSources: $dziSource
                        });
                    } else {
                        window.$openseadragonViewer.open($dziSource);
                    }
                    zoomModal.modal();
                }
            });
        }
    };
})();