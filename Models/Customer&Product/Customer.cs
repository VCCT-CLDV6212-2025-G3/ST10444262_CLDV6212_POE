using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Azure;
using Azure.Data.Tables;

namespace ST10444262_CLDV6212_POE.Models
{
    public class Customer : ITableEntity
    {
        //Table properties
        public string PartitionKey { get; set; } = "Customer";
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        //------------------------------------------------------------------------------------------//
        //Customer properties
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }

        [Display(Name = "Customer Id")]
        public string CustomerId
        {
            get => RowKey;
            set => RowKey = value;
        }

    }
}
//---------------------END OF FILE------------------------------------------------------------------//