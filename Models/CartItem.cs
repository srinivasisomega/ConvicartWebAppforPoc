using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvicartWebApp.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        // Foreign key to the Store table
        public int ProductId { get; set; }

        // Navigation property to the Store entity
        [ForeignKey("ProductId")]
        public Store Product { get; set; }

        public int Quantity { get; set; }

        // Total price calculation based on the product's price and quantity
        public decimal TotalPrice => Product.Price * Quantity;
    }


}
