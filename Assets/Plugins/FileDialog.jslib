var plugin = {

    RequestOpenDialogWeb: function () {

        var inputFile = document.createElement("input");
        inputFile.setAttribute("type", "file");
        inputFile.multiple = true;
        inputFile.accept = "image/jpeg, image/png";
        inputFile.onchange = function () {

            console.log(this.files);

            var listQrCodes = [];

            for (i = 0; i < this.files.length; i++) {
                listQrCodes.push(URL.createObjectURL(this.files[i]));
              }


            var jsonData = JSON.stringify(listQrCodes);
            console.log("Enviar JSON Data: " + jsonData);

            SendMessage('QRReading', 'LoadedFilesWeb',  jsonData );
        }

        inputFile.click();
    },
}

mergeInto(LibraryManager.library, plugin);