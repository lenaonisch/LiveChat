window.isPopoverOpen=false;

var f=function(e){
	if(window.isPopoverOpen)
		f2(e);
	else
		f1(e);
}

var f2=function(e){
	e.preventDefault();
	console.log("Stop connection");
	$.connection.hub.stop();
	window.isPopoverOpen=false;
}

var f1=function(e){
	e.preventDefault();
	$('#StartChat').prop('disabled', true);
	$.support.cors = true;
    // Reference the auto-generated proxy for the hub.
    $.connection.hub.url = CentralChatHub + '/signalr';
    var chat = $.connection.chatHub;
	$.connection.hub.qs = "company=Default";
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
		console.log("Start connection");
        $('#sendmessage').click(function () {
            // Call the Send method on the hub.
            //SEND
            chat.server.send($('#message').val());
            // Clear text box and reset focus for next comment.
            $('#message').val('').focus();
        });
		window.isPopoverOpen=true;
		$('#StartChat').prop('disabled', false);
    });
}


function EmbedChat(CentralChatHub) {
    $("[data-toggle=popover]").popover({ html: true, container: "#LiveChatWrapper" });
    $("#StartChat").on("click",f);
}

// This optional function html-encodes messages for display in the page.
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}