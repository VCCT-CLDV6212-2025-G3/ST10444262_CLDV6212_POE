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
        //Insert a customer
        public async Task InsertCustomerAsync(Customer customer)
        {
            await _tableClientCustomer.AddEntityAsync(customer);
        }
        //------------------------------------------------------------------------------------------//
        //List customers
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
        // GET a single customer by PartitionKey and RowKey
        public async Task<Customer> GetCustomerAsync(string rowKey)
        {
            return await _tableClientCustomer.GetEntityAsync<Customer>("Customer", rowKey);
        }
        //------------------------------------------------------------------------------------------//
        // UPDATE a customer
        public async Task UpdateCustomerAsync(Customer customer)
        {
            await _tableClientCustomer.UpsertEntityAsync(customer, TableUpdateMode.Replace);
        }
        //------------------------------------------------------------------------------------------//
        // DELETE a customer
        public async Task DeleteCustomerAsync(string rowKey)
        {
            await _tableClientCustomer.DeleteEntityAsync("Customer", rowKey);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Product
        //Insert a product
        public async Task InsertProductAsync(Product product)
        {
            await _tableClientProduct.AddEntityAsync(product);
        }
        //------------------------------------------------------------------------------------------//
        //List products
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
        // GET a single product by PartitionKey and RowKey
        public async Task<Product> GetProductAsync(string rowKey)
        {
            return await _tableClientProduct.GetEntityAsync<Product>("Product", rowKey);
        }
        //------------------------------------------------------------------------------------------//
        // UPDATE a product
        public async Task UpdateProductAsync(Product product)
        {
            await _tableClientProduct.UpsertEntityAsync(product, TableUpdateMode.Replace);
        }
        //------------------------------------------------------------------------------------------//
        // DELETE a product
        public async Task DeleteProductAsync(string rowKey)
        {
            await _tableClientProduct.DeleteEntityAsync("Product", rowKey);
        }
        #endregion
        //------------------------------------------------------------------------------------------//
        #region Order
        //Insert an order
        public async Task InsertOrderAsync(Order order)
        {
            await _tableClientOrder.AddEntityAsync(order);
        }
        //------------------------------------------------------------------------------------------//
        //List orders
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
        // GET a single order by PartitionKey and RowKey
        public async Task<Order> GetOrderAsync(string partitionKey, string rowKey)
        {
            return await _tableClientOrder.GetEntityAsync<Order>(partitionKey, rowKey);
        }
        //------------------------------------------------------------------------------------------//
        // UPDATE an order
        public async Task UpdateOrderAsync(Order order)
        {
            await _tableClientOrder.UpsertEntityAsync(order, TableUpdateMode.Replace);
        }
        //------------------------------------------------------------------------------------------//
        // DELETE an order
        public async Task DeleteOrderAsync(string partitionKey, string rowKey)
        {
            await _tableClientOrder.DeleteEntityAsync(partitionKey, rowKey);
        }
        #endregion
    }
}
//---------------------END OF FILE------------------------------------------------------------------//