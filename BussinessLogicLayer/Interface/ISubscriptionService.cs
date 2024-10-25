namespace ConvicartWebApp.BussinessLogicLayer.Interface
{
    public interface ISubscriptionService
    {
        bool UpdateSubscription(string subscriptionType, int days, decimal amount, int customerId, out string errorMessage);
    }

}
