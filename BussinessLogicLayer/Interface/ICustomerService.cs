using ConvicartWebApp.DataAccessLayer.Models;

namespace ConvicartWebApp.BussinessLogicLayer.Interface
{
    // ICustomerService.cs
    public interface ICustomerService
    {
        Customer GetCustomerById(int? customerId);
        void AddPointsToCustomer(Customer customer, int points);
        Task<Customer> GetCustomerByEmailAndPasswordAsync(string email, string password);
        Task<Customer> GetCustomerByEmailAsync(string email); // New method
        bool VerifyPassword(Customer? customer, string currentPassword);
        Task<bool> ChangePasswordAsync(Customer? customer, string newPassword);
        Task<Customer> GetCustomerByIdAsync(int customerId);
        Task<Customer> GetCustomerWithAddressByIdAsync(int customerId);
        Task UpdateCustomerProfileAsync(Customer customer);
        Task<bool> SaveProfileImageAsync(IFormFile image, int customerId);
    }

    

}
