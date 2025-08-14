

'use strict';

document.addEventListener('DOMContentLoaded', function (e) {
    (function () {
        const deactivateAcc = document.querySelector('#formAccountDeactivation');

        // Update/reset user image of account page
        let accountUserImage = document.getElementById('UploadedImage');
        const fileInput = document.querySelector('.account-file-input');

        if (accountUserImage) {
            const resetImage = accountUserImage.src;
            fileInput.onchange = () => {
                const file = fileInput.files[0];
                if (!file) return;

                const fileType = file.type; // betulkan property
                if (fileType === "application/pdf") {
                    const fileReader = new FileReader();
                    fileReader.onload = function () {
                        const typedarray = new Uint8Array(this.result);
                        pdfjsLib.getDocument(typedarray).promise.then(function (pdf) {
                            pdf.getPage(1).then(function (page) {
                                const scale = 1.5;
                                const viewport = page.getViewport({ scale: scale });

                                const canvas = document.createElement('canvas');
                                const context = canvas.getContext('2d');
                                canvas.height = viewport.height;
                                canvas.width = viewport.width;

                                page.render({ canvasContext: context, viewport: viewport }).promise.then(function () {
                                    accountUserImage.src = canvas.toDataURL();
                                });
                            });
                        });
                    };
                    fileReader.readAsArrayBuffer(file);
                } else {
                    accountUserImage.src = URL.createObjectURL(file);
                }

            };
        }
    })();
});