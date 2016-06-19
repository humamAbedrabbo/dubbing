// To replace a RegEx in a string with another string
String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

function addScene(orderTrnHdrIntno) {
    var url = "/adaptation/addScene";
    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { orderTrnHdrIntno: orderTrnHdrIntno },
        success: function (result) {
            $("#scenesListContainer").empty();
            $("#scenesListContainer").html(result);
            addSceneClickEvent(orderTrnHdrIntno);
                  
        }
    });
}

function deleteScene(orderTrnHdrIntno) {
    var url = "/adaptation/deleteScene";
    var sceneNo = $("#activeScene").val();

    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { orderTrnHdrIntno: orderTrnHdrIntno, sceneNo: sceneNo },
        success: function (result) {
            $("#scenesListContainer").empty();
            $("#scenesListContainer").html(result);
            addSceneClickEvent(orderTrnHdrIntno);

        }
    });
}

function addDialog() {
    var url = "/adaptation/addDialog";
    var orderTrnHdrIntno = $("#orderTrnHdrIntno").val();
    var sceneNo = $("#activeScene").val();
    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { orderTrnHdrIntno: orderTrnHdrIntno, sceneNo: sceneNo },
        success: function (result) {
            $("#dialogListContainer").empty();
            $("#dialogListContainer").html(result);
        }
    });
}

function addSceneClickEvent(orderTrnHdrIntno) {
    $(".scene").click(function () {

        if (!$(this).hasClass("active")) {
            $(".scene").removeClass("active");
            $(this).addClass("active");
            var sceneNo = $(this).data("scene");
            $("#activeScene").val(sceneNo);

            var url = "/adaptation/dialogList";
            $.ajax({
                contentType: 'application/json',
                method: 'GET',
                url: url,
                data: { orderTrnHdrIntno: orderTrnHdrIntno, sceneNo: sceneNo },
                success: function (result) {
                    $("#dialogListContainer").empty();
                    $("#dialogListContainer").html(result);
                   
                }
            });

        }

    });
}