
$(function () {
    $("#statinonid").dxTextBox({
        name: "stationid",
        value: stID
    });

    $("#staionname").dxTextBox({
        name: "stationname",
        value: sname
    });
    $("#latitude").dxTextBox({
        name: "latitude",
        value: Lat

    });
    $("#longnitude").dxTextBox({
        name: "longnitude",
        value: Long

    });
    $("#city").dxTextBox({
        name: "city",
        value: City

    });
    $("#district").dxTextBox({
        name: "district",
        value: District

    });
    $("#state").dxTextBox({
        name: "state",
        value: State
    });
    $("#tahesil").dxTextBox({
        name: "tahesil",
        value: Tahesil

    });
    $("#Block").dxTextBox({
        name: "Block",
        value: Block

    });
    $("#village").dxTextBox({
        name: "village",
        value: Village

    });
    $("#image").dxFileUploader({
        selectButtonText: "Select photo",
        labelText: "",
        accept: "image/*",
        name: "photo",
        uploadMode: "UseForm"
    });

    $("#address").dxTextBox({
        name: "address",
        value: Address

    });
    $("#bank").dxTextBox({
        name: "bank",
        value: Bank

    });
    $("#busstand").dxTextBox({
        name: "busstand",
        value: BusStand

    });
    $("#airport").dxTextBox({
        name: "airport",
        value: Airport

    });
    $("#otherinfo").dxTextBox({
        name: "otherinfo",
        value: other
    });
    if (pname !== "") {

        $("#profile").dxSelectBox({
            dataSource: statonprofile,
            valueExpr: "value",
            displayExpr: "name",
            value: statonprofile[pname - 1].name,
            onValueChanged: function (e) {

                $.ajax({
                    url: "/Admin/Station/getDrodownvalue",
                    dataType: "json",
                    data: { "value": e.value },
                    success: function (result) {
                        //$("#partialview").append("@Html.Action('GridViewPartial')");  
                        $("#partialview").load('Station/GridViewPartial/');
                    }
                });
            }
        });
        $("#partialview").load('/Admin/Station/GridViewPartial/');
    }
    else {
        $("#profile").dxSelectBox({
            dataSource: statonprofile,
            valueExpr: "value",
            displayExpr: "name",
            onValueChanged: function (e) {
                $.ajax({
                    url: "/Admin/Station/getDrodownvalue",
                    dataType: "json",
                    data: { "value": e.value },
                    success: function (result) {
                        //$("#partialview").append("@Html.Action('GridViewPartial')");  
                        $("#partialview").load('GridViewPartial/');
                    }
                });
            }
        });
    }
    if (pname !== "") {
        $("#button").dxButton({
            text: "Update Station",
            type: "success",
            onClick: function () {
                //DevExpress.ui.dialog.alert("Uncomment the line to enable sending a form to the server.", "Click Handler");
                $("#form").submit();
            }
        });
    }
    else {
        $("#button").dxButton({
            text: "Create Station",
            type: "success",
            onClick: function () {
                //DevExpress.ui.dialog.alert("Uncomment the line to enable sending a form to the server.", "Click Handler");
                $("#form").submit();
            }
        });
    }
    


});