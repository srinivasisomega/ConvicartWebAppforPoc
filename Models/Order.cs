using System.ComponentModel.DataAnnotations;
namespace ConvicartWebApp.Models
{
   

    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public Customer Customer { get; set; }

        public Store Product { get; set; }
    }

}
