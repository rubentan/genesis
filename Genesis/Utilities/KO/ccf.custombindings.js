
//deferedComputed = function(readfunc){     
//    return ko.computed({read:readfunc, deferEvaluation:true}); 
//}; 
//deferedObs = deferedComputed(function() {     
///* code to compute the observables value */
//});
/// <reference path="../../jquery-1.7.2.min.js" />

ko.bindingHandlers['class'] = {
    'update': function (element, valueAccessor) {
        if (element['__ko__previousClassValue__']) {
            ko.utils.toggleDomNodeCssClass(element, element['__ko__previousClassValue__'], false);
        }
        var value = ko.utils.unwrapObservable(valueAccessor());
        ko.utils.toggleDomNodeCssClass(element, value, true);
        element['__ko__previousClassValue__'] = value;
    }
};


ko.bindingHandlers.autocomplete = {
    init: function (element, valueAccessor, allBindings, viewModel) {
        var $element = $(element),
			value = valueAccessor(),
            $config = typeof(value) === 'function' ? value() : value;

        $element.autocomplete($config);

        $config.search = function (term) {
            if ($element.autocomplete("widget").is(":visible")) {
                $element.autocomplete("close");
                return;
            }

            $element.addClass("searching");
            $element.autocomplete("search", term);
            $element.focus();
        };

        // allow override of render item
        if ($config.renderItem !== undefined) {
            $element.data("autocomplete")._renderItem = $config.renderItem;
        }

        $element.data("autocomplete")._resizeMenu = function () {
            var ul = this.menu.element;
            ul.outerWidth(this.element.outerWidth());
        };
    }
};


ko.bindingHandlers.watermark = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var value = valueAccessor(), allBindings = allBindingsAccessor();
        var defaultWatermark = ko.utils.unwrapObservable(value);
        var $element = $(element)

        setTimeout(function () {
            $element.val(defaultWatermark).css("color", '#aaa');
        }, 0);

            function clearMessage() {
                if ($element.val() == defaultWatermark)
                    $element.val("");
                    $element.css("color", '#aaa');
            }

            function insertMessage() {
                if ($element.val().length == 0 || $element.val() == defaultWatermark) {
                    $element.val(defaultWatermark);
                    $element.css("color", '#aaa');
                } else
                    $element.css("color", '#272727');
            }


            $element.focus(clearMessage);
            $element.blur(insertMessage);
            $element.change(insertMessage);
    }
};



ko.bindingHandlers.yesno = { update: function(element,valueAccessor){         
    var text = 'no';         
    var value = ko.utils.unwrapObservable(valueAccessor());         
    if(value) { text = 'yes'; } $(element).text(text); 
    } 
 };


//ADRIAN AUG 2013 : Range Extenders
 ko.extenders.range = function (target, range) {



     target.subscribe(function (newValue) {

         if (!(newValue >= range.min && newValue <= range.max)) {
             alert('This field accepts numeric values from ' + range.min + ' to ' + range.max + ' only.\n No decimal and negative values.');
             target('');
         }

     });


     return target;


 };


 ko.extenders.decimalAllowNegVal = function (target, precision) {
     //create a writeable computed observable to intercept writes to our observable 

     var result = ko.computed({
         read: target,  //always return the original observables value 
         write: function (newValue) {
             var current = target();
             roundingMultiplier = Math.pow(10, precision);

             var wholeNumber;
             if (newValue != undefined) {
                 wholeNumber = newValue.split('.');
             }

             newValueAsNum = isNaN(newValue) || wholeNumber[0].length > 10 ? 0 : parseFloat(+newValue);
             valueToWrite = Math.round(newValueAsNum * roundingMultiplier) / roundingMultiplier;

             //                if (!$.isEmptyObject(newValue)){
             //                    if (newValue.match('0.') == "0."){
             //                        newValueAsNum = 0;
             //                    }
             //                }

             //only write if it changed 
             if (valueToWrite !== current) {
                 target(valueToWrite);
             } else {
                 //if the rounded value is the same, but a different value was written, force a notification for the current field 
                 if (newValue !== current) {
                     target.notifySubscribers(valueToWrite);
                 }
             }
         }
     });

     //initialize with current value to make sure it is rounded appropriately 
     result(target());

     //return the new computed observable 
     return result;
 };




ko.extenders.decimal = function(target, precision) {   
    //create a writeable computed observable to intercept writes to our observable 

    var result = ko.computed({ 
        read: target,  //always return the original observables value 
        write: function(newValue) { 
            var current = target();
                roundingMultiplier = Math.pow(10, precision);

                var wholeNumber;
                if (newValue != undefined){
                    wholeNumber = newValue.split('.');
                }
                newValueAsNum = isNaN(newValue) || (newValue < 0 || wholeNumber[0].length > 10 ) ? 0 : parseFloat(+newValue);
                valueToWrite = Math.round(newValueAsNum * roundingMultiplier) / roundingMultiplier;

//                if (!$.isEmptyObject(newValue)){
//                    if (newValue.match('0.') == "0."){
//                        newValueAsNum = 0;
//                    }
//                }
                 
            //only write if it changed 
            if (valueToWrite !== current) { 
                target(valueToWrite); 
            } else { 
                //if the rounded value is the same, but a different value was written, force a notification for the current field 
                if (newValue !== current) { 
                    target.notifySubscribers(valueToWrite); 
                } 
            } 
        } 
    }); 
  
    //initialize with current value to make sure it is rounded appropriately 
    result(target()); 
  
    //return the new computed observable 
    return result; 
};


ko.extenders.required = function (target, overrideMessage) {
    //add some sub-observables to our observable     
    target.hasError = ko.observable();
    target.validationMessage = ko.observable();
        
    //define a function to do validation     
    function validate(newValue) {
        target.hasError(newValue ? false : true);
        target.validationMessage(newValue ? "" : overrideMessage || "");
    }

    //initial validation     
    validate(target());

    //validate whenever the value changes     
    target.subscribe(validate);

    //return the original observable     
    return target;
}; 



ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        //initialize datepicker with some optional options
        var options = allBindingsAccessor().datepickerOptions || {};
        $(element).datepicker(options);

        //handle the field changing
        ko.utils.registerEventHandler(element, "change", function () {
            var observable = valueAccessor();
            observable($(element).datepicker("getDate"));
        });

        //handle disposal (if KO removes by the template binding)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).datepicker("destroy");
        });

    },
    //update the control when the view model changes
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor()),
            current = $(element).datepicker("getDate");

        if (value - current !== 0) {
            $(element).datepicker("setDate", value);
        }
    }
};

ko.bindingHandlers.jqButton = {
    init: function (element) {
        $(element).button(); // Turns the element into a jQuery UI button
    },
    update: function (element, valueAccessor) {
        var currentValue = valueAccessor();
        // Here we just update the "disabled" state, but you could update other properties too
        $(element).button("option", "disabled", currentValue.enable === false);
    }
};






//custom dialogForm
        //rviaje 06/13/2012
        ko.bindingHandlers.dialog = {
        init: function(element, valueAccessor, allBindingsAccessor) {
            var options = ko.utils.unwrapObservable(valueAccessor()) || {};
            //do in a setTimeout, so the applyBindings doesn't bind twice from element being copied and moved to bottom
            setTimeout(function() { 
                options.close = function() {
                    allBindingsAccessor().dialogVisible(false);                        
                };
                
                $(element).dialog(options);          
            }, 0);
            
            //handle disposal (not strictly necessary in this scenario)
             ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
                 $(element).dialog("destroy");
             });   
        },
        update: function(element, valueAccessor, allBindingsAccessor) {
             var shouldBeOpen = ko.utils.unwrapObservable(allBindingsAccessor().dialogVisible);
             $(element).dialog(shouldBeOpen ? "open" : "close");
             
        }
};



//jquery grid 
ko.bindingHandlers.grid = {

    init: function (element, valueAccessor) {
        var value = valueAccessor();
        var dataArr = ko.utils.unwrapObservable(value.data).slice(0);

        var grid = $(element).jqGrid({
            data: dataArr,
            datatype: "local",
            localReader:
                        { repeatitems: false,
                            id: value.rowId
                        },
            gridview: true,
            height: 150,
            hoverrows: false,
            colModel: value.colModel,
            pager: value.pager,
            rowNum: 10,
            onSelectRow: function (id) {
                var observable = valueAccessor();
                var item = $(element).jqGrid('getLocalRow', id);
                $(element).jqGrid('editRow', id, true);
                observable.selectedItem(item);
            }


        });

        $(element).jqGrid("setGridParam", { data: ko.utils.unwrapObservable(value.data).slice(0) });

        

    },
    update: function (element, valueAccessor) {
        var value = valueAccessor();
        var gridData = $(element).jqGrid('getGridParam', 'data');
        var newData = ko.utils.unwrapObservable(value.data);

        var prevPage = $(element).jqGrid("getGridParam", 'page');
        var rowNum = parseInt($(element).jqGrid("getGridParam", 'rowNum'), 10);
        var lastPage = Math.ceil(newData.length / rowNum);

        $(element).jqGrid('clearGridData')
                      .jqGrid('setGridParam', { data: newData })
                      .jqGrid('setGridParam', { lastpage: lastPage })
                      .trigger('reloadGrid', [{ page: prevPage}]);


    }
};

//dataTables
ko.bindingHandlers['dataTable'] = {
    'init': function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        if ($.data(element, isInitialisedKey) === true)
            return;

        var binding = ko.utils.unwrapObservable(valueAccessor());
        var isInitialisedKey = "ko.bindingHandlers.dataTable.isInitialised";
        var options = {};

        // ** Initialise the DataTables options object with the data-bind settings **

        // Clone the options object found in the data bindings.  This object will form the base for the DataTable initialisation object.
        if (binding.options)
            options = $.extend(options, binding.options);

        // Define the tables columns.
        if (binding.columns && binding.columns.length) {
            options.aoColumns = [];
            ko.utils.arrayForEach(binding.columns, function (col) {
                options.aoColumns.push({ mDataProp: col });
            })
        }

        // Register the row template to be used with the DataTable.
        if (binding.rowTemplate && binding.rowTemplate != '') {
            options.fnRowCallback = function (row, data, displayIndex, displayIndexFull) {
                // Render the row template for this row.
                $(row).mouseover(function () {
                // $(row).attr("style", "background-color:#E6FF99 !important;");
                //$('td.odd, td.even').attr("style", "background-color:#E6FF99 !important;");

                });
                $(row).mouseout(function () {
                    //$(row).removeAttr("style");
                    //$('td.odd, td.even').removeAttr("style");
                });

                //add tr id attr for template
                $(row).attr('id',data.Id ); 

                ko.renderTemplate(binding.rowTemplate, data, null, row, "replaceChildren");

                return row;
            }
        }


        // row selected highlighted
        $('.dataTable tbody tr').live("click", function() {
		    $('.dataTable tbody tr').removeClass('row_selected');
			$(this).addClass('row_selected');
	     });


        if (binding.rowClick && binding.rowClick != '') {
           
                options.fnRowCallback = function (row, data, displayIndex, displayIndexFull) {
                
                $(row).mouseover(function () {
                    //$(row).addClass("hovering");
                });

                $(row).mouseout(function () {
                    //$(row).removeClass("hovering");
                });
               
                $(row).click(function (event) {
                    binding.rowClick(data);
                });

                //add tr id attr for template
                $(row).attr('id',data.Id ); 

                return row;
            }
        }

        // Set the data source of the DataTable.
        if (binding.dataSource) {
            var dataSource = ko.utils.unwrapObservable(binding.dataSource);

            // If the data source is a function that gets the data for us...
            if (typeof dataSource == 'function' && dataSource.length == 2) {
                // Register a fnServerData callback which calls the data source function when the DataTable requires data.
                options.fnServerData = function (source, criteria, callback) {
                    dataSource(ko.bindingHandlers['dataTable'].convertDataCriteria(criteria), function (result) {
                        callback({
                            aaData: ko.utils.unwrapObservable(result.Data),
                            iTotalRecords: ko.utils.unwrapObservable(result.TotalRecords),
                            iTotalDisplayRecords: ko.utils.unwrapObservable(result.DisplayedRecords)
                        });
                    });
                }

                // In this data source scenario, we are relying on the server processing.
                options.bProcessing = true;
                options.bServerSide = true;
            }
            // If the data source is a javascript array...
            else if (dataSource instanceof Array) {
                // Set the initial datasource of the table.
                options.aaData = ko.utils.unwrapObservable(binding.dataSource);

                // If the data source is a knockout observable array...
                if (ko.isObservable(binding.dataSource)) {
                    // Subscribe to the dataSource observable.  This callback will fire whenever items are added to 
                    // and removed from the data source.
                    binding.dataSource.subscribe(function (newItems) {
                        // ** Redraw table **
                        var dataTable = $(element).dataTable();
                        // Get a list of rows in the DataTable.
                        var tableNodes = dataTable.fnGetNodes();
                        // If the table contains rows...
                        if (tableNodes.length) {
                            // Unregister each of the table rows from knockout.
                            ko.utils.arrayForEach(tableNodes, function (node) { ko.cleanNode(node); });
                            // Clear the datatable of rows.
                            dataTable.fnClearTable();
                        }

                        // Unwrap the items in the data source if required.
                        var unwrappedItems = [];
                        ko.utils.arrayForEach(newItems, function (item) {
                            unwrappedItems.push(ko.utils.unwrapObservable(item));
                        });

                        // Add the new data back into the data table.
                        dataTable.fnAddData(unwrappedItems);
                    });
                }


            }
            // If the dataSource was not a function that retrieves data, or a javascript object array containing data.
            else {
                throw 'The dataSource defined must either be a javascript object array, or a function that takes special parameters.';
            }
        }

        // If no fnRowCallback has been registered in the DataTable's options, then register the default fnRowCallback.
        // This default fnRowCallback function is called for every row in the data source.  The intention of this callback
        // is to build a table row that is bound it's associated record in the data source via knockout js.
        if (!options.fnRowCallback) {
            options.fnRowCallback = function (row, srcData, displayIndex, displayIndexFull) {
                var columns = this.fnSettings().aoColumns

                // Empty the row that has been build by the DataTable of any child elements.
                var destRow = $(row);
                destRow.empty();

                // For each column in the data table...
                ko.utils.arrayForEach(columns, function (column) {
                    var columnName = column.mDataProp;
                    // Create a new cell.
                    var newCell = $("<td></td>");
                    // Insert the cell in the current row.
                    destRow.append(newCell);
                    // bind the cell to the observable in the current data row.
                    var accesor = eval("srcData['" + columnName.replace(".", "']['") + "']");
                    ko.applyBindingsToNode(newCell[0], { text: accesor }, srcData);
                });

                return destRow[0];
            }
        }

        $(element).dataTable(options);
        $.data(element, isInitialisedKey, true);

        // Tell knockout that the control rendered by this binding is capable of managing the binding of it's descendent elements.
        // This is crutial, otherwise knockout will attempt to rebind elements that have been printed by the row template.
        return { controlsDescendantBindings: true };
    },

    convertDataCriteria: function (srcOptions) {
        var getColIndex = function (name) {
            var matches = name.match("\\d+");

            if (matches && matches.length)
                return matches[0];

            return null;
        }

        var destOptions = { Columns: [] };

        // Figure out how many columns in in the data table.
        for (var i = 0; i < srcOptions.length; i++) {
            if (srcOptions[i].name == "iColumns") {
                for (var j = 0; j < srcOptions[i].value; j++)
                    destOptions.Columns.push(new Object());
                break;
            }
        }

        ko.utils.arrayForEach(srcOptions, function (item) {
            var colIndex = getColIndex(item.name);

            if (item.name == "iDisplayStart")
                destOptions.RecordsToSkip = item.value;
            else if (item.name == "iDisplayLength")
                destOptions.RecordsToTake = item.value;
            else if (item.name == "sSearch")
                destOptions.GlobalSearchText = item.value;
            else if (cog.string.startsWith(item.name, "bSearchable_"))
                destOptions.Columns[colIndex].IsSearchable = item.value;
            else if (cog.string.startsWith(item.name, "sSearch_"))
                destOptions.Columns[colIndex].SearchText = item.value;
            else if (cog.string.startsWith(item.name, "mDataProp_"))
                destOptions.Columns[colIndex].ColumnName = item.value;
            else if (cog.string.startsWith(item.name, "iSortCol_")) {
                destOptions.Columns[item.value].IsSorted = true;
                destOptions.Columns[item.value].SortOrder = colIndex;

                var sortOrder = ko.utils.arrayFilter(srcOptions, function (item) {
                    return item.name == "sSortDir_" + colIndex;
                });

                if (sortOrder.length && sortOrder[0].value == "desc")
                    destOptions.Columns[item.value].SortDirection = "Descending";
                else
                    destOptions.Columns[item.value].SortDirection = "Ascending";
            }
        });

        return destOptions;
    }
};
