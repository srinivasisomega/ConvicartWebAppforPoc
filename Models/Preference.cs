using System.ComponentModel.DataAnnotations;
namespace ConvicartWebApp.Models
{
    public class Preference
    {
        [Key]
        public int PreferenceId { get; set; }

        [MaxLength(255)]
        public string PreferenceName { get; set; }
    }

}
