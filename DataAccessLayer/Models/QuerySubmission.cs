using System.ComponentModel.DataAnnotations;

namespace ConvicartWebApp.DataAccessLayer.Models
{
    public enum QueryStatus
    {
        Unseen,
        ResolvingQuery,
        Resolved
    }
    public class QuerySubmission
    {
        [Key]
        public int Id { get; set; }
        // Name field with required validation
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
        public string Name { get; set; }

        // Mobile field with required validation and regex for mobile number format
        [Required(ErrorMessage = "Mobile number is required.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Mobile number must be a 10-digit number.")]
        public string Mobile { get; set; }

        // Email field with required validation and email format
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        // Description field with optional validation
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string Description { get; set; }
        public QueryStatus Status { get; set; } = QueryStatus.Unseen;

    }
}

