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

function getOrderTrnHdrIntno()
{
    return $("#orderTrnHdrIntno").val();
}

function getActiveScene()
{
    return $("#activeScene").val();
}

function getActiveSceneIntno() {
    var sceneNo = getActiveScene();
    var elem = $(".scene[data-sceneno='" + sceneNo + "']");
    return elem.data("sceneintno");
}

function setActiveScene(sceneNo)
{
    $("#activeScene").val(sceneNo);
    $(".scene").removeClass("active");
    $(".scene[data-sceneno='" + sceneNo + "']").addClass("active");
    sceneDetails();
    dialogList();
}

function charactersList() {
    var orderTrnHdrIntno = getOrderTrnHdrIntno();
    var url = "/adaptation/charactersList";
    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { orderTrnHdrIntno: orderTrnHdrIntno },
        success: function (result) {
            $("#charactersList").empty();
            $("#charactersList").html(result);
        }
    });
}

// scene functions
function sceneOnClick() {
    if (!$(this.event.target).hasClass("active")) {
        //$(".scene").removeClass("active");
        //$(this).addClass("active");
        var sceneNo = $(this.event.target).data("sceneno");
        setActiveScene(sceneNo);
    }
}

function addScene() {
    var orderTrnHdrIntno = getOrderTrnHdrIntno();
    var url = "/adaptation/addScene";
    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { orderTrnHdrIntno: orderTrnHdrIntno },
        success: function (result) {
            $("#scenesListContainer").empty();
            $("#scenesListContainer").html(result);            
        }
    });
}

function sceneDetails() {
    var sceneNo = getActiveScene();
    var elem = $(".scene[data-sceneno='" + sceneNo + "']");
    if (elem.hasClass("active")) {
        var sceneIntno = elem.data("sceneintno");        
        var url = "/adaptation/sceneDetails";

        $.ajax({
            contentType: 'application/json',
            method: 'GET',
            url: url,
            data: { sceneIntno: sceneIntno },
            success: function (result) {
                $("#sceneDetailsContainer").empty();
                $("#sceneDetailsContainer").html(result);
            }
        });
    }
}

function saveSceneTimeCode() {
    var sceneIntno = $("#sceneStartTimeCode").data("sceneintno");
    var sceneStartTimeCode = $("#sceneStartTimeCode").val();
    var sceneEndTimeCode = $("#sceneEndTimeCode").val();
    var url = "/adaptation/saveSceneTimeCode";

    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { sceneIntno: sceneIntno, startTimeCode: sceneStartTimeCode, endTimeCode: sceneEndTimeCode },
        success: function (result) {
            $("#sceneDetailsContainer").empty();
            $("#sceneDetailsContainer").html(result);
            $(".sceneDetailsPanel").removeClass("panel-primary");
            $(".sceneDetailsPanel").addClass("panel-success");
        }
    });
}

function onSceneTimeCodeKeyDown() {
    if ($(".sceneDetailsPanel").hasClass("panel-primary")) {
        $(".sceneDetailsPanel").addClass("panel-warning");
        $(".sceneDetailsPanel").removeClass("panel-primary");
    }

    if (this.event.which == 27) {
        sceneDetails();
        if ($(".sceneDetailsPanel").hasClass("panel-warning")) {
            $(".sceneDetailsPanel").addClass("panel-primary");
            $(".sceneDetailsPanel").removeClass("panel-warning");
        }
    }
    else if (this.event.which == 13) {
        saveSceneTimeCode();         
    }
}

function deleteScene() {
    var orderTrnHdrIntno = getOrderTrnHdrIntno();
    var url = "/adaptation/deleteScene";
    var sceneNo = getActiveScene();

    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { orderTrnHdrIntno: orderTrnHdrIntno, sceneNo: sceneNo },
        success: function (result) {
            $("#scenesListContainer").empty();
            $("#scenesListContainer").html(result);            
        }
    });
}

// dialog functions
function dialogList() {
    var sceneIntno = getActiveSceneIntno();
    var url = "/adaptation/dialogList";
    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { sceneIntno: sceneIntno },
        success: function (result) {
            $("#dialogListContainer").empty();
            $("#dialogListContainer").html(result);
            fixTags();
            fixTimes();
        }
    });
}

function addDialog() {
    var sceneIntno = getActiveSceneIntno();
    var url = "/adaptation/addDialog";
    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { sceneIntno: sceneIntno },
        success: function (result) {
            $("#dialogListContainer").empty();
            $("#dialogListContainer").html(result);
            fixTags();
            fixTimes();
        }
    });
}

function deleteDialog(dialogIntno) {
    var sceneIntno = getActiveSceneIntno();
    // var dialogIntno = $(this).data("dialogintno");
    var url = "/adaptation/deleteDialog";
    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { dialogIntno: dialogIntno },
        success: function (result) {
            $("#dialogListContainer").empty();
            $("#dialogListContainer").html(result);
            fixTags();
            fixTimes();
        }
    });
}

function saveDialogTimeCode(dialogIntno) {
    
    var dialogStartTimeCode = $("#dialogStartTimeCode_" + dialogIntno).val();
    var dialogEndTimeCode = $("#dialogEndTimeCode_" + dialogIntno).val();
    var url = "/adaptation/saveDialogTimeCode";

    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { dialogIntno: dialogIntno, startTimeCode: dialogStartTimeCode, endTimeCode: dialogEndTimeCode },
        success: function (result) {
            $("#dialogContainer_" + dialogIntno).empty();
            $("#dialogContainer_" + dialogIntno).html(result);
            fixTimes();
        }
    });
}

function onDialogTimeCodeKeyDown() {
    var dialogIntno = $(this.event.target).data("dialog");
    $("#dialogPanel_" + dialogIntno).removeClass("panel-default");
    $("#dialogPanel_" + dialogIntno).removeClass("panel-success");
    $("#dialogPanel_" + dialogIntno).addClass("panel-warning");
    if (this.event.which == 27) {
        var oldValue = $(this.event.target).data("original");
        $(this.event.target).val(oldValue);
        $("#dialogPanel_" + dialogIntno).removeClass("panel-warning");
        $("#dialogPanel_" + dialogIntno).removeClass("panel-success");
        $("#dialogPanel_" + dialogIntno).addClass("panel-default");
    }
    else if (this.event.which == 13) {
        
        saveDialogTimeCode(dialogIntno);
        $("#dialogPanel_" + dialogIntno).removeClass("panel-default");
        $("#dialogPanel_" + dialogIntno).removeClass("panel-warning");
        $("#dialogPanel_" + dialogIntno).addClass("panel-success");
    }
}

// subtitle functions
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
            fixTags();
            fixTimes();
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
            fixTags();
            fixTimes();
        }
    });
}

function editCharacter(subtitleIntno) {
    var oldName = $("#subtitle_" + subtitleIntno).text();
    $("#characterContainer_" + subtitleIntno).empty();
    var url = "/adaptation/editCharacter";

    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { subtitleIntno: subtitleIntno },
        success: function (result) {
            $("#characterContainer_" + subtitleIntno).empty();
            $("#characterContainer_" + subtitleIntno).html(result);
        }
    });
}

function onCharNameKeyDown() {
    if (this.event.which == 27) {
        var oldName = $(this.event.target).data("original");
        var subtitleIntno = $(this.event.target).data("id");
        var html = '<a id="subtitle_' + subtitleIntno + '" data-subtitle="' + subtitleIntno + '" href="javascript:editCharacter(' + subtitleIntno + ');">' + oldName + '</a>';
        $("#characterContainer_" + subtitleIntno).empty();
        $("#characterContainer_" + subtitleIntno).html(html);
    }
    else if (this.event.which == 13) {
        var newName = $(this.event.target).val();
        var subtitleIntno = $(this.event.target).data("id");
        saveCharacter(subtitleIntno, newName);
    }
}

function saveCharacter(subtitleIntno, newName) {
    var url = "/adaptation/saveCharacter";

    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { subtitleIntno: subtitleIntno, newName: newName },
        success: function (result) {
            $("#characterContainer_" + subtitleIntno).empty();
            var html = '<a id="subtitle_' + subtitleIntno + '" data-subtitle="' + subtitleIntno + '" href="javascript:editCharacter(' + subtitleIntno + ');">' + newName + '</a>';
            $("#characterContainer_" + subtitleIntno).html(html);
        }
    });
}

function fixTags() {
    $(".scentence").each(function (index) {
        var s = $(this).text().trim();
        s = addTextTags(s);
        $(this).html(s);
    });
}

function fixTimes() {
    $('.startPicker').datetimepicker({
        format: 'HH:mm:ss',
        viewDate: false

    });

    $('.endPicker').datetimepicker({
        format: 'HH:mm:ss',
        viewDate: false

    });

    $('.startPicker').on('dp.hide', function () {
        saveSubtitleStartTime();
    });

    $('.endPicker').on('dp.hide', function () {
        saveSubtitleEndTime();
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

function onSubtitleInputKeyDown() {
    if (this.event.which == 27) {
        cancelEditSubtitle();
    }
    else if (this.event.which == 13) {
        saveEditSubtitle();
    }
}


function saveSubtitleStartTime() {
    var newTime = $(this.event.target).val();
    var subtitleIntno = $(this.event.target).data("subtitle");
    var url = "/adaptation/saveSubtitleStartTimeCode";


    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { subtitleIntno: subtitleIntno, startTime: newTime },
        success: function (result) {

        }
    });
}

function saveSubtitleEndTime() {
    var newTime = $(this.event.target).val();
    var subtitleIntno = $(this.event.target).data("subtitle");
    var url = "/adaptation/saveSubtitleEndTimeCode";


    $.ajax({
        contentType: 'application/json',
        method: 'GET',
        url: url,
        data: { subtitleIntno: subtitleIntno, endTime: newTime },
        success: function (result) {

        }
    });
}
/**************************************************************************************************************/
/*

// TEMPORARY: add tags to loaded scentences






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

   
                    $('.startPicker').datetimepicker({
                        format: 'HH:mm:ss',
                        viewDate: false
                        
                    });

                    $('.endPicker').datetimepicker({
                        format: 'HH:mm:ss',
                        viewDate: false

                    });

                    $('.startPicker').on('dp.hide', function () {
                        saveSubtitleStartTime();
                    });

                    $('.endPicker').on('dp.hide', function () {
                        saveSubtitleEndTime();
                    });

                    $(".startDialogTime").keydown(function (e) {
                        if (e.which == 13) {
                            saveDialogStartTime();
                        }
                    });

                    $(".endDialogTime").keydown(function (e) {
                        if (e.which == 13) {
                            saveDialogEndTime();
                        }
                    });

                }
            });

        }

    });

}

*/