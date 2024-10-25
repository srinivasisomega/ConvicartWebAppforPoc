using ConvicartWebApp.DataAccessLayer.Models;

namespace ConvicartWebApp.PresentationLayer.ViewModels
{
    public class CustomerProfileViewModel
    {
        public Customer Customer { get; set; }
        public Address Address { get; set; }
        public List<Preference> Preferences { get; set; }
        public List<CustomerPreference> CustomerPreferences { get; set; }
    }
}

