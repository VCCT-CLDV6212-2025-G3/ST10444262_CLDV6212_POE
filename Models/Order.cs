using System.ComponentModel.DataAnnotations;
using Azure;
using Azure.Data.Tables;

namespace ST10444262_CLDV6212_POE.Models
{
    public class Order : ITableEntity
    {
        //Table properties
        public string PartitionKey { get; set; } = "Order";
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        //------------------------------------------------------------------------------------------//
        //Order properties
        [Display(Name = "Order ID")]
        public int OrderID { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Time of Order")]
        public DateTime OrderDate { get; set; }
    }
}
//---------------------END OF FILE------------------------------------------------------------------//