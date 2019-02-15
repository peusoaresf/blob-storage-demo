using System.IO;
using System.Threading.Tasks;

namespace WebUI.Classes
{
    public interface IFileManager
    {
        Task<string> DeleteAsync(Arquivo arquivo);
        Task<string> DownloadAsync(Arquivo arquivo);
        Task<long> MergeChunksAsync(string fileUrl, string fileToken);
        Task<long> UploadAsync(Stream stream, string fileUrl);
        Task<long> UploadChunkAsync(Stream stream, string chunkToken);
    }
}