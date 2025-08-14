using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ST10444262_CLDV6212_POE.Models;
using ST10444262_CLDV6212_POE.Services;
using System.Reflection;

namespace ST10444262_CLDV6212_POE.Controllers
{
    public class CustomerProductController : Controller
    {
        private readonly TableStorageService _tableStorage;

        public CustomerProductController(TableStorageService tableStorage)
        {
            _tableStorage = tableStorage;
        }
        //------------------------------------------------------------------------------------------//
        // GET: CustomerProduct
        public async Task<IActionResult> Index()
        {
            var customers = await _tableStorage.GetAllCustomersAsync();
            var products = await _tableStorage.GetAllProductAsync();

            //Create one object to pass through the View() from the 2 separate objects
            var viewModel = new CustomerProductViewModel
            {
                Customers = customers,
                Products = products
            };

            return View(viewModel);
        }
        //------------------------------------------------------------------------------------------//

        // GET: CustomerProduct/Create
        public ActionResult Create()
        {
            return View(new CustomerProductFormViewModel());
        }
        //------------------------------------------------------------------------------------------//

        // POST: CustomerProduct/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerProductFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Ensure model.Customer and model.Product are not null before inserting.
                if (model.Customer != null)
                {
                    // Generate ID for customer
                    model.Customer.CustomerId = Guid.NewGuid().ToString();
                    await _tableStorage.InsertCustomerAsync(model.Customer);
                }
                if (model.Product != null)
                {
                    // Generate ID for product
                    model.Product.ProductID = Guid.NewGuid().ToString();
                    await _tableStorage.InsertProductAsync(model.Product);
                }
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
        //------------------------------------------------------------------------------------------//
        // GET: CustomerProduct/CreateCustomer
        public IActionResult CreateCustomer()
        {
            return View(new Customer());
        }
        //------------------------------------------------------------------------------------------//
        // POST: CustomerProduct/CreateCustomer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomer([Bind("CustomerName,CustomerEmail")] Customer model)
        {
            // Remove RowKey and CustomerId from ModelState validation
            if (ModelState.ContainsKey("RowKey"))
            {
                ModelState.Remove("RowKey");
            }
            if (ModelState.ContainsKey("CustomerId"))
            {
                ModelState.Remove("CustomerId");
            }

            if (ModelState.IsValid)
            {
                // Generate unique ID (RowKey) automatically
                model.CustomerId = Guid.NewGuid().ToString();

                await _tableStorage.InsertCustomerAsync(model);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
        //------------------------------------------------------------------------------------------//
        // GET: CustomerProduct/EditCustomer/{id}
        public async Task<IActionResult> EditCustomer(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _tableStorage.GetCustomerAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }
        //------------------------------------------------------------------------------------------//
        // POST: CustomerProduct/EditCustomer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomer(string id, [Bind("PartitionKey,RowKey,Timestamp,ETag,CustomerName,CustomerEmail,CustomerId")] Customer customer)
        {
            if (id != customer.RowKey)
            {
                return NotFound();
            }

            // Remove RowKey, CustomerId, PartitionKey, Timestamp, ETag from ModelState validation
            // as they are part of ITableEntity and handled by Azure.Data.Tables.
            // ETag and Timestamp are also part of ITableEntity and are for concurrency control.
            ModelState.Remove("RowKey");
            ModelState.Remove("CustomerId");
            ModelState.Remove("PartitionKey");
            ModelState.Remove("Timestamp");
            ModelState.Remove("ETag");

            if (ModelState.IsValid)
            {
                try
                {
                    await _tableStorage.UpdateCustomerAsync(customer);
                }
                catch (RequestFailedException ex) when (ex.Status == 404)
                {
                    // If the entity wasn't found (e.g., deleted by another user), handle appropriately
                    return NotFound();
                }
                catch (Exception)
                {
                    // Add a model error or display a generic error message
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    return View(customer);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
        //------------------------------------------------------------------------------------------//
        // GET: CustomerProduct/DeleteCustomer/{id}
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _tableStorage.GetCustomerAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }
        //------------------------------------------------------------------------------------------//
        // POST: CustomerProduct/DeleteCustomer/{id} - Confirmed Deletion
        [HttpPost, ActionName("DeleteCustomer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedCustomer(string id)
        {
            await _tableStorage.DeleteCustomerAsync(id);
            return RedirectToAction(nameof(Index));
        }
        //------------------------------------------------------------------------------------------//
        // GET: CustomerProduct/CreateProduct
        public IActionResult CreateProduct()
        {
            return View(new Product());
        }
        //------------------------------------------------------------------------------------------//
        // POST: CustomerProduct/CreateProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct([Bind("ProductName,ProductDescription,ProductPrice")] Product model)
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

            if (ModelState.IsValid)
            {
                // Generate unique RowKey automatically
                model.ProductID = Guid.NewGuid().ToString();

                await _tableStorage.InsertProductAsync(model);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
        //------------------------------------------------------------------------------------------//
        // GET: CustomerProduct/EditProduct/{id}
        public async Task<IActionResult> EditProduct(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _tableStorage.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        //------------------------------------------------------------------------------------------//
        // POST: CustomerProduct/EditProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(string id, [Bind("PartitionKey,RowKey,Timestamp,ETag,ProductName,ProductDescription,ProductPrice,StudentNumber")] Product product)
        {
            if (id != product.RowKey)
            {
                return NotFound();
            }

            // Remove RowKey, ProductID, PartitionKey, Timestamp, ETag from ModelState validation
            ModelState.Remove("RowKey");
            ModelState.Remove("ProductID");
            ModelState.Remove("PartitionKey");
            ModelState.Remove("Timestamp");
            ModelState.Remove("ETag");

            if (ModelState.IsValid)
            {
                try
                {
                    await _tableStorage.UpdateProductAsync(product);
                }
                catch (RequestFailedException ex) when (ex.Status == 404)
                {
                    return NotFound();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    return View(product);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        //------------------------------------------------------------------------------------------//
        // GET: CustomerProduct/DeleteProduct/{id}
        public async Task<IActionResult> DeleteProduct(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _tableStorage.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        //------------------------------------------------------------------------------------------//
        // POST: CustomerProduct/DeleteProduct/{id} - Confirmed Deletion
        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedProduct(string id)
        {
            await _tableStorage.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
        //------------------------------------------------------------------------------------------//
    }
}

//---------------------END OF FILE------------------------------------------------------------------//