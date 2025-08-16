using Azure.Storage.Files.Shares;

namespace ST10444262_CLDV6212_POE.Services
{
    public class FileShareService
    {
        private readonly ShareClient _shareClient;
        //------------------------------------------------------------------------------------------//
        #region Configuration
        public FileShareService(IConfiguration config)
        {
            //file storage connection
            var connectionString = config["AzureStorageConnectionString"];
            //file storage name
            string shareName = "file";
            _shareClient = new ShareClient(connectionString, shareName);
            _shareClient.CreateIfNotExists();
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Upload File
        /// <summary>
        /// Upload a file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return;

            var directory = _shareClient.GetRootDirectoryClient();
            var fileClient = directory.GetFileClient(file.FileName);

            using var stream = file.OpenReadStream();
            await fileClient.CreateAsync(file.Length);
            await fileClient.UploadAsync(stream);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Display file
        /// <summary>
        /// Display
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> ListFilesAsync()
        {
            var directory = _shareClient.GetRootDirectoryClient();
            var files = new List<string>();

            await foreach (var item in directory.GetFilesAndDirectoriesAsync())
            {
                if (!item.IsDirectory)
                    files.Add(item.Name);
            }

            return files;
        }
        #endregion
        //------------------------------------------------------------------------------------------//\
        #region Download File
        /// <summary>
        /// Download the file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            var directory = _shareClient.GetRootDirectoryClient();
            var fileClient = directory.GetFileClient(fileName);
            var download = await fileClient.DownloadAsync();
            return download.Value.Content;
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Delete File
        /// <summary>
        /// Delete a file from Azure File Share
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task DeleteFileAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var directory = _shareClient.GetRootDirectoryClient();
            var fileClient = directory.GetFileClient(fileName);
            await fileClient.DeleteIfExistsAsync();
        }
        #endregion
    }
}
//---------------------END OF FILE------------------------------------------------------------------//