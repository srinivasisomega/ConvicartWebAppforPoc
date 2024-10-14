namespace ConvicartWebApp.Models
{
    public class CustomerPreference
    {
        public string CustomerId { get; set; }
        public int PreferenceId { get; set; }

        public Customer Customer { get; set; }
        public Preference Preference { get; set; }
    }
}
