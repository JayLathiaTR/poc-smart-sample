namespace TR.SmartSample.BlazorWebUI.Models
{
    public class InventoryDetails
    {
        public required string Product { get; set; }
        public int Inventory { get; set; }
        public required string Price { get; set; }
        public required string CurrentMarketShare { get; set; }
    }
}
