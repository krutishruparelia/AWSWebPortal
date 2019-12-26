$(function () {
    function logEvent(eventName) {
        var logList = $("#events ul"),
            newItem = $("<li>", { text: eventName });

        logList.prepend(newItem);
    }
    $("#stationdisplaygrid").dxDataGrid({
        dataSource: stationsd,
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: true,
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
            allowUpdating: true,
            allowDeleting: true,
            allowAdding: true,
            editEnabled: true,
            searchPanel: true
        },
        columnChooser: {
            enabled: true
        },
        columnFixing: {
            enabled: true
        },
        columns: [
            {
                dataField: "Name",
                caption: "Name",
                fixed: true
            }, 
            {
                dataField: "StationID",
                caption: "StationID",
                fixed: true
            },
            {
                dataField: "Latitude",
                caption: "Latitude"

            },
            {
                dataField: "Longitude",
                caption: "Longitude"

            },
            {
                dataField: "City",
                caption: "City",
                visible: false

            },
            {
                dataField: "District",
                caption: "District"
            },
            {
                dataField: "State",
                caption: "State",
                visible: false
            },
            {
                dataField: "TehsilTaluk",
                caption: "Tehsil/Taluka",
                visible: false
            },
            {
                dataField: "Block",
                caption: "Block",
                visible: false
            },
            {
                dataField: "Village",
                caption: "Village",
                visible: false
            },
            {
                dataField: "Address",
                caption: "Address",
                visible: false
            },
            {
                dataField: "Bank",
                caption: "Bank",
                visible: false
            },
            {
                dataField: "BusStand",
                caption: "Bus Stand",
                visible: false
            },
            {
                dataField: "RailwayStation",
                caption: "Railway Station",
                visible: false
            },
            {
                dataField: "Airport",
                caption: "Airport",
                visible: false
            },
            {
                dataField: "OtherInformation",
                caption: "OtherInformation",
                visible: false
            },
            {
                dataField: "Profile",
                caption: "Profile"
            },
            {
                dataField: "Gain",
                caption: "Gain"
            },
            {
                dataField: "Offset",
                caption: "Offset"
            },
            {
                dataField: "SerialNumber",
                caption: "Serial Number"
            },
            {
                dataField: "ShowInGraph",
                caption: "ShowInGraph"
            },
            {
                dataField: "ShowInGrid",
                caption: "ShowInGrid"
            }
            
        ],

        onEditingStart: function (e) {
            if (DevExpress.ui.dialog.alert("You are trying to edit station Be carefull it may cause Data loss", "Warning!!")) {
                e.cancel = true;
                var newData = JSON.stringify(e.newData);
                var id = e.data.ID;
                window.location = "/Admin/Station/Index?ID=" + id;
            }
        },
        onInitNewRow: function (e) {
            e.cancel = true;
            window.location = "/Admin/Station/";
        },
        onRowInserting: function (e) {
            
            logEvent("RowInserting");
        },
        onRowInserted: function (e) {

            logEvent("RowInserted");
        },
        onRowUpdating: function (e) {
            //var newData = JSON.stringify(e.newData);
            //var olddata = JSON.stringify(e.oldData);
            //$.ajax({
            //    url: "/Admin/Role/Update",
            //    dataType: "json",
            //    data: { "newdata": newData, "olddata": olddata },
            //    success: function (result) {
            //        deferred.resolve(result.data, {
            //            totalCount: result.totalCount,
            //            summary: result.summary,
            //            groupCount: result.groupCount
            //        });
            //    }
            //});

            logEvent("RowUpdating");
        },
        onRowUpdated: function (e) {
            DevExpress.ui.notify("Role Updated", "Updated successfully", "success");

            logEvent("RowUpdated");
        },
        onRowRemoving: function (e) {
            $.ajax({
                url: "/Admin/StationGridDisplay/Remove",
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
            DevExpress.ui.notify("Station Deleted","success");
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