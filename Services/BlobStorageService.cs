using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ST10444262_CLDV6212_POE.Services
{

    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        public BlobStorageService(IConfiguration config)
        {
            //blob storage connection
            var connectionString = config["AzureStorageConnectionString"];
            //blob container name
            string containerName = "images";

            var blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            _containerClient.CreateIfNotExists();
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// handles uploading a file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task UploadFileAsync(IFormFile file)
        {
            var blobClient = _containerClient.GetBlobClient(file.FileName);
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// Handles downloading a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            if (await blobClient.ExistsAsync())
            {
                var response = await blobClient.DownloadAsync();
                return response.Value.Content;
            }
            return null;
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// Handles getting all the files
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetAllFilesAsync()
        {
            var blobs = _containerClient.GetBlobsAsync();
            var files = new List<string>();

            await foreach (BlobItem blob in blobs)
            {
                files.Add(blob.Name);
            }

            return files;
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// Handles the deletion of a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task DeleteFileAsync(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets the blobs url to allow previews
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetBlobUrl(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            return blobClient.Uri.ToString(); // Returns the URL (public or SAS)
        }
    }


}
//---------------------END OF FILE------------------------------------------------------------------//
