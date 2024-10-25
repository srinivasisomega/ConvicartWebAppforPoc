using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ConvicartWebApp.DataAccessLayer.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [MaxLength(255)]
        [Required]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Mobile number must be a 10-digit number.")]
        public string? Number { get; set; }

        [MaxLength(255)]
        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [MaxLength(255)]
        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$", ErrorMessage = "Password must be at least 8 characters long, include an uppercase letter, a lowercase letter, a number, and a special character.")]
        public string? Password { get; set; }

        [MaxLength(20)]
        [RegularExpression("Bronze|Silver|Gold")]
        public string? Subscription { get; set; }
        public DateTime? SubscriptionDate { get; set; }

        [Range(1, int.MaxValue)]
        public int? Age { get; set; }

        [RegularExpression("M|F|O")]
        public char? Gender { get; set; }

        public int? AddressId { get; set; }

        public int PointBalance { get; set; }

        [Column(TypeName = "VARBINARY(MAX)")]
        public byte[]? ProfileImage { get; set; }

        public DateTime? LastPointsAddedDate { get; set; }
        public Address? Address { get; set; }
    }

}
