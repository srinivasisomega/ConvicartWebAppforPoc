using System.ComponentModel.DataAnnotations;
namespace ConvicartWebApp.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Store
    {
        [Key]
        public int ProductId { get; set; }

        [MaxLength(255)]
        public string? ProductName { get; set; }

        [Column(TypeName = "decimal(10, 2)")] // Specify precision and scale
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(5, 2)")] // Specify precision and scale
        public decimal Carbs { get; set; }

        [Column(TypeName = "decimal(5, 2)")] // Specify precision and scale
        public decimal Proteins { get; set; }

        [Column(TypeName = "decimal(5, 2)")] // Specify precision and scale
        public decimal Vitamins { get; set; }

        [Column(TypeName = "decimal(5, 2)")] // Specify precision and scale
        public decimal Minerals { get; set; }

        public TimeSpan CookTime { get; set; }

        public TimeSpan PrepTime { get; set; }

        [MaxLength(20)]
        [RegularExpression("Easy|Medium|Hard")]
        public string? Difficulty { get; set; }
        public int? PreferenceId { get; set; }

        [Range(1, 5)]
        public int? Rating { get; set; }
        public Preference? Preference { get; set; }
    }


}
