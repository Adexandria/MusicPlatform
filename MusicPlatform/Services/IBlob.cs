using Azure;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Text_Speech.Services
{
    public  interface IBlob
    {
        Task Upload(IFormFile model);
        Uri GetUri(string file);

        Task Delete(string url);

      //  Task UploadFile(FileStream file);
      
       // Task UploadStream(Stream model);
        
       /* Task<byte[]> Download(IFormFile file);*/
        Task<Response<Azure.Storage.Blobs.Models.BlobDownloadInfo>> DownloadFile(string file);
    }
}
