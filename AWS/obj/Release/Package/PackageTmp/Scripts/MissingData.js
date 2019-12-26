$(function () {
    function logEvent(eventName) {
        var logList = $("#events ul"),
            newItem = $("<li>", { text: eventName });

        logList.prepend(newItem);
    }
    $("#PerContainer").dxDataGrid({
        dataSource: PerDataJ,
        keyExpr: "ID",
        showBorders: true,
        paging: {
            pageSize: 10
        },
        headerFilter: {
            visible: true,
            allowSearch: true
        },


        searchPanel: { visible: true },
        editing: {
            mode: "popup",
            allowUpdating: true,
            allowDeleting: true,
            allowAdding: true,
            editEnabled: true,
            searchPanel: true
        },
        columns: [
            {
                dataField: "UserID",
                caption: "Users",
                width: 125,
                lookup: {
                    dataSource: UserLookupJ,
                    displayExpr: "FirstName",
                    valueExpr: "ID"
                }
            },
            {
                dataField: "ShowMissingData",
                caption: "Show Missing Data",
                width: 200,
                dataType: "boolean"

            }

        ],

        onEditingStart: function (e) {
            logEvent("EditingStart");
        },
        onInitNewRow: function (e) {
            logEvent("InitNewRow");
        },
        onRowInserting: function (e) {
            var newData = JSON.stringify(e.data);
            $.ajax({
                url: "/Admin/MissingDataPermission/Insert",
                dataType: "json",
                data: { "value": newData },
                success: function (result) {
                    deferred.resolve(result.data, {
                        totalCount: result.totalCount,
                        summary: result.summary,
                        groupCount: result.groupCount
                    });
                }
            });
            logEvent("RowInserting");
        },
        onRowInserted: function (e) {
            DevExpress.ui.notify("Permission Applied", "success");
        },
        onRowUpdating: function (e) {
            var newData = JSON.stringify(e.newData);
            var olddata = JSON.stringify(e.oldData);
            $.ajax({
                url: "/Admin/MissingDataPermission/Update",
                dataType: "json",
                data: { "newdata": newData, "olddata": olddata },
                success: function (result) {
                    deferred.resolve(result.data, {
                        totalCount: result.totalCount,
                        summary: result.summary,
                        groupCount: result.groupCount
                    });
                }
            });

            logEvent("RowUpdating");
        },
        onRowUpdated: function (e) {
            DevExpress.ui.notify("Permission Updated", "success");
            logEvent("RowUpdated");
        },
        onRowRemoving: function (e) {
            $.ajax({
                url: "/Admin/MissingDataPermission/Remove",
                dataType: "json",
                data: { "ID": e.data.ID },
                success: function (result) {
                    deferred.resolve(result.data, {
                        totalCount: result.totalCount,
                        summary: result.summary,
                        groupCount: result.groupCount
                    });
                }
            });
            logEvent("RowRemoving");
        },
        onRowRemoved: function (e) {
            DevExpress.ui.notify("Permission Removed", "success");
        },
        onContentReady: function (e) {
            var saveButton = $(".dx-button[aria-label='Save']");
            if (saveButton.length > 0)
                saveButton.click(function (event) {
                    if (!isUpdateCanceled) {
                        DoSomething(e.component);
                        event.stopPropagation();
                    }
                });
        }

    });

    $("#clear").dxButton({
        text: "Clear",
        onClick: function () {
            $("#events ul").empty();
        }
    });
});