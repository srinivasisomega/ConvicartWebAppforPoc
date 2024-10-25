using System.ComponentModel.DataAnnotations;
namespace ConvicartWebApp.DataAccessLayer.Models
{

    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        [MaxLength(255)]
        public string StreetAddress { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [MaxLength(100)]
        public string State { get; set; }

        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string Country { get; set; }
    }


}
