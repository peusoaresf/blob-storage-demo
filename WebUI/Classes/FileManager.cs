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
        private static string _baseUrl = "C:\\GED_local\\";

        public async Task<string> Upload(Stream stream, string fileUrl)
        {
            await Sleep();

            Directory.CreateDirectory(_baseUrl + Path.GetDirectoryName(fileUrl));

            using (FileStream fileStream = System.IO.File.Create(_baseUrl + fileUrl, stream.Length > 0 ? (int) stream.Length : 1))
            {                
                stream.CopyTo(fileStream);
            }

            return "success";
        }

        public async Task<string> Delete(Arquivo arquivo)
        {
            await Sleep();

            if (File.Exists(_baseUrl + arquivo.Url))
            {
                File.Delete(_baseUrl + arquivo.Url);
            }

            return "success";
        }

        public async Task<string> Download(Arquivo arquivo)
        {
            await Sleep();

            Stream stream = System.IO.File.Open(_baseUrl + arquivo.Url, FileMode.Open);

            return PrepararJson(arquivo, stream);
        }

        private string PrepararJson(Arquivo arquivo, Stream stream)
        {
            string json = String.Empty;

            using (stream)
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    var jss = new JavaScriptSerializer();

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