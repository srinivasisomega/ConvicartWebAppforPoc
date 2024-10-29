namespace ConvicartWebApp.PresentationLayer.ViewModels
{
    public class SubscriptionViewModel
    {
        public int CustomerId { get; set; }
        public string CurrentSubscriptionType { get; set; }
        public string CurrentSubscriptionDuration { get; set; } // "Month" or "Year"
    }


}
