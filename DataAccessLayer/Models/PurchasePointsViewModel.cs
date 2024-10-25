using System.ComponentModel.DataAnnotations;

namespace ConvicartWebApp.DataAccessLayer.Models
{
    public class PurchasePointsViewModel
    {
        [Required]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Please specify the number of points you wish to purchase.")]
        [Range(1, 1000, ErrorMessage = "You can purchase between 1 and 1000 points at a time.")]
        public int PointsToPurchase { get; set; }

        [Display(Name = "Amount to Pay")]
        public decimal AmountToPay { get; set; }

        // Optional confirmation message after purchase
        public string? ConfirmationMessage { get; set; }
    }
}
