using System.ComponentModel.DataAnnotations;
namespace ConvicartWebApp.Models
{
   
    public class Diet
    {
        [Key]
        public int DietId { get; set; }

        [MaxLength(255)]
        public string DietName { get; set; }
    }

}
