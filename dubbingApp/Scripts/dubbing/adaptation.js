// To replace a RegEx in a string with another string
String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

// Replace the tags with label spans when finish editing
function addTextTags(scentence) {
    var s = scentence;

    s = s.replace(/\.\.\./g, '[t3]');
    s = s.replace(/\.\./g, '[t2]');
    s = s.replace(/\./g, '[t1]');
    s = s.replace(/\[t1\]/g, '<span class="tag label label-success">.</span>');
    s = s.replace(/\[t2\]/g, '<span class="tag label label-info">..</span>');
    s = s.replace(/\[t3\]/g, '<span class="tag label label-warning">...</span>');

    return s;
}

// Remove all span tags from scentence static text
function removeTextTags(elem) {
    $(elem).children("span").each(function (index) {
        var s = $(this).text();
        $(this).replaceWith(s);
    });

}

// TEMPORARY: add tags to loaded scentences
function fixTags() {
    $(".scentence").each(function (index) {
        var s = $(this).text().trim();
        s = addTextTags(s);
        $(this).html(s);
    });
}

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

function cancelEditSubtitle() {    
    var elem = "#subtitleContainer_" + $(this.event.target).data("subtitle");
    var subtitleIntno = $(this.event.target).data("subtitle");
    var oldSubtitle = $("#oldSubtitle_" + subtitleIntno).val();

    $(elem).empty();
    
    
    var html = '<span onclick="javascript: editSubtitle(' + subtitleIntno + ');">' + addTextTags(oldSubtitle) + '</span>';
    $(elem).html(html);
}

function saveEditSubtitle() {
    var newSubtitle = $(this.event.target).val();
    var elem = "#subtitleContainer_" + $(this.event.target).data("subtitle");
    var subtitleIntno = $(elem).data("subtitle");
    var url = "/adaptation/saveSubtitle";


    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { subtitleIntno: subtitleIntno, newSubtitle: newSubtitle },
        success: function (result) {
            $(elem).empty();
            

            var html = '<span onclick="javascript: editSubtitle(' + subtitleIntno + ');">' + addTextTags(newSubtitle) + '</span>';
            $(elem).html(html);
        }
    });
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

function onSubtitleInputKeyDown() {
    if (this.event.which == 27) {
        cancelEditSubtitle();
    }
    else if (this.event.which == 13) {
        saveEditSubtitle();
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

function editSubtitle(subtitleIntno) {
    var url = "/adaptation/editSubtitle";

    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { subtitleIntno: subtitleIntno },
        success: function (result) {
            $("#subtitleContainer_" + subtitleIntno).empty();
            $("#subtitleContainer_" + subtitleIntno).html(result);
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
                    fixTags();
                }
            });

        }

    });

}

