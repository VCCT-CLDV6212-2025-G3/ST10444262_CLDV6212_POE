using Microsoft.AspNetCore.Mvc;
using ST10444262_CLDV6212_POE.Models.OrderInventory;
using ST10444262_CLDV6212_POE.Services;
using System.Text.Json;

public class OrderInventoryController : Controller
{
    private readonly QueueStorageService _queue;

    public OrderInventoryController(QueueStorageService queue)
    {
        _queue = queue;
    }

    public async Task<IActionResult> Index()
    {
        var model = new OrderInventoryViewModel
        {
            PeekedOrderMessage = await _queue.PeekOrderMessageAsync(),
            PeekedInventoryMessage = await _queue.PeekInventoryMessageAsync()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SendOrder(OrderViewModel order)
    {
        // Generate unique OrderId here
        order.OrderId = Guid.NewGuid().ToString();
        var jsonMessage = JsonSerializer.Serialize(order);
        await _queue.SendOrderMessageAsync(jsonMessage);
        TempData["Message"] = "Order message sent!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> SendInventory(InventoryViewModel inventory)
    {
        var jsonMessage = JsonSerializer.Serialize(inventory);
        await _queue.SendInventoryMessageAsync(jsonMessage);
        TempData["Message"] = "Inventory message sent!";
        return RedirectToAction(nameof(Index));
    }
}