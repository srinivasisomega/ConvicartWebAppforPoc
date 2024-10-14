using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace ConvicartWebApp.Models
{
    public class ApplicationRole : IdentityRole
    {
        [Required]
        [MaxLength(20)]
        [RegularExpression("Bronze|Silver|Gold", ErrorMessage = "Subscription must be either 'Bronze', 'Silver', or 'Gold'.")]
        public override string Name { get; set; } = "Bronze";  // Default subscription type

        // Add more fields if necessary
    }

}
