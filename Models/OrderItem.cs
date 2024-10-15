using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvicartWebApp.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        // Foreign key to Order
        public int OrderId { get; set; }

        // Navigation property to the Order entity
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        // Foreign key to Product (Store)
        public int ProductId { get; set; }

        // Navigation property to the Store entity
        [ForeignKey("ProductId")]
        public Store Product { get; set; }

        // Other properties
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // Calculated property for total price
        public decimal TotalPrice => Price * Quantity;
    }


}
