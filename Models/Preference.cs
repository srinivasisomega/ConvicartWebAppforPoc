using System.ComponentModel.DataAnnotations;
namespace ConvicartWebApp.Models
{
    public class Preference
    {
        [Key]
        public int PreferenceId { get; set; }

        [MaxLength(255)]
        public string? PreferenceName { get; set; }
        [MaxLength(255)]
        public string? PreferenceDescription { get; set; }
        [MaxLength(255)]
        public string? ImageURLCusine { get; set; }
    }

}
