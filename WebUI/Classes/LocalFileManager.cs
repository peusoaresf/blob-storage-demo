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
    public class LocalFileManager : IFileManager
    {
        //private static string _baseUrl = "C:\\dev\\ged_repo\\"; // Thinkpad
        private static string _baseUrl = "C:\\GED_local\\"; // Tenti

        public async Task<string> DeleteAsync(Arquivo arquivo)
        {
            await SleepAsync();

            if (File.Exists(_baseUrl + arquivo.Url))
            {
                File.Delete(_baseUrl + arquivo.Url);
            }

            return "success";
        }

        public async Task<Stream> GetStream(Arquivo arquivo)
        {
            await SleepAsync();

            return System.IO.File.Open(_baseUrl + arquivo.Url, FileMode.Open);
        }

        public async Task<long> MergeChunksAsync(string fileUrl, string fileToken)
        {
            await SleepAsync();

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

            return fileSize;
        }

        public async Task<long> UploadAsync(Stream stream, string fileUrl)
        {
            await SleepAsync();

            Directory.CreateDirectory(_baseUrl + Path.GetDirectoryName(fileUrl));

            long fileSize;

            using (FileStream fileStream = System.IO.File.Create(_baseUrl + fileUrl, stream.Length > 0 ? (int) stream.Length : 1))
            {
                fileSize = fileStream.Length;
                stream.CopyTo(fileStream);
            }

            return fileSize;
        }

        public async Task<long> UploadChunkAsync(Stream stream, string chunkToken)
        {
            return await UploadAsync(stream, $"tmp{chunkToken.Substring(chunkToken.IndexOf("-") + 1)}/{chunkToken}.tmp");
        }

        private async Task SleepAsync()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(1000);
            });
        }
    }
}