using System.ComponentModel.DataAnnotations;
namespace ConvicartWebApp.Models
{
   

    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(20)]
        public string Number { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(255)]
        public string Password { get; set; }

        [MaxLength(20)]
        [RegularExpression("Bronze|Silver|Gold")]
        public string Subscription { get; set; }
        public DateTime? SubscriptionDate { get; set; }

        [Range(1, int.MaxValue)]
        public int? Age { get; set; }

        [RegularExpression("M|F|O")]
        public char? Gender { get; set; }

        public int? AddressId { get; set; }

        public int PointBalance { get; set; }

        [MaxLength(255)]
        public string ProfilePicUrl { get; set; }

        public int? DietId { get; set; }

        public Address Address { get; set; }
    }

}
