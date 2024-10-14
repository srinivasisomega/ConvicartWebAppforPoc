using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace ConvicartWebApp.Models
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public DateTime SubscriptionStartDate { get; set; } = DateTime.UtcNow;

    }

}
