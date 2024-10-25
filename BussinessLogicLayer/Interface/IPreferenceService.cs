using ConvicartWebApp.DataAccessLayer.Models;

namespace ConvicartWebApp.BussinessLogicLayer.Interface
{
    public interface IPreferenceService
    {
        Task<List<Preference>> GetPreferencesAsync();
        Task<List<int>?> GetCustomerPreferencesAsync(int customerId);
        Task UpdateCustomerPreferencesAsync(int customerId, List<int> selectedPreferences);
        Task<List<CustomerPreference>> GetCustomerPreferencesListAsync(int? customerId);

    }

}
