using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ST10444262_CLDV6212_POE.Services;

namespace ST10444262_CLDV6212_POE.Controllers
{
    public class ContractController : Controller
    {
        private readonly FileShareService _fileService;
        //------------------------------------------------------------------------------------------//
        public ContractController(FileShareService fileService)
        {
            _fileService = fileService;
        }
        //------------------------------------------------------------------------------------------//
        #region List
        /// <summary>
        /// Displays a list of uploaded files
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var files = await _fileService.ListFilesAsync();
            return View(files);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Upload
        /// <summary>
        ///  Uploads a file to storage
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            await _fileService.UploadFileAsync(file);
            return RedirectToAction("Index");
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Download
        /// <summary>
        /// Downloads a specified file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<IActionResult> Download(string fileName)
        {
            var stream = await _fileService.DownloadFileAsync(fileName);
            return File(stream, "application/octet-stream", fileName);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Delete
        /// <summary>
        /// Deletes a specified file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                await _fileService.DeleteFileAsync(fileName);
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion

    }
}
//---------------------END OF FILE------------------------------------------------------------------//