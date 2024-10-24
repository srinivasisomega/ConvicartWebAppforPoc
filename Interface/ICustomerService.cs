using ConvicartWebApp.Models;

namespace ConvicartWebApp.Interface
{
    // ICustomerService.cs
    public interface ICustomerService
    {
        Customer GetCustomerById(int customerId);
        void AddPointsToCustomer(Customer customer, int points);

        // New asynchronous method to get a customer by email and password
        Task<Customer> GetCustomerByEmailAndPasswordAsync(string email, string password);
    }




}
