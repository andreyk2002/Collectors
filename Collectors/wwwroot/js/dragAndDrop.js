"use strict"

let data = null;

$(document).ready(function () {
    initDragAndDrop();
    dragDropOperation();
})

function initDragAndDrop() {
    $("#uploadBlock").on("dragenter drop dragover", function (evt) {
        evt.preventDefault();
        evt.stopPropagation();
    })
}

function dragDropOperation() {
    $("#uploadBlock").on("drop", function (evt) {
        evt.preventDefault();
        evt.stopPropagation();

        let images = evt.originalEvent.dataTransfer.files;
        let uploadMessage = "";
        //images.forEach(item => {if(item.type)})

        if (images.length > 1) {
            uploadMessage = "You're trying to upload more than one image" +
                "for your collection.<br/> The first image will be chosen by default";
        } else if (images.length > 0) {
            uploadMessage = "Image " + images[0].name;
        }
        $("#uploadBlock").html(uploadMessage);
        data = new FormData();
        data.append(images[0].name, images[0]);

        //$.ajax({
        //    type: "POST",
        //    url: "",
        //    contentType: false,
        //    processData: false,
        //    data: data,
        //    success: function (message) {
        //        $("#uploadBlock").html(message);
        //    },
        //    error: function () {
        //        $("#uploadBlock").html("Error found :");
        //    },
        //    beforeSend: function () {
        //        $("#imageUpload").show();
        //    },
        //    complete: function () {
        //        $("#imageUpload").hide();
        //    }
        //})

    })
}

$("#createButton").on("click", function () {
    $.ajax({
        type: "POST",
        url: "",
        contentType: false,
        processData: false,
        data: data,
        success: function (message) {
            $("#uploadBlock").html(message);
        },
        error: function () {
            $("#uploadBlock").html("Error found :");
        },
        beforeSend: function () {
            $("#imageUpload").show();
        }
    })
})