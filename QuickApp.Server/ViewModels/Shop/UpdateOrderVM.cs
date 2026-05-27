using System.ComponentModel.DataAnnotations;

namespace QuickApp.Server.ViewModels.Shop
{
    public class UpdateOrderVM
    {
        [Range(0, double.MaxValue)]
        public decimal Discount { get; set; }

        [StringLength(500)]
        public string? Comments { get; set; }
    }
}
