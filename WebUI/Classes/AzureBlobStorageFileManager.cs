using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            if (!arquivo.IsDiretorio)
            {
                CloudBlobContainer container = await GetCloudBlobContainerAsync();
                CloudBlockBlob blob = container.GetBlockBlobReference(arquivo.Url);

                await blob.DeleteAsync();
            }

            return "success";
        }

        public async Task<Stream> GetStream(Arquivo arquivo)
        {
            CloudBlobContainer container = await GetCloudBlobContainerAsync();
            CloudBlockBlob blob = container.GetBlockBlobReference(arquivo.Url);

            return await blob.OpenReadAsync();
        }

        public async Task<long> MergeChunksAsync(string fileUrl, string fileToken)
        {
            CloudBlobContainer container = await GetCloudBlobContainerAsync();

            IEnumerable<string> chunksBlobs = OrdenarChunksBlobs(GetChunksBlobs(container, fileToken));

            CloudBlockBlob novoBlob = container.GetBlockBlobReference(fileUrl);

            //int contador = 1;

            using (Stream novoBlobStream = await novoBlob.OpenWriteAsync())
            {
                foreach (var chunk in chunksBlobs)
                {
                    CloudBlockBlob chunkBlob = container.GetBlockBlobReference(chunk);
                    
                    using (Stream chunkStream = await chunkBlob.OpenReadAsync())
                    {
                        /*using (SqlConnection sqlConnection = new SqlConnection("Data Source=TENTIRJD0155;Initial Catalog=blobstoragedemodb;User ID=restuser;Password=123456"))
                        {
                            await sqlConnection.OpenAsync();

                            SqlCommand sqlCommand = sqlConnection.CreateCommand();

                            sqlCommand.CommandText = "insert into Aux(arquivo, contador) values('" + novoBlob.Name + "', " + contador + ")";
                            contador++;

                            await sqlCommand.ExecuteNonQueryAsync();
                        }*/

                        byte[] chunkStreamContent = new byte[chunkStream.Length];
                        
                        await chunkStream.ReadAsync(chunkStreamContent, 0, Convert.ToInt32(chunkStream.Length));
                        
                        Task t = novoBlobStream.WriteAsync(chunkStreamContent, 0, Convert.ToInt32(chunkStream.Length));

                        if (await Task.WhenAny(t, Task.Delay(50000)) != t)
                        {
                            // Se passado um tempo de 50 segundos e a escrita intermediaria no arquivo destino ainda não foi finalizada,
                            // é preciso lançar um erro e reiniciar o processamento do arquivo. Não é possível determinar se a execução anterior foi finalizada e 
                            // o quanto foi escrito na stream destino
                            throw new Exception("Merge Timeout - Reiniciar o processamento do arquivo");
                        }
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
    }
}