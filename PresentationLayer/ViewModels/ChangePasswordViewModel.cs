using System.ComponentModel.DataAnnotations;

namespace ConvicartWebApp.PresentationLayer.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$",
            ErrorMessage = "Password must be at least 8 characters long, include an uppercase letter, a lowercase letter, a number, and a special character.")]
        public string NewPassword { get; set; }
    }

}
