namespace ConvicartWebApp.DataAccessLayer.Models
{
    public class CustomerPreference
    {
        public int CustomerId { get; set; }
        public int PreferenceId { get; set; }

        public Customer Customer { get; set; }
        public Preference Preference { get; set; }
    }
}
