
$(function () {
    $("#lookup").dxLookup({
        items: alluser,
        showPopupTitle: false,
        onValueChanged: function (e) {
            var previousValue = e.previousValue;
            var newValue = e.value;
            $.ajax({
                url: "/Admin/PermissionMaster/PermissionMaster",
                dataType: "json",
                data: { "value": newValue },
                success: function (result) {
                 
                    showgrid(result);
                }
            });
            
        }
    });

    var dataGrid;
    
    var showLoadPanel = function () {
        loadPanel.show();
    };

    $(".show-panel").dxButton({
        stylingMode: "contained",
        text: "Assign Rights",
        type: "default",
        onClick: showLoadPanel
    });

    var loadPanel = $(".loadpanel").dxLoadPanel({
        shadingColor: "rgba(0,0,0,0.4)",
        position: { of: "#form-container" },
        visible: false,
        showIndicator: true,
        showPane: true,
        shading: true,
        closeOnOutsideClick: false,
        onShown: function () {
            setTimeout(function () {
                loadPanel.hide();
            }, 3000);
        },
        onHidden: function () {
            var data = $("#selected-items-container").text();
            var selectedUser = $("#lookup").dxLookup('option', 'value');
            $.ajax({
                url: "/Admin/PermissionMaster/Insert",
                data: { "ID": data, "user": selectedUser },
                success: function (result) {
                    if (DevExpress.ui.dialog.alert("Permission Applied.", "Success")) {
                        dataGrid.refresh();
                    }
                }
            });
        }
    }).dxLoadPanel("instance");

});

function showgrid(parsejson) {
    dataGrid = $("#grid-container").dxDataGrid({
        dataSource: stationalldata,
        allowColumnReordering: true,
        allowColumnResizing: true,
        columnAutoWidth: true,
        keyExpr: "ID",
        selection: {
            mode: "multiple"
        },
        editing: {
            texts: {
                confirmDeleteMessage: ''
            }
        },
        paging: {
            pageSize: 10
        },
        headerFilter: {
            visible: true,
            allowSearch: true
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
                caption: "Gain",
                visible: false
            },
            {
                dataField: "Offset",
                caption: "Offset",
                visible: false
            },
            {
                dataField: "SerialNumber",
                caption: "Serial Number",
                visible: false
            },
            {
                dataField: "ShowInGraph",
                caption: "ShowInGraph",
                visible: false
            },
            {
                dataField: "ShowInGrid",
                caption: "ShowInGrid",
                visible: false
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
        filterRow: { visible: true },
        searchPanel: { visible: true },
        onSelectionChanged: function (selectedItems) {
            var data = selectedItems.selectedRowsData;
            if (data.length > 0)
                $("#selected-items-container").text(
                    $.map(data, function (value) {
                        return value.ID;
                    }).join(", "));
            else
                $("#selected-items-container").text("Nobody has been selected");
        },
        selectedRowKeys: JSON.parse("[" + parsejson + "]")

    }).dxDataGrid("instance");

}
