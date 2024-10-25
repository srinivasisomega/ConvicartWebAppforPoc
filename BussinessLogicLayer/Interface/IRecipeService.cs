using ConvicartWebApp.DataAccessLayer.Models;

namespace ConvicartWebApp.BussinessLogicLayer.Interface
{
    public interface IRecipeService
    {
        Task<List<RecipeSteps>> GetRecipeStepsByProductIdAsync(int productId);
    }

}
