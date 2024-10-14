namespace ConvicartWebApp.Models
{
    public class CustomerProfileViewModel
    {
        public Customer Customer { get; set; }
        public ApplicationUserRole UserRole { get; set; }
        public ApplicationRole Role { get; set; }
        public Address Address { get; set; }
        public List<Preference> Preferences { get; set; }
        public List<CustomerPreference> CustomerPreferences { get; set; }
    }
}

