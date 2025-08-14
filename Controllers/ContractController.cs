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
        public async Task<IActionResult> Index()
        {
            var files = await _fileService.ListFilesAsync();
            return View(files);
        }
        //------------------------------------------------------------------------------------------//
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            await _fileService.UploadFileAsync(file);
            return RedirectToAction("Index");
        }
        //------------------------------------------------------------------------------------------//
        public async Task<IActionResult> Download(string fileName)
        {
            var stream = await _fileService.DownloadFileAsync(fileName);
            return File(stream, "application/octet-stream", fileName);
        }
        //------------------------------------------------------------------------------------------//
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


    }
}
//---------------------END OF FILE------------------------------------------------------------------//