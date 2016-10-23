
// Template functions

// Tag functions
function dispTags() {
    var typeId = $("#tag_type_selector > option")[tag_type_selector.selectedIndex].value;
    var url = '/tags/tagList';
    $.ajax({
        contentType: 'text/html',
        method: 'GET',
        url: url,
        data: { typeId: typeId },
        success: function (result) {
            $("#tag_list").empty();
            $("#tag_list").html(result);
        }
    });
}
function editTag(tagId) {
    $(".tagLabel[data-id='" + tagId + "']").toggleClass('hidden');
    $(".tag[data-id='" + tagId + "']").toggleClass('hidden');
    $("#saveTag[data-id='" + tagId + "']").toggleClass('hidden');
}
function saveTag(tagId) {
    var tagName = $(".tag[data-id='" + tagId + "']").val();
    var url = '/tags/saveTag';
    $.ajax({
        contentType: 'text/html',
        method: 'GET',
        url: url,
        data: { tagId: tagId, name: tagName },
        success: function (result) {
            $("#saveTag[data-id='" + tagId + "']").toggleClass('hidden');
            $(".tagLabel[data-id='" + tagId + "']").text(tagName);
            $(".tagLabel[data-id='" + tagId + "']").toggleClass('hidden');
            $(".tag[data-id='" + tagId + "']").toggleClass('hidden');
        }
    });
}
function deleteTag(tagId) {
    var tagName = $(".tag[data-id='" + tagId + "']").val();
    if (!confirm('Are you sure you want to delete [' + tagName + "]?"))
        return

    var typeId = $("#tag_type_selector > option")[tag_type_selector.selectedIndex].value;
    var url = '/tags/deleteTag';
    $.ajax({
        contentType: 'text/html',
        method: 'GET',
        url: url,
        data: { tagId: tagId, typeId: typeId },
        success: function (result) {
            $("#tag_list").empty();
            $("#tag_list").html(result);
        }
    });
}
function saveNewTag() {
    var tagName = $("#newTagName").val();
    var typeId = $("#tag_type_selector > option")[tag_type_selector.selectedIndex].value;
    if (typeId == 0)
    {
        alert("You must select a tag type");
        return;
    }
    var url = '/tags/saveNewTag';
    $.ajax({
        contentType: 'text/html',
        method: 'GET',
        url: url,
        data: { name: tagName, typeId: typeId },
        success: function (result) {
            $("#tag_list").empty();
            $("#tag_list").html(result);
        }
    });
}
// Tag Templates functions
function editTemplate(tagTemplateHdrIntno) {
    var url = '/templates/editTemplate';
    $.ajax({
        contentType: 'text/html',
        method: 'GET',
        url: url,
        data: { id: tagTemplateHdrIntno },
        success: function (result) {
            $("#editTemplateContainer").empty();
            $("#editTemplateContainer").html(result);

        }
    });

}
function saveTemplate() {
    var tagTemplateHdrIntno = $("#tagTemplateHdrIntno").val();
    var tempTitle = $("#tempTitle").val();
    var tempDescription = $("#tempDesc").val();
    var tempText = $("#tempText").val();
    var url = '/templates/saveTemplate';
    $.ajax({
        contentType: 'text/html',
        method: 'GET',
        url: url,
        data: { tagTemplateHdrIntno: tagTemplateHdrIntno, title: tempTitle, desc: tempDescription, text: tempText },
        success: function (result) {
            $("#templateList").empty();
            $("#templateList").html(result);

        }
    });
}
function saveNewTemplate() {
    var tagTemplateHdrIntno = 0;
    var tempTitle = $("#ntempTitle").val();
    var tempDescription = $("#ntempDesc").val();
    var tempText = $("#ntempText").val();
    var url = '/templates/saveTemplate';
    $.ajax({
        contentType: 'text/html',
        method: 'GET',
        url: url,
        data: { tagTemplateHdrIntno: tagTemplateHdrIntno, title: tempTitle, desc: tempDescription, text: tempText },
        success: function (result) {
            $("#templateList").empty();
            $("#templateList").html(result);

        }
    });
}
function deleteTemplate(id) {
    var url = '/templates/deleteTemplate';
    $.ajax({
        contentType: 'text/html',
        method: 'GET',
        url: url,
        data: { id: id },
        success: function (result) {
            $("#templateList").empty();
            $("#templateList").html(result);

        }
    });
}
function addtemplateDtl(id) {
    var tagName = $("#newDtlTagName").val();
    var tagMinScore = $("#newDtlMinScore").val();
    var tagWeight = $("#newDtlWeight").val();
    var url = '/templates/addDetail';
    $.ajax({
        contentType: 'text/html',
        method: 'GET',
        url: url,
        data: { id: id, tagName: tagName, minScore: tagMinScore, weight: tagWeight },
        success: function (result) {
            $("#templateDtlList").empty();
            $("#templateDtlList").html(result);

        }
    });
}
function saveDetail(id) {
    var tagName = $("#dtlTagName_" + id).val();
    var tagMinScore = $("#dtlMinScore_" + id).val();
    var tagWeight = $("#dtlWeight_" + id).val();
    var url = '/templates/saveDetail';
    $.ajax({
        contentType: 'text/html',
        method: 'GET',
        url: url,
        data: { id: id, tagName: tagName, minScore: tagMinScore, weight: tagWeight },
        success: function (result) {
            $("#templateDtlList").empty();
            $("#templateDtlList").html(result);

        }
    });
}
function deleteDetail(id) {
    var url = '/templates/deleteDetail';
    $.ajax({
        contentType: 'text/html',
        method: 'GET',
        url: url,
        data: { id: id },
        success: function (result) {
            $("#templateDtlList").empty();
            $("#templateDtlList").html(result);

        }
    });
}