using ConvicartWebApp.BussinessLogicLayer.Interface;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
namespace ConvicartWebApp.BussinessLogicLayer.Services
{
    public class PreferenceService : IPreferenceService
    {
        private readonly ConvicartWarehouseContext _context;

        public PreferenceService(ConvicartWarehouseContext context)
        {
            _context = context;
        }

        public async Task<List<Preference>> GetPreferencesAsync()
        {
            return await _context.Preferences.ToListAsync().ConfigureAwait(false);
        }
        public async Task<List<CustomerPreference>> GetCustomerPreferencesListAsync(int? customerId)
        {
            return await _context.CustomerPreferences
                .Where(cp => cp.CustomerId == customerId)
                .Include(cp => cp.Preference)
                .ToListAsync();
        }
        public async Task<List<int>> GetCustomerPreferencesAsync(int customerId)
        {
            return await _context.CustomerPreferences
                .Where(cp => cp.CustomerId == customerId)
                .Select(cp => cp.PreferenceId)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task UpdateCustomerPreferencesAsync(int customerId, List<int> selectedPreferences)
        {
            var existingPreferences = await _context.CustomerPreferences
                .Where(cp => cp.CustomerId == customerId)
                .ToListAsync()
                .ConfigureAwait(false);

            _context.CustomerPreferences.RemoveRange(existingPreferences);

            if (selectedPreferences != null && selectedPreferences.Any())
            {
                foreach (var preferenceId in selectedPreferences)
                {
                    var customerPreference = new CustomerPreference
                    {
                        CustomerId = customerId,
                        PreferenceId = preferenceId
                    };
                    _context.CustomerPreferences.Add(customerPreference);
                }
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }

}
