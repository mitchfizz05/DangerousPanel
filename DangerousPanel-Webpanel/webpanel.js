
// Websocket instance
var ws;

// Have we been authenticated yet?
var authenticated = false;

function connect(ip, port, onTokenRequired, onDone) {
    console.log("Connecting to " + ip + ":" + port);

    ws = new WebSocket("ws://" + ip + ":" + port + "/panel", "edpanel");
    ws.addEventListener("open", function () {
        console.log("Websocket connection to server open.");
    });
    ws.addEventListener("message", function (event) {
        console.log("Received message from server: " + event.data);

        if (event.data == "authenticate") {
            // Authenticate request
            var token = onTokenRequired(false);
            if (token == null) {
                ws.close();
                onDone(true);
                return;
            }
            ws.send(token);
            console.log("Sent token \"" + token + "\"!");
        } else if (event.data == "authenticated") {
            // Authenticated message
            authenticated = true;
        } else if (event.data == "authfail") {
            // Authentication failed
            var token = onTokenRequired(true);
            if (token == null) {
                ws.close();
                onDone(true);
                return;
            }
            ws.send(token);
            console.log("Sent token \"" + token + "\"!");
        }
    })
}

$(document).ready(function () {
    // Connect
    connect("192.168.0.8", 7751, function (failed) {
        // Token required
        if (failed) {
            return prompt("Incorrect access token. Please try again.");
        }
        else {
            return prompt("Please enter the access token displayed on the server console.");
        }
    }, function (success) {
        // Finished connecting
        if (success) {
            authenticated = true;
            console.log("Connected!");
        } else {
            console.warn("Connection failed!");
            alert("Connection failed.");
        }
    });
});

$(".edbutton").click(function () {
    var key = $(this).attr("data-key");

});
