using Azure;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Text_Speech.Services
{
    public class Blob : IBlob
    {
        private readonly string Container = "textimages";
        private readonly BlobServiceClient _blobServiceClient;
        public Blob(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task Upload(IFormFile model)
        {
            
            var blobClient = GetBlobServiceClient(model.FileName);
            await blobClient.UploadAsync(model.OpenReadStream(), overwrite: true);

        }
        public Uri GetUri(string file) 
        {
            var blobClient = GetBlobServiceClient(file);
            if (blobClient.ExistsAsync().Result)
            {
                return blobClient.Uri;
                
            }
            
            return null;
        }
        private BlobClient GetBlobServiceClient(string name)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(Container);
            var blobClient = blobContainer.GetBlobClient(name);
            
            return blobClient;
        }

        public async  Task Delete(string url)
        {
            var length = "https://deola.blob.core.windows.net/textimages/";
            var filename = url.Remove(0, length.Length);
            var blobclient = GetBlobServiceClient(filename);
            await blobclient.DeleteIfExistsAsync();
        }

        public async Task<Response<Azure.Storage.Blobs.Models.BlobDownloadInfo>> DownloadFile(string url)
        {
            var length = "https://deola.blob.core.windows.net/textimages/";
            var filename = url.Remove(0, length.Length);
            var blobclient = GetBlobServiceClient(filename);
            if (await blobclient.ExistsAsync())
            {
                var a = await blobclient.DownloadAsync();
                return a;
            }
            throw new NullReferenceException(nameof(blobclient));

        }



        /*  public async Task UploadFile(FileStream file)
          {
              var blobClient = GetBlobServiceClient("Document.Docx");
              await blobClient.UploadAsync(file, overwrite: true);

          }*/

        /*   public async Task UploadStream(Stream model)
           {
               var blobClient = GetBlobServiceClient("Audio.mp3");
               await blobClient.UploadAsync(model,overwrite:true);    

           }*/
    }
}

