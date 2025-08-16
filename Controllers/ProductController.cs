using Azure;
using Microsoft.AspNetCore.Mvc;
using ST10444262_CLDV6212_POE.Models;
using ST10444262_CLDV6212_POE.Services;
using System.Threading.Tasks;

namespace ST10444262_CLDV6212_POE.Controllers
{
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        private readonly BlobStorageService _blobStorageService;
        //------------------------------------------------------------------------------------------//
        public ProductController(TableStorageService tableStorageService, BlobStorageService blobStorageService)
        {
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
        }
        //------------------------------------------------------------------------------------------//
        #region Display All Products
        /// <summary>
        /// Display a list of all products
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _tableStorageService.GetAllProductAsync();
                return View(products);
            }
            catch (Exception ex)
            {
                // Handle the exception and returns an error view or message
                return StatusCode(500, $"An error occurred while retrieving products: {ex.Message}");
            }
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Creating New Product
        /// <summary>
        /// Display the form for creating a new product
        /// </summary>
        /// <returns></returns>
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Submitting New Product
        /// <summary>
        /// Handles the form submission for creating a new product
        /// </summary>
        /// <param name="product"></param>
        /// <param name="imageFile"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            // Remove RowKey and ProductID from ModelState validation
            if (ModelState.ContainsKey("RowKey"))
            {
                ModelState.Remove("RowKey");
            }
            if (ModelState.ContainsKey("ProductID"))
            {
                ModelState.Remove("ProductID");
            }
            if (ModelState.ContainsKey("ProductFileName"))
            {
                ModelState.Remove("ProductFileName");
            }
            if (ModelState.ContainsKey("ProductFileUrl"))
            {
                ModelState.Remove("ProductFileUrl");
            }

            if (ModelState.IsValid)
            {
                // Generate a unique ID for the new product
                product.ProductID = Guid.NewGuid().ToString();

                if (imageFile != null)
                {
                    // Upload the image to Blob Storage and get the URL
                    product.ProductFileUrl = await _blobStorageService.UploadProductImageAsync(imageFile, product.ProductID);
                    product.ProductFileName = imageFile.FileName;
                }

                // Insert the product entity into Table Storage
                await _tableStorageService.InsertProductAsync(product);

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Display Single Product
        /// <summary>
        /// Display the details of a single product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _tableStorageService.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Editing a Product
        /// <summary>
        /// Display the form for editing an existing product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _tableStorageService.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Submitting a Edit
        /// <summary>
        /// Handles the form submission for editing an existing product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <param name="newImageFile"></param>
        /// <returns></returns>
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Product product, IFormFile newImageFile)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            //Clear validation for all fields that are not part of the core form data
            ModelState.Clear();

            
            TryValidateModel(product);

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the existing, complete product from the database.
                    var originalProduct = await _tableStorageService.GetProductAsync(id);
                    if (originalProduct == null)
                    {
                        return NotFound();
                    }

                    // Update the original product with the new data from the form.
                    originalProduct.ProductName = product.ProductName;
                    originalProduct.ProductDescription = product.ProductDescription;
                    originalProduct.ProductPrice = product.ProductPrice;
                    originalProduct.ETag = product.ETag;

                    // Conditionally handle the image update.
                    if (newImageFile != null && newImageFile.Length > 0)
                    {
                        // Delete the old image from Blob Storage.
                        if (!string.IsNullOrEmpty(originalProduct.ProductFileName))
                        {
                            await _blobStorageService.DeleteProductImageAsync($"{originalProduct.ProductID}-{originalProduct.ProductFileName}");
                        }

                        // Upload the new image and update the originalProduct object.
                        originalProduct.ProductFileUrl = await _blobStorageService.UploadProductImageAsync(newImageFile, originalProduct.ProductID);
                        originalProduct.ProductFileName = newImageFile.FileName;
                    }
                    // If newImageFile is null, the existing image properties on originalProduct are untouched.

                    // Save the now-complete object back to Table Storage.
                    await _tableStorageService.UpdateProductAsync(originalProduct);
                }
                catch (RequestFailedException ex)
                {
                    if (ex.Status == 412)
                    {
                        ModelState.AddModelError(string.Empty, "The product has been updated by another user. Please try again.");
                        return View(product);
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            // If ModelState is not valid, return the view with the current product model.
            return View(product);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Deleting Product
        /// <summary>
        /// Display the confirmation page for deleting a product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _tableStorageService.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Submitting a Delete
        //Handles the form submission for deleting a product
        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var product = await _tableStorageService.GetProductAsync(id);
            if (product != null)
            {
                // Delete the image from blob storage before deleting the table entity
                if (!string.IsNullOrEmpty(product.ProductFileName))
                {
                    await _blobStorageService.DeleteProductImageAsync($"{product.ProductID}-{product.ProductFileName}");
                }

                await _tableStorageService.DeleteProductAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
//---------------------END OF FILE------------------------------------------------------------------//