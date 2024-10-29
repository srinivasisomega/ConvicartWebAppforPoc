using ConvicartWebApp.PresentationLayer.ViewModels;

namespace ConvicartWebApp.BussinessLogicLayer.Interface
{
    public interface ISubscriptionService
    {
        Task<SubscriptionViewModel> GetCustomerSubscriptionAsync(int customerId);

        bool UpdateSubscription(string subscriptionType, int days, decimal amount, int customerId, out string errorMessage);
    }

}
