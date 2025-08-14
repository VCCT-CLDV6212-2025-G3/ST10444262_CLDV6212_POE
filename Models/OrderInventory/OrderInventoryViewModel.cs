namespace ST10444262_CLDV6212_POE.Models.OrderInventory
{
    public class OrderInventoryViewModel
    {
        public OrderViewModel Order { get; set; } = new();
        public InventoryViewModel Inventory { get; set; } = new();
        public string? Message { get; set; }  
        public string? PeekedOrderMessage { get; set; }
        public string? PeekedInventoryMessage { get; set; }
    }

}
