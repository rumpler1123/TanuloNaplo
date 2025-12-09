window.startDictation = (dotNetHelper) => {
    // böngésző támogatás ellenőrzése
    if (!('webkitSpeechRecognition' in window) && !('SpeechRecognition' in window)) {
        alert("Ez a böngésző nem támogatja a beszédfelismerést. Próbáld Chrome-ban!");
        return;
    }

    var SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
    var recognition = new SpeechRecognition();

    recognition.lang = "hu-HU"; // magyar nyelv
    recognition.continuous = false; // ha néma hallgat
    recognition.interimResults = false;

    // felvétel
    recognition.onstart = function () {
        console.log("Mikrofon bekapcsolva...");
    };

    // szöveget hallott
    recognition.onresult = function (event) {
        var transcript = event.results[0][0].transcript;
        dotNetHelper.invokeMethodAsync('ReceiveDictation', transcript);
    };

    //  hiba van
    recognition.onerror = function (event) {
        console.error("Hiba: " + event.error);
        dotNetHelper.invokeMethodAsync('StopRecordingUI');
    };

    // vége
    recognition.onend = function () {
        dotNetHelper.invokeMethodAsync('StopRecordingUI');
    };

    recognition.start();
};