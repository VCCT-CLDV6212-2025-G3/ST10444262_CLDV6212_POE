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
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        [Display(Name = "Product Description")]
        public string ProductDescription { get; set; }
        [Display(Name = "Product Price")]
        public double ProductPrice { get; set; }
        public string ProductFileName { get; set; }
        public string ProductFileUrl { get; set; }

        [Display(Name = "Product Number")]
        public string ProductID
        {
            get => RowKey;
            set => RowKey = value;
        }
    }

}
//---------------------END OF FILE------------------------------------------------------------------//
