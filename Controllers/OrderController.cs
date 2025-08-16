using Microsoft.AspNetCore.Mvc;
using ST10444262_CLDV6212_POE.Models;
using ST10444262_CLDV6212_POE.Services;
using System.Text.Json;
using System.Threading.Tasks;

namespace ST10444262_CLDV6212_POE.Controllers
{
    [Route("[controller]")]
    public class OrderController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        private readonly QueueStorageService _queueStorageService;
        //------------------------------------------------------------------------------------------//
        public OrderController(TableStorageService tableStorageService, QueueStorageService queueStorageService)
        {
            _tableStorageService = tableStorageService;
            _queueStorageService = queueStorageService;
        }
        //------------------------------------------------------------------------------------------//
        #region Displays all orders
        /// <summary>
        /// Displays a list of all orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var orders = await _tableStorageService.GetAllOrdersAsync();

                // Peek a single message from the queue and pass it to the view
                var queueMessage = await _queueStorageService.PeekOrderMessageAsync();
                ViewData["QueueMessage"] = queueMessage;

                return View(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving orders: {ex.Message}");
            }
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Display New Order
        /// <summary>
        /// Displays the form for creating a new order
        /// </summary>
        /// <returns></returns>
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Submitting New Order
        /// <summary>
        /// Handles the form submission for creating a new order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerName, ProductName")] Order order)
        {

            ModelState.Remove("OrderID");
            ModelState.Remove("RowKey");

            if (ModelState.IsValid)
            {
                // Generate a unique RowKey using a GUID (string)
                order.RowKey = Guid.NewGuid().ToString();


                order.OrderID = Math.Abs(order.RowKey.GetHashCode());

                // Set the order date to the current UTC time
                order.OrderDate = DateTime.UtcNow;

                // Serialize the order object to a JSON string for the queue
                var orderMessage = JsonSerializer.Serialize(order);

                // Send the serialized order to the Azure Queue 
                await _queueStorageService.SendOrderMessageAsync(orderMessage);

                // Insert the order entity into Table Storage
                await _tableStorageService.InsertOrderAsync(order);

                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Display Single Order
        /// <summary>
        /// Displays the details of a single order
        /// </summary>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        [HttpGet("Details/{rowKey}")]
        public async Task<IActionResult> Details(string rowKey)
        {
            if (string.IsNullOrEmpty(rowKey))
            {
                return NotFound();
            }

            var order = await _tableStorageService.GetOrderAsync("Order", rowKey);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Display Delete
        /// <summary>
        /// Displays the confirmation page for deleting an order
        /// </summary>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        [HttpGet("Delete/{rowKey}")]
        public async Task<IActionResult> Delete(string rowKey)
        {
            if (string.IsNullOrEmpty(rowKey))
            {
                return NotFound();
            }

            var order = await _tableStorageService.GetOrderAsync("Order", rowKey);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Submitting Delete
        /// <summary>
        /// Handles the form submission for deleting an order
        /// </summary>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        [HttpPost("Delete/{rowKey}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string rowKey)
        {
            await _tableStorageService.DeleteOrderAsync("Order", rowKey);
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
//---------------------END OF FILE------------------------------------------------------------------//