

$(function() {

    $(document).on("click", ".savePermissions", function () {
        var $this = $(this);
        var modal = $($this.data("containerid"));
        var selects = modal.find("select");
        var changedPerm = new Array();
        selects.each(function() {
            var ddl = $(this);
            if (ddl.data("originalperm") != ddl.val()) {
                changedPerm[changedPerm.length] = { entityId: ddl.data("entityid"), permissions: ddl.val() };
            }
        });
        if (changedPerm.length > 0) {
            var data = "changedPermissions=" + JSON.stringify(changedPerm, null);
            modal.block({ message: '<img src="/images/busy.gif" /> Please wait...' });
            $.ajax({
                type: "POST",
                url: $this.data("saveurl"),
                data: data,
                success: function (result) {
                    modal.unblock();
                    window.location.replace(window.location.href.replace("#", ""));
                },
                error: function () {
                    modal.unblock();
                    alert("Errors occured in saving project permissions, Please try again later");
                },
                dataType: "json"
            });
        }
    });
  

});

//docller.editPermissionsGrid = function () {
//    this.grid = null;
//};

//docller.editPermissionsGrid.prototype = (function () {
//    function autocompleteRenderer(instance, td, row, col, prop, value, cellProperties) {
//        //Handsontable.AutocompleteCell.renderer.apply(this, arguments);
//        //td.title = 'Type to show the list of options';
//        //var cell = $(td);
//        //cell.width("15%");
//        var $td = $(td);
//        var $text = $('<div class="htAutocomplete"></div>');
//        var $arrow = $('<div class="htAutocompleteArrow">&#x25BC;</div>');
//        Handsontable.TextCell.renderer(instance, $text[0], row, col, prop, value, cellProperties);

//        if ($text.html() === '') {
//            $text.html('&nbsp;');
//        }
//        $text.append($arrow);
//        $td.empty().append($text);
//    }

//    function autocompleteEditor(instance, td, row, col, prop, keyboardProxy, cellProperties) {
//       keyboardProxy.typeahead({ source: cellProperties.soure });

//    }


//    function cellRenderer(instance, td, row, col, prop, value, cellProperties) {
//        Handsontable.TextCell.renderer.apply(this, arguments);

//    }
//    return {

//        create: function (data) {
//            this.grid = $("#editPermissions-grid");
//            var $this = this;
//            this.grid.handsontable({
//                data: data,
//                startRows: 1,
//                startCols: 1,
//                scrollH: "none",
//                scrollV:"all",
//                colWidths: [75, 100, 75, 50],
//                fillHandle: false,
//                colHeaders: ["Name", "E-mail", "Company",  "Permissions"],
//                columns: [
//                            { data: "displayName",readOnly: true, type: { renderer: cellRenderer } },
//                            { data: "email", readOnly: true, type: { renderer: cellRenderer } },
//                            { data: "companyName", readOnly: true, type: { renderer: cellRenderer } },
//                            { data: "permissions", type: { renderer: autocompleteRenderer, editor: autocompleteEditor }, soure: ["1", "2", "4"] }
//                            //{ data: "status", type: { renderer: autocompleteRenderer } }
//                ],
//                cells: function (row, col, prop) {
//                    var cellProperties = {};
//                    /*if (col === 0) {
//                        cellProperties.readOnly = true; //make cell read-only if it is first row or the text reads 'readOnly'
//                    */

//                    return cellProperties;
//                }/*,
//                autoComplete: [
//                                  {
//                                      match: function (row, col, data) {
//                                          if (col == 3) {
//                                              return true;
//                                          }
//                                          return false;
//                                      },
//                                      source: function () {
//                                          var statusSource = new Array();
//                                          statusSource[0] = "1";
//                                          statusSource[1] = "2";
//                                          statusSource[2] = "4";
//                                          statusSource[3] = "8";

//                                          for (var i = 0; i < docller.statusData.length; i++) {
//                                              var status = docller.statusData[i];
                                              
//                                                  statusSource[i] = status.statusText;
//                                              if (status.statusId > 0) {
//                                                  $this.statusCache[status.statusText] = status.statusId;
//                                              }
//                                          }
//                                          return statusSource;
//                                      },
//                                      strict: true
//                                  }
//                ]*/
//            });
//        }
//    };
//})();
