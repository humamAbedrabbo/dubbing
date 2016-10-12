
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
