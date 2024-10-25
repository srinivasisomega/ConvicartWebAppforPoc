using ConvicartWebApp.DataAccessLayer.Models;
namespace ConvicartWebApp.PresentationLayer.ViewModels
{
    public class CartViewModel
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public Dictionary<int, string> ProductNames { get; set; } = new Dictionary<int, string>();

        public decimal TotalAmount => CartItems.Sum(item => item.TotalPrice); // Total before discount
        public decimal Discount { get; set; } // Discount rate
        public decimal TaxRate { get; set; }  // Tax rate
        public decimal ShippingCost { get; set; }  // Shipping cost
        public decimal DiscountedSubtotal { get; set; }  // Subtotal after applying discount
        public decimal TaxAmount { get; set; }  // Calculated tax amount
        public decimal FinalTotal { get; set; }  // Final total after tax, shipping, and discount
    }





}
