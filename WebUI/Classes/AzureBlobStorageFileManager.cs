using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace WebUI.Classes
{
    public class AzureBlobStorageFileManager : IFileManager
    {
        public async Task<string> DeleteAsync(Arquivo arquivo)
        {
            CloudBlobContainer container = await GetCloudBlobContainerAsync();
            CloudBlockBlob blob = container.GetBlockBlobReference(arquivo.Url);

            await blob.DeleteAsync();

            return "success";
        }

        public async Task<string> DownloadAsync(Arquivo arquivo)
        {
            CloudBlobContainer container = await GetCloudBlobContainerAsync();
            CloudBlockBlob blob = container.GetBlockBlobReference(arquivo.Url);

            Stream stream = await blob.OpenReadAsync();

            return PrepararJson(arquivo, stream);
        }

        public async Task<long> MergeChunksAsync(string fileUrl, string fileToken)
        {
            CloudBlobContainer container = await GetCloudBlobContainerAsync();

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
        }

        public async Task<long> UploadAsync(Stream stream, string fileUrl)
        {
            CloudBlobContainer container = await GetCloudBlobContainerAsync();
            CloudBlockBlob blob = container.GetBlockBlobReference(fileUrl);

            await blob.UploadFromStreamAsync(stream);

            return stream.Length;
        }

        public async Task<long> UploadChunkAsync(Stream stream, string chunkToken)
        {
            return await UploadAsync(stream, $"tmp{chunkToken.Substring(chunkToken.IndexOf("-") + 1)}/{chunkToken}.tmp");
        }

        private IEnumerable<string> GetChunksBlobs(CloudBlobContainer container, string fileToken)
        {
            CloudBlobDirectory blobDirectory = container.GetDirectoryReference($"tmp{fileToken}");

            var blobs = new List<string>();

            foreach (var item in blobDirectory.ListBlobs(useFlatBlobListing: true))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    if (blob.Name.Contains(fileToken))
                    {
                        blobs.Add(blob.Name);
                    }
                }
            }

            return blobs;
        }

        private async Task<CloudBlobContainer> GetCloudBlobContainerAsync()
        {
            CloudBlobClient blobClient = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("ged");

            await container.CreateIfNotExistsAsync();

            return container;
        }
        
        private IEnumerable<string> OrdenarChunksBlobs(IEnumerable<string> chunksBlobs)
        {
            int maxLength = chunksBlobs.Max(x => x.Length);

            return chunksBlobs.OrderBy(x => x.PadLeft(maxLength, '0'));
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
                        Buffer = br.ReadBytes((int)stream.Length)
                    });
                }
            }

            return json;
        }
    }
}