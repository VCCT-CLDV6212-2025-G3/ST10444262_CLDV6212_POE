using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ST10444262_CLDV6212_POE.Models.Blob;
using ST10444262_CLDV6212_POE.Services;
using System;

namespace ST10444262_CLDV6212_POE.Controllers
{
    public class MultimediaController : Controller
    {
        private readonly BlobStorageService _blobService;

        public MultimediaController(BlobStorageService blobService)
        {
            _blobService = blobService;

        }
        //------------------------------------------------------------------------------------------//
        public async Task<IActionResult> Index()
        {
            var fileNames = await _blobService.GetAllFilesAsync();
            var filesWithUrls = fileNames.Select(fn => new BlobFileViewModel
            {
                FileName = fn,
                Url = _blobService.GetBlobUrl(fn)  
            }).ToList();

            return View(filesWithUrls);
        }

        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// handles uploading a file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                await _blobService.UploadFileAsync(file);
            }
            return RedirectToAction("Index");
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// Handles downloading a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<IActionResult> Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("File name must be provided.");

            var stream = await _blobService.DownloadFileAsync(fileName);
            if (stream == null)
                return NotFound();

            return File(stream, "application/octet-stream", fileName);
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// Handles the deletion of a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Delete(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest();

            await _blobService.DeleteFileAsync(fileName);
            return RedirectToAction("Index");
        }

    }

}
//---------------------END OF FILE------------------------------------------------------------------//