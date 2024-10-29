using ConvicartWebApp.BussinessLogicLayer.Interface;
using ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace ConvicartWebApp.BussinessLogicLayer.Services
{
    // CustomerService.cs
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            return await _repository.GetCustomerByIdAsync(customerId);
        }
        public Customer GetCustomerById(int? customerId)
        {
            if (customerId == null) return null;

            // Since we have an async version of this method, we can call it synchronously here.
            return _repository.GetCustomerByIdAsync(customerId.Value).Result;
        }


        public async Task<Customer> GetCustomerWithAddressByIdAsync(int customerId)
        {
            return await _repository.GetCustomerWithAddressByIdAsync(customerId);
        }

        public async Task UpdateCustomerProfileAsync(Customer customer)
        {
            var existingCustomer = await _repository.GetCustomerByIdAsync(customer.CustomerId);
            if (existingCustomer != null)
            {
                existingCustomer.Name = customer.Name;
                existingCustomer.Number = customer.Number;
                existingCustomer.Email = customer.Email;
                existingCustomer.Age = customer.Age;
                existingCustomer.Gender = customer.Gender;

                await _repository.UpdateCustomerAsync(existingCustomer);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task<Customer> GetCustomerByEmailAndPasswordAsync(string email, string password)
        {
            return await _repository.GetCustomerByEmailAndPasswordAsync(email, password);
        }

        public async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            return await _repository.GetCustomerByEmailAsync(email);
        }

        public bool VerifyPassword(Customer? customer, string currentPassword)
        {
            return customer?.Password == currentPassword;
        }

        public async Task<bool> ChangePasswordAsync(Customer? customer, string newPassword)
        {
            if (customer == null) return false;

            customer.Password = newPassword;
            await _repository.UpdateCustomerAsync(customer);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SaveProfileImageAsync(IFormFile image, int customerId)
        {
            var customer = await _repository.GetCustomerByIdAsync(customerId);
            if (customer == null || image == null || image.Length == 0) return false;

            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                customer.ProfileImage = memoryStream.ToArray();
                await _repository.UpdateCustomerAsync(customer);
                await _repository.SaveChangesAsync();
            }

            return true;
        }

        // Implement AddPointsToCustomer as a synchronous method
        public void AddPointsToCustomer(Customer customer, int points)
        {
            customer.PointBalance += points;
            _repository.UpdateCustomerAsync(customer).Wait();  // Synchronously save changes
            _repository.SaveChangesAsync().Wait();
        }
    }
}
