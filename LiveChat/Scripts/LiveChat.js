function EmbedChat(CentralChatHub) {
    $("[data-toggle=popover]").popover({ html: true, container: "#LiveChatWrapper" });
    $("#StartChat").click(function () {
        // Reference the auto-generated proxy for the hub.
        $.connection.hub.url = CentralChatHub + '/signalr';
        var chat = $.connection.chatHub;
        // Create a function that the hub can call back to display messages.
        //RECEIVE
        chat.client.addNewMessageToPage = function (name, group, message) {
            // Add the message to the page.
            $('#discussion').append('<dt><strong>' + htmlEncode(name)
                + '</strong>: ' + htmlEncode(message) + '</dt>');
        };

        // Set initial focus to message input box.
        $('#message').focus();
        // Start the connection.
        $.connection.hub.start().done(function () {
            $('#sendmessage').click(function () {
                // Call the Send method on the hub.
                //SEND
                chat.server.send($('#message').val());
                // Clear text box and reset focus for next comment.
                $('#message').val('').focus();
            });

            $("#StartChat").click(function () {
                $.connection.hub.stop();
            });
        });
    });
}

// This optional function html-encodes messages for display in the page.
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}