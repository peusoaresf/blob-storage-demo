using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace WebUI.Classes
{
    public class FileManager
    {
        private async Task<CloudBlobContainer> GetCloudBlobContainer()
        {
            CloudBlobClient blobClient = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("ged");

            await container.CreateIfNotExistsAsync();

            return container;
        }

        private IEnumerable<string> GetChunksBlobs(CloudBlobContainer container, string fileToken)
        {
            CloudBlobDirectory blobDirectory = container.GetDirectoryReference($"tmp{fileToken}");

            var blobs = new List<string>();

            foreach (var item in blobDirectory.ListBlobs(useFlatBlobListing: true))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob) item;
                    if (blob.Name.Contains(fileToken))
                    {
                        blobs.Add(blob.Name);
                    }
                }
            }

            return blobs;
        }

        private IEnumerable<string> OrdenarChunksBlobs(IEnumerable<string> chunksBlobs)
        {
            int maxLength = chunksBlobs.Max(x => x.Length);

            return chunksBlobs.OrderBy(x => x.PadLeft(maxLength, '0'));
        }










        //private static string _baseUrl = "C:\\dev\\ged_repo\\";
        private static string _baseUrl = "C:\\GED_local\\";

        public async Task<long> MergeChunks(string fileUrl, string fileToken)
        {
            // BLOB STORAGE

            CloudBlobContainer container = await GetCloudBlobContainer();

            IEnumerable<string> chunksBlobs = OrdenarChunksBlobs(GetChunksBlobs(container, fileToken));

            CloudBlockBlob novoBlob = container.GetBlockBlobReference(fileUrl);
            
            using (Stream novoBlobStream = await novoBlob.OpenWriteAsync())
            {
                foreach (var chunk in chunksBlobs)
                {
                    CloudBlockBlob chunkBlob = container.GetBlockBlobReference(chunk);

                    using (Stream chunkStream = await chunkBlob.OpenReadAsync())
                    {
                        byte[] chunkStreamContent = new byte[chunkStream.Length];

                        chunkStream.Read(chunkStreamContent, 0, (int) chunkStream.Length);
                        novoBlobStream.Write(chunkStreamContent, 0, (int) chunkStream.Length);
                    }
                }
            }

            long fileSize = (await novoBlob.OpenReadAsync()).Length;

            foreach (var chunk in chunksBlobs)
            {
                CloudBlockBlob chunkBlob = container.GetBlockBlobReference(chunk);

                await chunkBlob.DeleteIfExistsAsync();
            }

            return fileSize;

            /*
             * FILE SYSTEM
             * 
             * await Sleep();

            IEnumerable<string> chunks = Directory.GetFiles(_baseUrl + $"tmp{fileToken}").Where(x => x.Contains(fileToken));

            int maxLength = chunks.Max(x => x.Length);

            chunks = chunks.OrderBy(x => x.PadLeft(maxLength, '0'));

            Directory.CreateDirectory(_baseUrl + Path.GetDirectoryName(fileUrl));

            long fileSize;

            using (FileStream fileStream = System.IO.File.Open(_baseUrl + fileUrl, FileMode.Append))
            {
                foreach (var chunk in chunks)
                {
                    using (FileStream chunkStream = System.IO.File.Open(chunk, FileMode.Open))
                    {
                        byte[] chunkStreamContent = new byte[chunkStream.Length];

                        chunkStream.Read(chunkStreamContent, 0, (int)chunkStream.Length);
                        fileStream.Write(chunkStreamContent, 0, (int)chunkStream.Length);
                    }
                }

                fileSize = fileStream.Length;
            }

            Directory.Delete(_baseUrl + $"tmp{fileToken}", true);

            return fileSize;*/
        }

        public async Task<long> UploadChunk(Stream stream, string chunkToken)
        {
            return await Upload(stream, $"tmp{chunkToken.Substring(chunkToken.IndexOf("-") + 1)}/{chunkToken}.tmp");
        }

        public async Task<long> Upload(Stream stream, string fileUrl)
        {
            CloudBlobContainer container = await GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(fileUrl);

            await blob.UploadFromStreamAsync(stream);

            return stream.Length;

            // BLOB STORAGE

            /*
             * FILE SYSTEM
             * 
             * await Sleep();

            Directory.CreateDirectory(_baseUrl + Path.GetDirectoryName(fileUrl));

            long fileSize;

            using (FileStream fileStream = System.IO.File.Create(_baseUrl + fileUrl, stream.Length > 0 ? (int) stream.Length : 1))
            {
                fileSize = fileStream.Length;
                stream.CopyTo(fileStream);
            }

            return fileSize;*/
        }

        public async Task<string> Delete(Arquivo arquivo)
        {
            CloudBlobContainer container = await GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(arquivo.Url);

            await blob.DeleteAsync();

            return "success";

            /*await Sleep();

            if (File.Exists(_baseUrl + arquivo.Url))
            {
                File.Delete(_baseUrl + arquivo.Url);
            }

            return "success";*/
        }

        public async Task<string> Download(Arquivo arquivo)
        {
            CloudBlobContainer container = await GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(arquivo.Url);

            Stream stream = await blob.OpenReadAsync();

            return PrepararJson(arquivo, stream);

            /*await Sleep();

            Stream stream = System.IO.File.Open(_baseUrl + arquivo.Url, FileMode.Open);

            return PrepararJson(arquivo, stream);*/
        }

        private string PrepararJson(Arquivo arquivo, Stream stream)
        {
            string json = String.Empty;

            using (stream)
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    var jss = new JavaScriptSerializer();
                    jss.MaxJsonLength = Int32.MaxValue;

                    json = jss.Serialize(new
                    {
                        NomeArquivo = arquivo.Nome,
                        MimeType = MimeMapping.GetMimeMapping(arquivo.Nome),
                        Buffer = br.ReadBytes((int) stream.Length)
                    });
                }
            }

            return json;
        }

        private async Task Sleep()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(1000);
            });
        }
    }

    public class BlobStorageManager {
        private async Task<CloudBlobContainer> GetCloudBlobContainer()
        {
            CloudBlobClient blobClient = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("ged");

            await container.CreateIfNotExistsAsync();

            return container;
        }

        public async Task<long> MergeChunks(string fileUrl, string fileToken)
        {
            CloudBlobContainer container = await GetCloudBlobContainer();

            IEnumerable<string> chunksBlobs = OrdenarChunksBlobs(GetChunksBlobs(container, fileToken));

            CloudBlockBlob novoBlob = container.GetBlockBlobReference(fileUrl);

            long fileSize;

            using (Stream novoBlobStream = await novoBlob.OpenWriteAsync())
            {
                foreach (var chunk in chunksBlobs)
                {
                    CloudBlockBlob chunkBlob = container.GetBlockBlobReference(chunk);

                    using (Stream chunkStream = await chunkBlob.OpenReadAsync())
                    {
                        byte[] chunkStreamContent = new byte[chunkStream.Length];

                        chunkStream.Read(chunkStreamContent, 0, (int) chunkStream.Length);
                        novoBlobStream.Write(chunkStreamContent, 0, (int) chunkStream.Length);
                    }
                }

                fileSize = novoBlobStream.Length;
            }

            foreach (var chunk in chunksBlobs)
            {
                CloudBlockBlob chunkBlob = container.GetBlockBlobReference(chunk);

                await chunkBlob.DeleteIfExistsAsync();
            }

            return fileSize;
        }

        private IEnumerable<string> GetChunksBlobs(CloudBlobContainer container, string fileToken)
        {
            CloudBlobDirectory blobDirectory = container.GetDirectoryReference($"tmp{fileToken}");

            var blobs = new List<string>();

            foreach (var item in blobDirectory.ListBlobs(useFlatBlobListing: true))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob) item;
                    if (blob.Name.Contains(fileToken))
                    {
                        blobs.Add(blob.Name);
                    }
                }
            }

            return blobs;
        }

        private IEnumerable<string> OrdenarChunksBlobs(IEnumerable<string> chunksBlobs)
        {
            int maxLength = chunksBlobs.Max(x => x.Length);

            return chunksBlobs.OrderBy(x => x.PadLeft(maxLength, '0'));
        }

        public async Task<List<string>> ListBlobs(string folderUrl)
        {
            CloudBlobContainer container = await GetCloudBlobContainer();

            CloudBlobDirectory blobDirectory = container.GetDirectoryReference(folderUrl);

            var blobs = new List<string>();

            foreach (var item in blobDirectory.ListBlobs(useFlatBlobListing: true))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob) item;
                    blobs.Add(blob.Name);
                }
            }

            /*List<string> blobs = new List<string>();

            foreach (IListBlobItem item in container.ListBlobs(//.ListBlobs(useFlatBlobListing: true))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob) item;
                    blobs.Add(blob.Name);
                }
            }*/

            return blobs;
        }

        public async Task<long> UploadBlob(Stream stream, string fileUrl)
        {
            CloudBlobContainer container = await GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(fileUrl);

            await blob.UploadFromStreamAsync(stream);

            return stream.Length;
        }
    }
    
    /*class BlobStorageManager
    {
        private CloudBlobContainer GetCloudBlobContainer()
        {
            CloudBlobClient blobClient = new CloudBlobClient(new Uri("https://blob-storage-uri"));
            CloudBlobContainer container = blobClient.GetContainerReference("/");
            container.CreateIfNotExists();
            return container;
        }

        public async Task<string> UploadBlob(FileStream fileStream, string fileUrl)
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(fileUrl);

            await blob.UploadFromStreamAsync(fileStream);

            return "success!";
        }

        public ActionResult ListBlobs()
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            List<string> blobs = new List<string>();
            foreach (IListBlobItem item in container.ListBlobs(useFlatBlobListing: true))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob) item;
                    blobs.Add(blob.Name);
                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob blob = (CloudPageBlob) item;
                    blobs.Add(blob.Name);
                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory dir = (CloudBlobDirectory) item;
                    blobs.Add(dir.Uri.ToString());
                }
            }

            return View(blobs);
        }

        public async Task<string> DownloadBlob(string fileUrl)
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(fileUrl);

            using (var fileStream = File.OpenWrite(@"c:\src\downloadedBlob.txt"))
            {
                await blob.DownloadToStreamAsync(fileStream);
            }

            return "success!";
        }

        public async Task<string> DeleteBlob(string fileUrl)
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(fileUrl);

            await blob.DeleteAsync();

            return "success!";
        }
    }*/
}