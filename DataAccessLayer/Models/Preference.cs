using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ConvicartWebApp.DataAccessLayer.Models
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
        [Column(TypeName = "VARBINARY(MAX)")]
        public byte[]? PreferenceImage { get; set; }
    }

}
