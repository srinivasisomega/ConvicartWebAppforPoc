using System.ComponentModel.DataAnnotations;

namespace ConvicartWebApp.ViewModels
{
        public class QuerySubmissionViewModel
        {
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Description { get; set; }

            [Phone]
            public string Mobile { get; set; }
        }
}
