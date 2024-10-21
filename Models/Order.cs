using System.ComponentModel.DataAnnotations;
namespace ConvicartWebApp.Models
{    public enum OrderStatus
    {
        OrderPlaced,
        DeliveryInProgress,
        Delivered
    }

    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; } // Final total after deductions
        public DateTime OrderDate { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.OrderPlaced; // Default status
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

}
