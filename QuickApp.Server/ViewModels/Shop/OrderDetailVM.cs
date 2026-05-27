namespace QuickApp.Server.ViewModels.Shop
{
    public class OrderDetailVM
    {
        public int Id { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
    }
}
