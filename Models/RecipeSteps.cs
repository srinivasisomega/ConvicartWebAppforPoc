using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ConvicartWebApp.Models
{
    public class RecipeSteps
    {
            // Foreign key to the Product
            [ForeignKey("Product")]
            public int ProductId { get; set; }

            // Step number, must be provided and part of the composite key
            [Key, Column(Order = 1)] // Part of composite key
            [Required] // Not null
            public int StepNo { get; set; }

            // Description of the step
            public string StepDescription { get; set; }

            // URL for the step's image
            public string ImageUrl { get; set; }
            public virtual Store Product { get; set; }
        
    }
}
