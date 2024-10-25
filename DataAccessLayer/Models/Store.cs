using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvicartWebApp.DataAccessLayer.Models
{
    public class Store
    {
        [Key]
        public int ProductId { get; set; }

        [MaxLength(255)]
        public string? ProductName { get; set; }

        [MaxLength(1000)] // Limit product description length
        public string? ProductDescription { get; set; }  // New column for product description

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

        public string? imgUrl { get; set; }

        [Column(TypeName = "VARBINARY(MAX)")]
        public byte[]? ProductImage { get; set; }

        [Range(1, 5)]
        public int? Rating { get; set; }

        public Preference? Preference { get; set; }
    }
}
