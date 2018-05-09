window.isPopoverOpen = false;
window.strBlockedConnection = "No operators online";
window.strLiveChat = "LiveChat ";
var f = function (e) {
    if (window.isPopoverOpen)
        popoverClose(e);
    else
        popoverOpen(e);
}

var popoverClose = function (e) {
    e.preventDefault();
    console.log("Stop connection");
    $.connection.hub.stop();
    window.isPopoverOpen = false;
}

var popoverOpen = function (e) {
    e.preventDefault();
    $('#StartChat').prop('disabled', true);
    $.support.cors = true;
    // Reference the auto-generated proxy for the hub.
    $.connection.hub.url = CentralChatHub + '/signalr';
    var chat = $.connection.chatHub;
    $.connection.hub.qs = "companyID=" + window.CompanyID;
    console.log("companyID=" + $.connection.hub.qs);
    // Create a function that the hub can call back to display messages.
    //RECEIVE
    chat.client.addNewMessageToPage = function (name, group, message) {
        // Add the message to the page.
        $('#discussion').append('<dt><strong>' + htmlEncode(name)
            + '</strong>: ' + htmlEncode(message) + '</dt>');
    };

    chat.client.blockUser = function () {
        console.log("user blocked");
        $('#sendmessage').prop('disabled', true);
        $("#lblChatDisabled").html(window.strBlockedConnection);
    }

    chat.client.unblockUser = function () {
        console.log("user unblocked");
        $('#sendmessage').prop('disabled', false);
        $("#lblChatDisabled").html("<br/>");
    }

    // Set initial focus to message input box.
    $('#message').focus();


    // Start the connection.
    $.connection.hub.start().done(function () {
        console.log("Start connection");
        $('#sendmessage').click(function () {
            // Call the Send method on the hub.
            //SEND
            chat.server.send($('#message').val());
            // Clear text box and reset focus for next comment.
            $('#message').val('').focus();
        });
        window.isPopoverOpen = true;
        $('#StartChat').prop('disabled', false);
    });
}


function EmbedChat(CentralChatHub) {
    $("[data-toggle=popover]").popover({ html: true, container: "#LiveChatWrapper" });
    $("#StartChat").on("click", f);
}

// This optional function html-encodes messages for display in the page.
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}

$(function () {
    $($.parseHTML(
        "<div id='LiveChatWrapper' class='card text-white bg-dark'>" +
        "<button class='btn btn-lg btn-dark'" +
        "        style='position:absolute; bottom:15px;'" +
        "        data-placement='top'" +
        "        id='StartChat'" +
        "        data-toggle=popover" +
        "        title='" + window.strLiveChat+"'" +
        "        data-content= \"" +
        "            <label id='lblChatDisabled'><br/></label>" +
        "            <input type='text' id='message' />" +
        "            <input type='button' id='sendmessage' value='Send' disabled/>" +
        "            <dl id='discussion' style='height:200px; overflow-y: scroll'></dl>" +
        "        \">Chat!" +
        "    </button>" +
        "    </div>"
    )).appendTo("." + window.ChatContainerName);
    EmbedChat(window.CentralChatHub);
    //$('#sendmessage').prop('disabled', true);
});