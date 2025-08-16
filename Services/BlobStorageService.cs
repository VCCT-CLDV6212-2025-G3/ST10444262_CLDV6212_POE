using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ST10444262_CLDV6212_POE.Models;
namespace ST10444262_CLDV6212_POE.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        //------------------------------------------------------------------------------------------//
        #region Configuration
        public BlobStorageService(IConfiguration config)
        {
            // blob storage connection string
            var connectionString = config["AzureStorageConnectionString"];
            // blob container name for product images
            string containerName = "product-images"; 

            var blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            _containerClient.CreateIfNotExists();
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Image Upload
        /// <summary>
        /// Uploads a new product image file.
        /// </summary>
        /// <param name="file">The IFormFile to upload.</param>
        /// <param name="productID"></param>
        /// <returns>The URL of the uploaded blob.</returns>
        public async Task<string> UploadProductImageAsync(IFormFile file, string productID)
        {
            var fileName = file.FileName;
            var blobClient = _containerClient.GetBlobClient(fileName);

            // Upload the file, overwriting if it exists
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            // Return the public URL of the uploaded blob
            return blobClient.Uri.ToString();
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Delete Image
        /// <summary>
        /// Deletes a product image file.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task DeleteProductImageAsync(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Display Image
        /// <summary>
        /// Gets the URL of a product image file for display.
        /// </summary>
        /// <param name="fileName">The name of the file to get the URL for.</param>
        /// <returns>The public URL of the blob.</returns>
        public string GetProductImageUrl(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            return blobClient.Uri.ToString();
        }
        #endregion
    }
}
//---------------------END OF FILE------------------------------------------------------------------//