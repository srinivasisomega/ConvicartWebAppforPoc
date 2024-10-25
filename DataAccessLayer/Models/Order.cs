using System.ComponentModel.DataAnnotations;
namespace ConvicartWebApp.DataAccessLayer.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; } // Final total after deductions
        public DateTime OrderDate { get; set; }

        public string? Status { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

}
