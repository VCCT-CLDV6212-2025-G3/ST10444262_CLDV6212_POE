using Azure.Storage.Queues;

namespace ST10444262_CLDV6212_POE.Services
{
    public class QueueStorageService
    {
        private readonly QueueClient _ordersQueueClient;
        private readonly QueueClient _inventoryQueueClient;
        //------------------------------------------------------------------------------------------//
        public QueueStorageService(IConfiguration config)
        {
            //queue storage connection 
            var connectionString = config["AzureStorageConnectionString"];
            //Orders Queue
            _ordersQueueClient = new QueueClient(connectionString, "orders");
            _ordersQueueClient.CreateIfNotExists();
            //Inventory Queue
            _inventoryQueueClient = new QueueClient(connectionString, "inventory");
            _inventoryQueueClient.CreateIfNotExists();
        }
        //------------------------------------------------------------------------------------------//
        public async Task SendOrderMessageAsync(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
                await _ordersQueueClient.SendMessageAsync(message);
        }
        //------------------------------------------------------------------------------------------//
        public async Task SendInventoryMessageAsync(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
                await _inventoryQueueClient.SendMessageAsync(message);
        }
        //------------------------------------------------------------------------------------------//
        public async Task<string?> PeekOrderMessageAsync()
        {
            var peeked = await _ordersQueueClient.PeekMessageAsync();
            return peeked?.Value?.MessageText;
        }
        //------------------------------------------------------------------------------------------//
        public async Task<string?> PeekInventoryMessageAsync()
        {
            var peeked = await _inventoryQueueClient.PeekMessageAsync();
            return peeked?.Value?.MessageText;
        }
    }
}

//---------------------END OF FILE------------------------------------------------------------------//