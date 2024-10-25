using ConvicartWebApp.BussinessLogicLayer.Interface;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace ConvicartWebApp.BussinessLogicLayer.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly ConvicartWarehouseContext _context;

        public RecipeService(ConvicartWarehouseContext context)
        {
            _context = context;
        }

        

        public async Task<List<RecipeSteps>> GetRecipeStepsByProductIdAsync(int productId)
        {
            return await _context.RecipeSteps
                .Where(s => s.ProductId == productId)
                .OrderBy(s => s.StepNumber)
                .ToListAsync();
        }
    }

}
