using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Azure;
using Azure.Data.Tables;
using ST10444262_CLDV6212_POE.Models;

namespace ST10444262_CLDV6212_POE.Services
{
    public class TableStorageService
    {
        private readonly TableClient _tableClientCustomer;
        private readonly TableClient _tableClientProduct;
        private readonly TableClient _tableClientOrder;
        //------------------------------------------------------------------------------------------//
        #region Configuration
        public TableStorageService(IConfiguration config)
        {
            //table storage connection string
            var connectionString = config["AzureStorageConnectionString"];
            //each tables name
            var tableNameCustomer = "Customer";
            var tableNameProduct = "Product";
            var tableNameOrder = "Order";

            //customer
            _tableClientCustomer = new TableClient(connectionString, tableNameCustomer);
            //product
            _tableClientProduct = new TableClient(connectionString, tableNameProduct);
            //order
            _tableClientOrder = new TableClient(connectionString, tableNameOrder);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Initialize
        //Initialize
        public async Task InitializeAsync()
        {
            await _tableClientCustomer.CreateIfNotExistsAsync();
            await _tableClientProduct.CreateIfNotExistsAsync();
            await _tableClientOrder.CreateIfNotExistsAsync();
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Customer
        /// <summary>
        /// Insert a customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task InsertCustomerAsync(Customer customer)
        {
            await _tableClientCustomer.AddEntityAsync(customer);
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// List customers
        /// </summary>
        /// <returns></returns>
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var customers = new List<Customer>();

            await foreach (Customer entity in _tableClientCustomer.QueryAsync<Customer>())
            {
                customers.Add(entity);
            }

            return customers;
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// GET a single customer by PartitionKey and RowKey
        /// </summary>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        public async Task<Customer> GetCustomerAsync(string rowKey)
        {
            return await _tableClientCustomer.GetEntityAsync<Customer>("Customer", rowKey);
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// UPDATE a customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task UpdateCustomerAsync(Customer customer)
        {
            await _tableClientCustomer.UpsertEntityAsync(customer, TableUpdateMode.Replace);
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// DELETE a customer
        /// </summary>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        public async Task DeleteCustomerAsync(string rowKey)
        {
            await _tableClientCustomer.DeleteEntityAsync("Customer", rowKey);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Product
        /// <summary>
        /// Insert a product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task InsertProductAsync(Product product)
        {
            await _tableClientProduct.AddEntityAsync(product);
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// List products
        /// </summary>
        /// <returns></returns>
        public async Task<List<Product>> GetAllProductAsync()
        {
            var products = new List<Product>();

            await foreach (Product entity in _tableClientProduct.QueryAsync<Product>())
            {
                products.Add(entity);
            }

            return products;
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// GET a single product by PartitionKey and RowKey
        /// </summary>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        public async Task<Product> GetProductAsync(string rowKey)
        {
            return await _tableClientProduct.GetEntityAsync<Product>("Product", rowKey);
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// UPDATE a product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task UpdateProductAsync(Product product)
        {
            await _tableClientProduct.UpsertEntityAsync(product, TableUpdateMode.Replace);
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// DELETE a product
        /// </summary>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        public async Task DeleteProductAsync(string rowKey)
        {
            await _tableClientProduct.DeleteEntityAsync("Product", rowKey);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Order
        /// <summary>
        /// Insert an order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task InsertOrderAsync(Order order)
        {
            await _tableClientOrder.AddEntityAsync(order);
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// List orders
        /// </summary>
        /// <returns></returns>
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = new List<Order>();
            await foreach (Order entity in _tableClientOrder.QueryAsync<Order>())
            {
                orders.Add(entity);
            }
            return orders;
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// GET a single order by PartitionKey and RowKey
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        public async Task<Order> GetOrderAsync(string partitionKey, string rowKey)
        {
            return await _tableClientOrder.GetEntityAsync<Order>(partitionKey, rowKey);
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// UPDATE an order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task UpdateOrderAsync(Order order)
        {
            await _tableClientOrder.UpsertEntityAsync(order, TableUpdateMode.Replace);
        }
        //------------------------------------------------------------------------------------------//
        /// <summary>
        /// DELETE an order
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="rowKey"></param>
        /// <returns></returns>
        public async Task DeleteOrderAsync(string partitionKey, string rowKey)
        {
            await _tableClientOrder.DeleteEntityAsync(partitionKey, rowKey);
        }
        #endregion
    }
}
//---------------------END OF FILE------------------------------------------------------------------//