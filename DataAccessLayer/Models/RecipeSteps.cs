using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ConvicartWebApp.DataAccessLayer.Models
{
    public class RecipeSteps
    {
        [Key]
        public int Id { get; set; }
        public int? ProductId { get; set; } // Foreign key to Products table

        public int? StepNumber { get; set; } // Step ID for each step in a recipe        
        [MaxLength(1000)]
        public string? StepDescription { get; set; } // Description of the step
        [Column(TypeName = "VARBINARY(MAX)")]
        public byte[]? Stepimage { get; set; }
        // Navigation property
        public virtual Store? Product { get; set; }
    }
}
