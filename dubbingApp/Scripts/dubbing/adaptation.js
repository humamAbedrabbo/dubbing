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

function deleteDialog(dialogIntno) {
    var url = "/adaptation/deleteDialog";
    
    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { dialogIntno: dialogIntno },
        success: function (result) {
            $("#dialogListContainer").empty();
            $("#dialogListContainer").html(result);
        }
    });
}

function editCharacterName(dialogIntno) {
    var url = "/adaptation/editCharacterName";

    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { dialogIntno: dialogIntno },
        success: function (result) {
            $("#charNameContainer_" + dialogIntno).empty();
            $("#charNameContainer_" + dialogIntno).html(result);
        }
    });
}

function cancelEditCharacterName() {
    var elem = "#charNameContainer_" + $(this.event.target).data("dialog");
    $(elem).empty();
    var oldCharacterName = $(elem).data("charname");
    var dialogIntno = $(elem).data("dialog");
    var html = '<a href="javascript: editCharacterName(' + dialogIntno + ');">' + oldCharacterName + '</a>';
    $(elem).html(html);
}

function saveEditCharacterName() {
    var newCharacterName = $(this.event.target).val();
    var elem = "#charNameContainer_" + $(this.event.target).data("dialog");
    var dialogIntno = $(elem).data("dialog");
    var url = "/adaptation/saveCharacterName";


    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { dialogIntno: dialogIntno, newCharacterName: newCharacterName },
        success: function (result) {
            $(elem).empty();
            $(elem).data("charname", newCharacterName);
            $(elem).data("id", result);
            
            var html = '<a href="javascript: editCharacterName(' + dialogIntno + ');">' + newCharacterName + '</a>';
            $(elem).html(html);
        }
    });


    
}

function onCharNameInputKeyDown() {
    if (this.event.which == 27) {
        cancelEditCharacterName();
    }
    else if (this.event.which == 13) {
        saveEditCharacterName();
    }
}

function addSubtitle(dialogIntno) {
    
    var url = "/adaptation/addSubtitle";

    
    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { dialogIntno: dialogIntno },
        success: function (result) {
            $("#subtitleListContainer_" + dialogIntno).empty();
            $("#subtitleListContainer_" + dialogIntno).html(result);
        }
    });
}

function deleteSubtitle(subtitleIntno, dialogIntno) {

    var url = "/adaptation/deleteSubtitle";


    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { subtitleIntno: subtitleIntno },
        success: function (result) {
            $("#subtitleListContainer_" + dialogIntno).empty();
            $("#subtitleListContainer_" + dialogIntno).html(result);
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

