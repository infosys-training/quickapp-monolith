namespace QuickApp.Server.ViewModels.Shop
{
    public class CreateOrderVM
    {
        public decimal Discount { get; set; }
        public string? Comments { get; set; }
        public string? CashierId { get; set; }
        public int CustomerId { get; set; }
        public List<CreateOrderDetailVM> OrderDetails { get; set; } = [];
    }

    public class CreateOrderDetailVM
    {
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public int ProductId { get; set; }
    }
}
