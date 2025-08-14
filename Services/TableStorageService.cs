using Azure.Data.Tables;
using ST10444262_CLDV6212_POE.Models;

namespace ST10444262_CLDV6212_POE.Services
{
    public class TableStorageService
    {
        private readonly TableClient _tableClientCustomer;
        private readonly TableClient _tableClientProduct;
        //------------------------------------------------------------------------------------------//
        public TableStorageService(IConfiguration config)
        {
            //table storage connection string
            var connectionString = config["AzureStorageConnectionString"];
            //each tables name
            var tableNameCustomer = "Customer";
            var tableNameProduct = "Product";

            //customer
            _tableClientCustomer = new TableClient(connectionString, tableNameCustomer);
            //product
            _tableClientProduct = new TableClient(connectionString, tableNameProduct);
        }
        //------------------------------------------------------------------------------------------//
        //Initialize
        public async Task InitializeAsync()
        {
            await _tableClientCustomer.CreateIfNotExistsAsync();
            await _tableClientProduct.CreateIfNotExistsAsync();
        }
        //------------------------------------------------------------------------------------------//
        //Insert a customer
        public async Task InsertCustomerAsync(Customer customer)
        {
            await _tableClientCustomer.AddEntityAsync(customer);
        }
        //------------------------------------------------------------------------------------------//
        //List customers
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var customer = new List<Customer>();

            await foreach (Customer entity in _tableClientCustomer.QueryAsync<Customer>())
            {
                customer.Add(entity);
            }

            return customer;
        }
        //------------------------------------------------------------------------------------------//
        // GET a single customer by PartitionKey and RowKey
        public async Task<Customer> GetCustomerAsync(string rowKey)
        {
            // PartitionKey is hardcoded to "Customer" in your Customer model
            return await _tableClientCustomer.GetEntityAsync<Customer>("Customer", rowKey);
        }
        //------------------------------------------------------------------------------------------//
        // UPDATE a customer
        public async Task UpdateCustomerAsync(Customer customer)
        {
            // Upsert will add the entity if it doesn't exist, or replace it if it does.
            // Replace means all properties are replaced.
            await _tableClientCustomer.UpsertEntityAsync(customer, TableUpdateMode.Replace);
        }
        //------------------------------------------------------------------------------------------//
        // DELETE a customer
        public async Task DeleteCustomerAsync(string rowKey)
        {
            // PartitionKey is hardcoded to "Customer" in your Customer model
            await _tableClientCustomer.DeleteEntityAsync("Customer", rowKey);
        }
        //------------------------------------------------------------------------------------------//
        //Insert a product
        public async Task InsertProductAsync(Product product)
        {
            await _tableClientProduct.AddEntityAsync(product);
        }
        //------------------------------------------------------------------------------------------//
        //List products
        public async Task<List<Product>> GetAllProductAsync()
        {
            var product = new List<Product>();

            await foreach (Product entity in _tableClientProduct.QueryAsync<Product>())
            {
                product.Add(entity);
            }

            return product;
        }
        //------------------------------------------------------------------------------------------//
        // GET a single product by PartitionKey and RowKey
        public async Task<Product> GetProductAsync(string rowKey)
        {
            // PartitionKey is hardcoded to "Product" in your Product model
            return await _tableClientProduct.GetEntityAsync<Product>("Product", rowKey);
        }
        //------------------------------------------------------------------------------------------//
        // UPDATE a product
        public async Task UpdateProductAsync(Product product)
        {
            // Upsert will add the entity if it doesn't exist, or replace it if it does.
            await _tableClientProduct.UpsertEntityAsync(product, TableUpdateMode.Replace);
        }
        //------------------------------------------------------------------------------------------//
        // DELETE a product
        public async Task DeleteProductAsync(string rowKey)
        {
            // PartitionKey is hardcoded to "Product" in your Product model
            await _tableClientProduct.DeleteEntityAsync("Product", rowKey);
        }
    }
}
//---------------------END OF FILE------------------------------------------------------------------//