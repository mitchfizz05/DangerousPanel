
// Websocket instance
var ws;

// Have we been authenticated yet?
var authenticated = false;

function connect(ip, port, onTokenRequired, onDone) {
    console.log("Connecting to " + ip + ":" + port);

    ws = new WebSocket("ws://" + ip + ":" + port + "/panel");
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
                onDone(false);
                return;
            }
            ws.send(token);
            console.log("Sent token \"" + token + "\"!");
        } else if (event.data == "authenticated") {
            // Authenticated message
            authenticated = true;
            onDone(true);
        } else if (event.data == "authfail") {
            // Authentication failed
            var token = onTokenRequired(true);
            if (token == null) {
                ws.close();
                onDone(false);
                return;
            }
            ws.send(token);
            console.log("Sent token \"" + token + "\"!");
        }
    })
}

function sendKey(key) {
    ws.send("key:" + key);
}

$(document).ready(function () {
    $("#connect-btn").click(function () {
        $("#connecting-overlay .connect-wrapper").hide();
        $("#connecting-overlay .loader").show();

        var ip = $("#connect-ip").val().split(":")[0];
        var port = $("#connect-ip").val().split(":")[1] || 7751;

        connect(ip, port, function (failed) {
            // Token required
            var message = "Please enter the access token displayed on the server console.";
            if (failed) {
                message = "Incorrect access token. Please try again.";
            }

            var result = prompt(message);

            if ( (result == null) || (result == "") ) {
                // Abort
                return null;
            } else {
                return result;
            }
        }, function (success) {
            // Finished connecting
            if (success) {
                authenticated = true;
                console.log("Connected!");
                $("#connecting-overlay").fadeOut(100);
            } else {
                console.warn("Connection failed!");
                alert("Connection failed.");
                $("#connecting-overlay").show();
                $("#connecting-overlay .connect-wrapper").show();
                $("#connecting-overlay .loader").hide();
            }
        })
    });

    $(".edbutton").click(function () {
        var key = $(this).attr("data-key");
        sendKey(key);
    });
});
