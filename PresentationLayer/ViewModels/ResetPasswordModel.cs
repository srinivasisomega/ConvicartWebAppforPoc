using System.ComponentModel.DataAnnotations;

namespace ConvicartWebApp.PresentationLayer.ViewModels
{
    public class ResetPasswordModel
    {
        public string Email { get; set; }
        public string Code { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

}
