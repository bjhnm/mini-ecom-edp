namespace mvvm_edp.Models
{
    public class OrderItemDisplay
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = "";
        public string ProductName { get; set; } = "";
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total => ProductPrice * Quantity;

        public OrderItemDisplay() { }
    }
}