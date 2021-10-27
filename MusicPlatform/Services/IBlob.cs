using Azure;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Text_Speech.Services
{
    public  interface IBlob
    {
        Task UploadSong(IFormFile model,string fileName);
        Task UploadImage(IFormFile model);
        string GetUri(string file);
        Task Delete(string url);
        Task<Response<Azure.Storage.Blobs.Models.BlobDownloadInfo>> DownloadFile(string file);
    }
}
