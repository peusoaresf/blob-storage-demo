const myJs = (function () {
    const getNumberOfChunks = function (self, fileSize) {
        return fileSize < self.BYTES_PER_CHUNK
            ? 1
            : (fileSize % self.BYTES_PER_CHUNK === 0
                ? fileSize / self.BYTES_PER_CHUNK
                : Math.floor(fileSize / self.BYTES_PER_CHUNK) + 1);
    };

    const chunkUploadPromise = function (chunk, chunkOrder, token) {
        return new Promise(function (resolve, reject) {
            var formData = new FormData();
            formData.append("chunk", chunk);
            formData.append("token", chunkOrder + "-" + token);

            return $.ajax({
                url: "Ajax/File/FileChunkUpload.aspx",
                type: "POST",
                data: formData,
                processData: false,
                contentType: false,
                success: function (res) {
                    resolve(res);
                },
                error: function (err) {
                    reject(err);
                }
            });
        });
    };

    const uploadFile = function (self, oFileHelper) {
        return new Promise(function (resolve, reject) {
            let file = oFileHelper.file,
                fileSize = file.size;

            let startReading = 0,
                endReading = self.BYTES_PER_CHUNK,
                numberOfChunks = getNumberOfChunks(self, fileSize),
                chunksCompleted = 0,
                chunkOrder = 1;

            let aPromise = [];

            while (startReading < fileSize) {
                var chunk = file.slice(startReading, endReading);

                var oPromise = chunkUploadPromise(chunk, chunkOrder, oFileHelper.token);

                aPromise.push(oPromise);

                oPromise
                    .then(function (res) {
                        chunksCompleted++;
                        self.onUploadProgress(oFileHelper, chunksCompleted / numberOfChunks);
                    });

                startReading = endReading;
                endReading = startReading + self.BYTES_PER_CHUNK;
                chunkOrder++;
            }

            self.windowReference.Promise.all(aPromise)
                .then(function (res) {
                    resolve(oFileHelper);
                })
                .catch(function (err) {
                    reject(err, oFileHelper);
                });
        });
    };

    const mergeChunksPromise = function (oFileHelper) {
        var formData = new FormData();
        formData.append("fileName", oFileHelper.file.name);
        formData.append("token", oFileHelper.token);

        return $.ajax({
            url: "Ajax/File/FileChunkMerge.aspx",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false
        });
    };

    function FileHelper(oFile) {
        this.file = oFile;
        this.token = oFile.name + (new Date()).getTime();
    };

    function FileUploader(aFile, iBytesPerChunk) {
        this.windowReference = window;
        this.BYTES_PER_CHUNK = iBytesPerChunk || 77570;
        this.files = [];
        for (var i = 0, length = aFile.length; i < length; i++) {
            this.files.push(new FileHelper(aFile[i]));
        }
    };

    FileUploader.prototype.start = function () {
        var self = this;

        for (let i = 0, length = self.files.length; i < length; i++) {
            self.onUploadStarted(self.files[i]);

            uploadFile(self, self.files[i])
                .then(function (oFileHelper) {
                    self.onUploadFinishing(oFileHelper);

                    mergeChunksPromise(oFileHelper)
                        .then(function () {
                            self.onUploadFinished(oFileHelper);
                        })
                        .catch(function (err) {
                            self.onUploadError(oFileHelper, err);
                        });
                })
                .catch(function (err, oFileHelper) {
                    self.onUploadError(oFileHelper, err);
                });
        }
    };

    FileUploader.prototype.onUploadStarted = function (oFileHelper) {
        console.log(oFileHelper.file.name + " upload started...");
    };

    FileUploader.prototype.onUploadProgress = function (oFileHelper, iProgress) {
        console.log(oFileHelper.file.name + " upload progress at " + (iProgress*100) + "%");
    };

    FileUploader.prototype.onUploadFinishing = function (oFileHelper) {
        console.log(oFileHelper.file.name + " upload is finishing...");
    };

    FileUploader.prototype.onUploadFinished = function (oFileHelper) {
        console.log(oFileHelper.file.name + " upload finished!");
    };

    FileUploader.prototype.onUploadError = function (oFileHelper, error) {
        console.log(oFileHelper.file.name + " upload error!");
        console.log(error);
    };

    return {
        FileUploader: FileUploader
    };
})();