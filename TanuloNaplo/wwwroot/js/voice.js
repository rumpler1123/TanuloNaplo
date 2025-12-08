window.startDictation = (dotNetHelper) => {
    // Megnézzük, hogy a böngésző tud-e ilyet
    if (!('webkitSpeechRecognition' in window) && !('SpeechRecognition' in window)) {
        alert("Ez a böngésző nem támogatja a beszédfelismerést. Próbáld Chrome-ban!");
        return;
    }

    var SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
    var recognition = new SpeechRecognition();

    recognition.lang = "hu-HU"; // Magyarul figyeljen
    recognition.continuous = false; // Ha csend van, álljon le
    recognition.interimResults = false;

    // Amikor elindul a felvétel
    recognition.onstart = function () {
        console.log("Mikrofon bekapcsolva...");
    };

    // Amikor szöveget hallott
    recognition.onresult = function (event) {
        var transcript = event.results[0][0].transcript;
        // Visszaküldjük a szöveget a C# oldalnak (Blazornak)
        dotNetHelper.invokeMethodAsync('ReceiveDictation', transcript);
    };

    // Ha hiba van
    recognition.onerror = function (event) {
        console.error("Hiba: " + event.error);
        dotNetHelper.invokeMethodAsync('StopRecordingUI');
    };

    // Ha vége
    recognition.onend = function () {
        dotNetHelper.invokeMethodAsync('StopRecordingUI');
    };

    recognition.start();
};