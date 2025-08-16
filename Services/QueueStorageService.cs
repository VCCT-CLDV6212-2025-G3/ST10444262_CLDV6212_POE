using Azure.Storage.Queues;

namespace ST10444262_CLDV6212_POE.Services
{
    public class QueueStorageService
    {
        private readonly QueueClient _ordersQueueClient;

        //------------------------------------------------------------------------------------------//
        #region Configuration
        public QueueStorageService(IConfiguration config)
        {
            //queue storage connection
            var connectionString = config["AzureStorageConnectionString"];

            //Orders Queue client initialization
            _ordersQueueClient = new QueueClient(connectionString, "order-queue");
            _ordersQueueClient.CreateIfNotExists();
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Send Order Message
        /// <summary>
        /// Sends a message to the orders queue.
        /// </summary>
        /// <param name="message">The message to send, typically a serialized Order object.</param>
        public async Task SendOrderMessageAsync(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
                await _ordersQueueClient.SendMessageAsync(message);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Peek Order Message
        /// <summary>
        /// Peeks the next message from the orders queue without removing it.
        /// </summary>
        /// <returns>The message text or null if the queue is empty.</returns>
        public async Task<string?> PeekOrderMessageAsync()
        {
            var peeked = await _ordersQueueClient.PeekMessageAsync();
            return peeked?.Value?.MessageText;
        }
        #endregion
    }
}
//---------------------END OF FILE------------------------------------------------------------------//