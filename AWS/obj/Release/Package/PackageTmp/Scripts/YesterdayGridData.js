
$(function () {
    function logEvent(eventName) {
        var logList = $("#events ul"),
            newItem = $("<li>", { text: eventName });

        logList.prepend(newItem);
    }
    $("#grid").dxDataGrid({
        dataSource: JSON.parse(Yesterdaygriddata),
        showBorders: true,
      
        headerFilter: {
            visible: true,
            allowSearch: true
        },
        paging: { enabled: false },
        searchPanel: { visible: true },
      
    });

    $("#clear").dxButton({
        text: "Clear",
        onClick: function () {
            $("#events ul").empty();
        }
    });
});