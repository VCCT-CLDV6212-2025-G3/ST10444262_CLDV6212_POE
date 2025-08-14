using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Azure;
using Azure.Data.Tables;

namespace ST10444262_CLDV6212_POE.Models
{
    public class Product : ITableEntity
    {
        //Table properties
        public string PartitionKey { get; set; } = "Product";
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        //------------------------------------------------------------------------------------------//
        //Product Properties
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public double ProductPrice { get; set; }
        [Display(Name = "Product Number")]
        public string ProductID
        {
            get => RowKey;
            set => RowKey = value;
        }
    }

}
//---------------------END OF FILE------------------------------------------------------------------//
