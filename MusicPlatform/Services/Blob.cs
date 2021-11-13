using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.RegularExpressions;
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

        public async Task UploadSong(IFormFile model,string fileName)
        {
            var file = Regex.Replace(fileName, @"\s+", "");
            BlobClient blobClient = GetBlobServiceClient(file);
            await blobClient.UploadAsync(model.OpenReadStream(), overwrite: true);

        }
        public string GetUri(string file) 
        {
            var filename = Regex.Replace(file, @"\s+", "");
            BlobClient blobClient = GetBlobServiceClient(filename);
            if (blobClient.ExistsAsync().Result)
            {
                return blobClient.Uri.AbsoluteUri;
            }
            return null;
        }
        public async Task UploadImage(IFormFile model)
        {
            BlobClient blobClient = GetBlobServiceClient(model.FileName);
            await blobClient.UploadAsync(model.OpenReadStream(), overwrite: true);
        }
        private BlobClient GetBlobServiceClient(string name)
        {
            BlobContainerClient blobContainer = _blobServiceClient.GetBlobContainerClient(Container);
            BlobClient blobClient = blobContainer.GetBlobClient(name);
                return blobClient;
           
        }

        public async  Task Delete(string url)
        {
            string length = "https://deola.blob.core.windows.net/textimages/";
            string filename = url.Remove(0, length.Length);
            BlobClient blobclient = GetBlobServiceClient(filename);
            await blobclient.DeleteIfExistsAsync();
        }

        public async Task<Response<BlobDownloadInfo>> DownloadFile(string url)
        {
            string length = "https://deola.blob.core.windows.net/textimages/";
            string filename = url.Remove(0, length.Length);
            BlobClient blobclient = GetBlobServiceClient(filename);
            if (await blobclient.ExistsAsync())
            {
                return await blobclient.DownloadAsync();
            }
            throw new NullReferenceException(nameof(blobclient));

        }

      
    }
}

