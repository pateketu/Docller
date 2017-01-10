/// <reference path="jquery-1.6.2-vsdoc.js" />
/// <reference path="jquery.validate.js" />
/// <reference path="jquery-ui-1.8.11.js" />
/// <reference path="jquery.unobtrusive-ajax.js" />
/// <reference path="jquery.dynatree.js" />
/// <reference path="Docller.js" />

(function ($) {

    $("#tree").dynatree({
        fx: { height: "toggle", duration: 300 },
        clickFolderMode: 1,
        persist: false,
        onActivate: function (node) {
            if (node.data.href) {
                window.location.href = node.data.href;
            }
        },
        //onExpand: function (expand, node) {
        //    var tree = $("#tree");
        //    var height = calculateHeight($(node.ul), 0);

        //    if (expand == false) {
        //        height = height * -1;
        //    }

        //},
        children: docller.treeData
    });


    //var calculateHeight = function (node, height) {
    //    var allLi = node.children("li");
    //    allLi.each(function (i) {
    //        var li = $(this);
    //        height += li.height();
    //        var all = li.children("ul");
    //        if (all.length > 0) {
    //            calculateHeight(all, height);
    //        }
    //    });

    //    return height;
    //};

})(jQuery);