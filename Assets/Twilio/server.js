const express = require('express');
const wss = require('websocket').server;
const http = require('http');
const bodyParser = require('body-parser');
const VoiceResponse = require('twilio').twiml.VoiceResponse;

const app = express();
app.use(bodyParser.urlencoded({ extended: false }))

var hostname = 'http://7c77eb3f.ngrok.io/';

var currentConnection;
var callSid;

const accountSid = 'ACb76d16c8bc2b004e5dddcd938787c35d';
const authToken = '30d49e1ab201595bb083590e8c00ab0b';
const client = require('twilio')(accountSid, authToken);


var server = http.createServer(function(request, response) {
    // process HTTP request. Since we're writing just WebSockets
    // server we don't have to implement anything.
    console.log('Websocket server started on port 1337');
});
server.listen(1337, function() { });

// create the server
wsServer = new wss({
    httpServer: server
});

function getDTMF (inputLength) {
    console.log('Starting DTMF');
    client.calls(callSid).update({twiml: '<Response><Gather action="' + hostname + '/input" input="dtmf" timeout="10" numDigits="' + inputLength + '"></Gather></Response>'});
}

function endCall () {
    console.log('Ending call');
    client.calls(callSid).update({status: 'completed'});
}


// WebSocket server
wsServer.on('request', function(request) {
    var connection = request.accept(null, request.origin);
    currentConnection = connection;
    console.log('New websocket client connected');

    // This is the most important callback for us, we'll handle
    // all messages from users here.
    connection.on('message', function(message) {
        if (message.type === 'utf8') {
            // process WebSocket message
            console.log('Received websocket message: ' + message.utf8Data);

            var data = message.utf8Data.split(',');
            if (data[0] == "get-input") {
                getDTMF(data[1]);
            } else if (data[0]== "end-call") {
                endCall();
            }
        }
    });

    connection.on('close', function(connection) {
        // close user connection
        console.log('Websocket client disconnected')
    });
});


app.post('/input', (request, response) => {
    console.log('Received DTMF input: ' + request.body['Digits']);

    if (currentConnection != null && currentConnection.connected) {
        currentConnection.sendUTF(request.body['Digits']);
        console.log('Forwarded to Unity.');
    } else {
        console.log('Unity not connected.');
    }

    const twiml = new VoiceResponse();
    twiml.redirect({ method: 'POST' }, hostname + "/incoming");

    // Render the response as XML in reply to the webhook request
    response.type('text/xml');
    response.send(twiml.toString());
})

// Create a route that will handle Twilio webhook requests, sent as an
// HTTP POST to /incoming in our application
app.post('/incoming', (request, response) => {
    console.log('Twilio request recieved');
    callSid = request.body['CallSid'];

    console.log('Call to ' + request.body['To'] + ' from ' + request.body['From'] + ', ' + request.body['CallStatus']);

    if (currentConnection != null && currentConnection.connected) {
        currentConnection.sendUTF(request.body['CallStatus'], + ',' + request.body['To'] + ',' + request.body['From']);
        console.log('Forwarded to Unity.');
    } else {
        console.log('Unity not connected.');
    }

    // Use the Twilio Node.js SDK to build an XML response
    const twiml = new VoiceResponse();
    twiml.play({}, "https://s3-eu-west-1.amazonaws.com/hacksheffield5.0/rickroll.mp3");

    // Render the response as XML in reply to the webhook request
    response.type('text/xml');
    response.send(twiml.toString());
});

app.post('/statuschange', (request, response) => {
    console.log('Twilio request recieved');
    console.log(request.body['CallStatus']);

    if (currentConnection != null && currentConnection.connected) {
        currentConnection.sendUTF(request.body['CallStatus']);
        console.log('Forwarded to Unity');
    } else {
        console.log('Unity not connected');
    }

    // Render the response as XML in reply to the webhook request
    response.type('text/xml');
    response.send();
});

// Create an HTTP server and listen for requests on port 3000
app.listen(3000, () => {
    console.log('Now listening for Twilio on port 3000.');
});