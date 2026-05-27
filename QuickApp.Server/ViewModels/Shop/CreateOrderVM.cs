using System.ComponentModel.DataAnnotations;
using QuickApp.Server.Attributes;

namespace QuickApp.Server.ViewModels.Shop
{
    public class CreateOrderVM
    {
        [Range(0, double.MaxValue)]
        public decimal Discount { get; set; }

        [StringLength(500)]
        public string? Comments { get; set; }

        public string? CashierId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "CustomerId must be a positive integer.")]
        public int CustomerId { get; set; }

        [Required]
        [MinimumCount(1, ErrorMessage = "At least one order detail is required.")]
        public List<CreateOrderDetailVM> OrderDetails { get; set; } = [];
    }

    public class CreateOrderDetailVM
    {
        [Range(0, double.MaxValue, ErrorMessage = "UnitPrice must be non-negative.")]
        public decimal UnitPrice { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Discount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be a positive integer.")]
        public int ProductId { get; set; }
    }
}
