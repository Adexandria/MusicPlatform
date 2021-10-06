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

        Task DeleteImage(string url);
      //  Task UploadFile(FileStream file);
      
       // Task UploadStream(Stream model);
        
       /* Task<byte[]> Download(IFormFile file);
        Task<string[]> DownloadFile(string file);*/
    }
}
