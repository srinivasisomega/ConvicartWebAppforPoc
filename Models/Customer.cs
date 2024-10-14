using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace ConvicartWebApp.Models
{
   

    public class Customer:IdentityUser
    {

        [MaxLength(255)]
        [Required]
        public override string? UserName { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Mobile number must be a 10-digit number.")]
        public string? Number { get; set; }

        [MaxLength(255)]
        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format")]
        public override string? Email { get; set; }
        [Range(1, int.MaxValue)]
        public int? Age { get; set; }

        [RegularExpression("M|F|O")]
        public char? Gender { get; set; }

        public int? AddressId { get; set; }

        public int PointBalance { get; set; }

        [MaxLength(255)]
        public string? ProfilePicUrl { get; set; }

        public Address? Address { get; set; }
    }

}
