/*
Generic functions called as part of Signal-R messaging service
*/

function StartMessaging() {
    var progressNotifier = $.connection.progressHub;

    $.connection.hub.start().done(function () {
        //DisplayMessage("Ready. Click on 'Start Download' below to find data  ...");
    });

    // client-side message functions that will be pushed from the server-side
    progressNotifier.client.initProgress = function (message) {
        InitProgress(message, 2);
    };

    progressNotifier.client.sendMessage = function (message) {
        DisplayMessage(message);
    };

    progressNotifier.client.updateProgress = function (message, count) {
        UpdateProgress(message, count);
    };

    progressNotifier.client.clearProgress = function (message) {
        FinaliseProgress();
    };

    progressNotifier.client.sendCurrentValue = function (currentValue) {
        DoCurrentValue(currentValue);
    }

    progressNotifier.client.stopClient = function (message, count, id) {
        StopMessaging(message, count, id);
    }
};