/// <reference path="_references.js" />
/**
* A KnockoutJs binding handler for the html tables javascript library DataTables.
*
* File:         knockout.bindings.dataTables.js
* Author:       Lucas Martin
* License:      Creative Commons Attribution 3.0 Unported License. http://creativecommons.org/licenses/by/3.0/ 
* 
* Copyright 2011, All Rights Reserved, Cognitive Shift http://www.cogshift.com  
*
* For more information about KnockoutJs or DataTables, see http://www.knockoutjs.com and http://www.datatables.com for details.                    
*/
ko.bindingHandlers['dataTable'] = {
    _onInitialisingEventName: "ko_bindingHandlers_dataTable_onInitialising",

    addOnInitListener: function (handler) {
        /// <Summary>
        /// Registers a event handler that fires when the Data Table is being initialised.
        /// </Summary>
        $(document).bind(ko.bindingHandlers.dataTable._onInitialisingEventName, handler);
    },
    removeOnInitListener: function (handler) {
        /// <Summary>
        /// Unregisters an event handler to the onInitialising event.
        /// </Summary>
        $(document).unbind(ko.bindingHandlers.dataTable._onInitialisingEventName, handler);
    },
    'init': function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var binding = ko.utils.unwrapObservable(valueAccessor());
        var isInitialisedKey = "ko.bindingHandlers.dataTable.isInitialised";
        var options = {};

        if ($.data(element, isInitialisedKey) === true)
            return;

        // ** Initialise the DataTables options object with the data-bind settings **

        // Clone the options object found in the data bindings.  This object will form the base for the DataTable initialisation object.
        if (binding.options)
            options = $.extend(options, binding.options);

        // Define the tables columns.
        if (binding.columns && binding.columns.length) {
            options.aoColumns = [];
            ko.utils.arrayForEach(binding.columns, function (col) {

                if (typeof col == "string") {
                    col = { mDataProp: col }
                }

                options.aoColumns.push(col);
            })
        }


        $(".dataTable tbody tr").live("click", function (event) {
            var oTable = $('.dataTable').dataTable();
            //$("td.row_selected", oTable.fnGetNodes()).removeClass('row_selected');
            $("table tbody tr td").removeClass('row_selected');
            $(event.target).parent().find("td").addClass('row_selected');
        });

        if (binding.rowClick && binding.rowClick != '') {

            options.fnRowCallback = function (row, data, displayIndex, displayIndexFull) {

                $(row).mouseover(function () {
                    //$(row).addClass("hovering"); 
                });

                $(row).mouseout(function () {
                    //$(row).removeClass("hovering");
                });

                $(row).click(function (e) {
                    binding.rowClick(data);
                    
                });

                return row;
            }
        }


        // Register the row template to be used with the DataTable.
        if (binding.rowTemplate && binding.rowTemplate != '') {
            options.fnRowCallback = function (row, data, displayIndex, displayIndexFull) {
                // Render the row template for this row.
                ko.renderTemplate(binding.rowTemplate, bindingContext.createChildContext(data), null, row, "replaceChildren");
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
                    dataSource.call(viewModel, ko.bindingHandlers['dataTable'].convertDataCriteria(criteria), function (result) {
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
                        var tableRows = dataTable.fnGetNodes();

                        // If the table contains rows...
                        if (tableRows.length) {
                            // Clear the datatable of rows, and if there are no items to display
                            // in newItems, force the fnClearTables call to rerender the table (because
                            // the call to fnAddData with a newItems.length == 0 wont rerender the table).
                            dataTable.fnClearTable(newItems.length == 0);
                        }

                        // Unwrap the items in the data source if required.
                        var unwrappedItems = [];
                        ko.utils.arrayForEach(newItems, function (item) {
                            unwrappedItems.push(ko.utils.unwrapObservable(item));
                        });

                        // Add the new data back into the data table.
                        dataTable.fnAddData(unwrappedItems);

                        // Unregister each of the table rows from knockout.
                        // NB: This must be called after fnAddData and fnClearTable are called because we want to allow
                        // DataTables to fire it's draw callbacks with the table's rows in their original state.  Calling
                        // this any earlier will modify the tables rows, which may cause issues with third party plugins that 
                        // use the data table.
                        ko.utils.arrayForEach(tableRows, function (tableRow) { ko.cleanNode(tableRow); });
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
                    ko.applyBindingsToNode(newCell[0], { text: accesor }, bindingContext.createChildContext(srcData));
                });

                $(row).attr('id', srcData.Id);

                return destRow[0];
            }
        }

        // Before the table has it's rows rendered, we want to scan the table for elements with knockout bindings
        // and bind them to the current binding context.  This is so you can bind elements like the header row of the
        // table to observables your view model.  Ideally, it would be great to call ko.applyBindingsToNode here,
        // but when we initialise the table with dataTables, it seems dataTables recreates the elements in the table
        // during it's initialisation proccess, killing any knockout bindings you apply before initialisation.  Instead,
        // we mark the elements to bind here with the ko-bind class so we can recognise the elements after the table has been initialised,
        // for binding.
        $(element).find("[data-bind]").each(function (i, childElement) {
            $(childElement).addClass("ko-bind");
        });

        // Fire the onInitialising event to allow the options object to be globally edited before the dataTables table is initialised.  This
        // gives third party javascript the ability to apply any additional settings to the dataTable before load.
        $(document).trigger(ko.bindingHandlers.dataTable._onInitialisingEventName, { options: options });

        // Mark the table element as initialised, and then initialise the table.
        // NB:  There is a funky behaviour in KnockoutJS which causes the init function of this handler to be invoked twice on the same
        // element.  To prevent the data table from being initialised twice we add an initialisation flag to the element here, and we
        // check for the flag at the top of this function to prevent the table from being initialised again.  This funky behaviour generally
        // occurs when the following circumstances are true:
        //      1) You've specified a row template for this binding.
        //      2) You've embedded a knockout bound drop down list within the row template.
        // In all other situations I've seen, the init function of this binding normally only fires once.
        $.data(element, isInitialisedKey, true);
        $(element).dataTable(options);

        // Apply bindings to those elements that were marked for binding.  See comments above.
        $(element).find(".ko-bind").each(function (e, childElement) {
            ko.applyBindingsToNode(childElement, null, bindingContext);
            $(childElement).removeClass("ko-bind");
        });

        // Tell knockout that the control rendered by this binding is capable of managing the binding of it's descendent elements.
        // This is crucial, otherwise knockout will attempt to rebind elements that have been printed by the row template.
        return { controlsDescendantBindings: true };
    },

    /*
    // This function transforms the data format that DataTables uses to transfer paging and sorting information to the server
    // to something that is a little easier to work with on the server side.  The resulting object should look something like 
    // this in C#
    public class DataGridCriteria
    {
    public int RecordsToTake { get; set; }
    public int RecordsToSkip { get; set; }
    public string GlobalSearchText { get; set; }

    public ICollection<DataGridColumnCriteria> Columns { get; set; }
    }

    public class DataGridColumnCriteria
    {
    public string ColumnName { get; set; }
    public bool IsSorted { get; set; }
    public int SortOrder { get; set; }
    public string SearchText { get; set; }
    public bool IsSearchable { get; set; }
    public SortDirection SortDirection { get; set; }
    }

    public enum SortDirection
    {
    Ascending,
    Descending
    }
    */
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
            else if (cog.utils.string.startsWith(item.name, "bSearchable_"))
                destOptions.Columns[colIndex].IsSearchable = item.value;
            else if (cog.utils.string.startsWith(item.name, "sSearch_"))
                destOptions.Columns[colIndex].SearchText = item.value;
            else if (cog.utils.string.startsWith(item.name, "mDataProp_"))
                destOptions.Columns[colIndex].ColumnName = item.value;
            else if (cog.utils.string.startsWith(item.name, "iSortCol_")) {
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

