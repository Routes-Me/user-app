var scanner = new Instascan.Scanner(
    {
        video: document.getElementById('preview')
    }
);
scanner.addListener('scan', function (content) {
    window.open(content, "_blank");
});
Instascan.Camera.getCameras().then(cameras => {
    debugger;
    if (cameras.length > 0) {
        scanner.start(cameras[1]);
    } else {
        debugger;
        $('.qr-scan-message').find('h6').text("No camera found!");
    }
});