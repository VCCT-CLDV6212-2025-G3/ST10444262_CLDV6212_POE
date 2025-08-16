using Azure;
using Microsoft.AspNetCore.Mvc;
using ST10444262_CLDV6212_POE.Models;
using ST10444262_CLDV6212_POE.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ST10444262_CLDV6212_POE.Controllers
{
    [Route("[controller]")]
    public class CustomerController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        //------------------------------------------------------------------------------------------//
        public CustomerController(TableStorageService tableStorageService)
        {
            _tableStorageService = tableStorageService;
        }
        //------------------------------------------------------------------------------------------//
        #region Display All Customers
        /// <summary>
        /// Action to display a list of all customers
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var customers = await _tableStorageService.GetAllCustomersAsync();
                // Returns a view with a list of customers
                return View(customers); 
            }
            catch (Exception ex)
            {
                // Handle the exception and return an error view or message
                return StatusCode(500, $"An error occurred while retrieving customers: {ex.Message}");
            }
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Create Customer
        /// <summary>
        /// Display the form for creating a new customer
        /// </summary>
        /// <returns></returns>
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View(); 
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Submitting Customer Creation
        /// <summary>
        /// Submission for creating a new customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerName, CustomerEmail")] Customer customer)
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
                // Generate a unique RowKey for the new customer
                customer.RowKey = Guid.NewGuid().ToString();

                await _tableStorageService.InsertCustomerAsync(customer);
                return RedirectToAction(nameof(Index)); 
            }
            return View(customer); 
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Display Single Customer
        /// <summary>
        /// Display the details of a single customer
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

            var customer = await _tableStorageService.GetCustomerAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Edit Customer
        /// <summary>
        /// Displays editing a customer
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

            var customer = await _tableStorageService.GetCustomerAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer); 
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Submitting Customer Edit
        /// <summary>
        /// Submission for editing a customer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("RowKey, CustomerName, CustomerEmail, PartitionKey, ETag")] Customer customer)
        {
            if (id != customer.RowKey)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _tableStorageService.UpdateCustomerAsync(customer);
                }
                catch (RequestFailedException ex)
                {
                    
                    if (ex.Status == 412)
                    {
                        ModelState.AddModelError(string.Empty, "The customer has been updated by another user. Please try again.");
                        return View(customer);
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Deleting a Customer
        /// <summary>
        /// Displays delting a customer 
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

            var customer = await _tableStorageService.GetCustomerAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer); 
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Submitting Customer Delete
        /// <summary>
        /// SUbmission for deleting customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _tableStorageService.DeleteCustomerAsync(id);
            return RedirectToAction(nameof(Index)); 
        }
        #endregion
    }
}
//---------------------END OF FILE------------------------------------------------------------------//