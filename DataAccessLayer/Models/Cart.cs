using System.ComponentModel.DataAnnotations;

namespace ConvicartWebApp.DataAccessLayer.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }

}
