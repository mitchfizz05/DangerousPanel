
// Websocket instance
var ws;

// Have we been authenticated yet?
var authenticated = false;

var touchscreen = false;

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
        } else if (event.data.substring(0, "meta:".length) == "meta:") {
            // Received metadata
            var meta = JSON.parse(event.data.substring("meta:".length));
            $("#cmdr-name").html("CMDR " + meta.CmdrName);
        }
    });
    ws.addEventListener("error", function (e) {
        console.warn("Websocket error!");
        onDone(false);
    });
    ws.addEventListener("close", function (e) {
        console.log("Connection closed");
        onDone(false);
    })
}

function sendKey(key) {
    ws.send("key:" + key);
}

function fetchPanelMetadata() {
    // Send request for metadata
    ws.send("fetchmeta"); // plz

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
                fetchPanelMetadata(); // Fetch any metadata
                console.log("Connected!");
                $("#connecting-overlay").fadeOut(100);
            } else {
                $("#connecting-overlay").show();
                $("#connecting-overlay .connect-wrapper").show();
                $("#connecting-overlay .loader").hide();
            }
        })
    });

    function btnPressHandler(btn) {
        btn.addClass("active");

        var key = btn.attr("data-key");
        sendKey(key);
    }

    function btnReleaseHandler(btn) {
        btn.removeClass("active");
    }

    $(".edbutton").each(function() {
        $(this)[0].addEventListener("touchstart", function () { touchscreen = true; btnPressHandler($(this)); });
        $(this)[0].addEventListener("touchend", function () { touchscreen = true; btnReleaseHandler($(this)); });
    });
    $(".edbutton").mousedown(function () {
        if (!touchscreen) { btnPressHandler($(this)); }
    }).mouseup(function () {
        if (!touchscreen) { btnReleaseHandler($(this)); }
    });

    $("#connect-ip").keypress(function (e) {
        if (e.keyCode == 13) {
            $("#connect-btn").click();
        }
    });

    // Every 10 seconds ask server for metadata
    setInterval(function () {
        if ((ws !== null) && (ws.readyState == ws.OPEN)) {
            fetchPanelMetadata();
        }
    }, 10000);
});
