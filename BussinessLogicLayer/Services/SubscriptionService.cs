using ConvicartWebApp.BussinessLogicLayer.Interface;
using ConvicartWebApp.DataAccessLayer.Data;

namespace ConvicartWebApp.BussinessLogicLayer.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ConvicartWarehouseContext _context;
        private readonly ICustomerService CustomerService;

        public SubscriptionService(ConvicartWarehouseContext context, ICustomerService customerService)
        {
            _context = context;
            CustomerService = customerService;
        }

        public bool UpdateSubscription(string subscriptionType, int days, decimal amount, int customerId, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Validate subscription type
            if (!new[] { "Bronze", "Silver", "Gold" }.Contains(subscriptionType))
            {
                errorMessage = "Invalid subscription type.";
                return false;
            }

            // Fetch the customer from the database using customerId
            var customer = CustomerService.GetCustomerById(customerId);
            if (customer == null)
            {
                errorMessage = "Customer not found.";
                return false;
            }

            // Calculate the new subscription end date
            DateTime currentDate = DateTime.Now;
            DateTime newSubscriptionEndDate = currentDate.AddDays(days);

            // Calculate points based on the amount
            int points = (int)(amount / 20);

            // Update the customer's subscription and points balance
            customer.Subscription = subscriptionType;
            customer.SubscriptionDate = newSubscriptionEndDate;
            customer.PointBalance += points;

            try
            {
                // Save changes to the database
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                errorMessage = "An error occurred while updating the subscription.";
                return false;
            }
        }
    }

}
