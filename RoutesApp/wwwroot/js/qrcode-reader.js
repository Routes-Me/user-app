var cameraId;
Html5Qrcode.getCameras().then(cameras => {
    debugger;
    if (devices && devices.length > 0) {
        cameraId = devices[1].id;
    }
    else {
        $('.qr-scan-message').find('h6').text("No camera found!");
        console.log("No camera found!")
    }
}).catch(err => {
})

var html5QrcodeScanner = new Html5QrcodeScanner(
    "reader", { fps: 10, qrbox: 250 });
html5QrcodeScanner.render(onScanSuccess);

function onScanSuccess(qrCodeMessage) {
    debugger;
    // handle on success condition with the decoded message
    window.open(qrCodeMessage, "_self")
    html5QrcodeScanner.clear();
    // ^ this will stop the scanner (video feed) and clear the scan area.
}

function onScanError(errorMessage) {
    // handle on error condition, with error message
}