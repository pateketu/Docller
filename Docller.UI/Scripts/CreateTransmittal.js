/// <reference path="jquery-1.6.2-vsdoc.js" />
/// <reference path="jquery.validate.js" />
/// <reference path="jquery-ui-1.8.11.js" />
/// <reference path="jquery.unobtrusive-ajax.js" />
///<reference path="~/Scripts/plugins/knockout-2.1.0.js"/>

$(function () {
    jQuery.validator.addMethod("requiredfortransmittal", function (value, element, param) {
        if (!window.$transmittal.isTransmitting)
            return true;
        if (value == 0) {
            value = null;
        }
        if (value != null) {
            var val = $.trim(value);
            return val.length != 0;
        }
        return false;
    });
    jQuery.validator.unobtrusive.adapters.addBool("requiredfortransmittal");
    
    ///Just fire off a async call to cache up the Subscribers 
}(jQuery));

docller.createTransmittal = function (files) {
    this.isTransmitting = false;
    this.files = ko.observableArray(files
    );
};

docller.createTransmittal.prototype = (function () {
    
    function handleRemovedItem(removedItem, target, data) {
        var currentVal = target.val();
        if (currentVal.length > 0) {
            if (currentVal.indexOf(removedItem.id) >= 0) {
                var newValue = "";
                
                for (var i = 0; i < data.length; i++) {
                    if (data[i] != removedItem.id) {
                        newValue += data[i] + ",";
                    }
                }
                target.val(newValue);
            }
            
        }
    }

    function getInitData(idValues, textValues) {
        var ids = idValues.split(",");
        var texts = textValues.split(",");
        var data = [];
        
        for (var i = 0; i < ids.length; i++) {
            data.push({ id: ids[i], text: texts[i] });
        }
        return data;
    }

    return {
        
        initFields: function () {
            $("#To").select2({
                placeholder: "Select individuals or companies",
                multiple: true,
                minimumInputLength: 1,
                quietMillis: 100,
                ajax: {
                    url: "/Project/GetProjectSubscribers",
                    dataType: 'json',
                    data: function (term, page) { // page is the one-based page number tracked by Select2
                        return {
                          
                            projectId: docller.projectId,
                            searchString: term, //search term
                        };
                    },
                    results: function (data, page) {
                        return { results: data };
                    }
                },
                initSelection:function(element, callback) {
                    var data = getInitData(element.val(), $("#ToText").val());
                    callback(data);
                }
            });
            
            $("#To").change(function (e) {
                if (e.added) {
                    var errorContainer = $(document).find("[data-valmsg-for='To']");
                    errorContainer.addClass("field-validation-valid").removeClass("field-validation-error");
                }
                if (e.removed) {
                    handleRemovedItem(e.removed, $(e.target), e.val);
                }
            });

            
            $("#Cc").select2({
                placeholder: "Select individuals or companies",
                multiple: true,
                minimumInputLength: 1,
                quietMillis: 100,
                ajax: {
                    url: "/Project/GetProjectSubscribers",
                    dataType: 'json',
                    data: function (term, page) { // page is the one-based page number tracked by Select2
                        return {

                            projectId: docller.projectId,
                            searchString: term, //search term
                        };
                    },
                    results: function (data, page) {
                        return { results: data };
                    }
                },
                initSelection: function (element, callback) {
                    var data = getInitData(element.val(), $("#CcText").val());
                    callback(data);
                }
            });

            $("#Cc").change(function(e) {
                if (e.removed) {
                    handleRemovedItem(e.removed, $(e.target), e.val);
                }
            });

        },
        removeFile: function(file) {
            /*var a = $(e.target).parent();
            var fileId = a.data("removefileid");
            $("#remove-" + fileId).remove();
            var all = $("#filesTable").find("a");
            if (all.length == 1) {
                all.remove();
            }*/
            window.$transmittal.files.remove(file);
            $("#FileCount").val(window.$transmittal.files().length);
            
        },
        transmit: function () {
            this.isTransmitting = true;
        },
        saveTransmittal:function() {
            this.isTransmitting = false;
        }
    };
})();

$(document).ready(function () {
    var transmittal = new docller.createTransmittal(docller.transmittalFiles);
    transmittal.initFields();
    $("#transmit").click(jQuery.proxy(transmittal.transmit, transmittal));
    $("#saveTransmittal").click(jQuery.proxy(transmittal.saveTransmittal, transmittal));
    //$("#filesTable").find("a").click(transmittal.removeFile);
    window.$transmittal = transmittal;
    

    ko.applyBindings(window.$transmittal, document.getElementById("filesTable"));

});

$(document).on("docller.filePicker.filesSelected", function (e, files) {
    //this might not work in all IE versions
    //http://stackoverflow.com/questions/9715936/replace-all-elements-in-knockout-js-observablearray
    $("#FileCount").val(files.length);
    window.$transmittal.files(files);
});

$(document).on("docller.filePicker.onload", function (e, data) {
    data.files = window.$transmittal.files();
    
});