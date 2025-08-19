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
            if (file == null || file.Length == 0)
            {
                //Add a message for the user.
                TempData["ErrorMessage"] = "Please select a file to upload.";
                return RedirectToAction("Index"); 
            }

            // Define the allowed extensions
            var allowedExtensions = new[] { ".pdf", ".docx", ".txt" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
            {
                // Add an error message to display on the page
                TempData["ErrorMessage"] = "Invalid file type. Only .pdf, .docx, and .txt files are allowed.";
                return RedirectToAction("Index");
            }

            try
            {
                await _fileService.UploadFileAsync(file); 
                TempData["SuccessMessage"] = "File uploaded successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception and show an error
                TempData["ErrorMessage"] = "An error occurred during file upload.";
            }

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