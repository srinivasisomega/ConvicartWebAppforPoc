using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvicartWebApp.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Store Product { get; set; }

        public string imgUrl { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice => Price * Quantity; // Calculated total price
    }



}
