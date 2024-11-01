using System.ComponentModel.DataAnnotations;

namespace ConvicartWebApp.ViewModels
{
    public class SignUpViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Phone]
        public string Number { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
