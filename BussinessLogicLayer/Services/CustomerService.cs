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

        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            return await _repository.GetByIdAsync(customerId);
        }

        public Customer? GetCustomerById(int? customerId)
        {
            if (customerId == null) return null;

            // Synchronously fetch customer by ID
            return _repository.GetByIdAsync(customerId.Value).Result;
        }

        public async Task<Customer?> GetCustomerWithAddressByIdAsync(int customerId)
        {
            return await _repository.GetCustomerWithAddressByIdAsync(customerId);
        }

        public async Task UpdateCustomerProfileAsync(Customer customer)
        {
            var existingCustomer = await _repository.GetByIdAsync(customer.CustomerId);
            if (existingCustomer != null)
            {
                existingCustomer.Name = customer.Name;
                existingCustomer.Number = customer.Number;
                existingCustomer.Email = customer.Email;
                existingCustomer.Age = customer.Age;
                existingCustomer.Gender = customer.Gender;

                _repository.Update(existingCustomer);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task<Customer?> GetCustomerByEmailAndPasswordAsync(string email, string password)
        {
            return await _repository.GetCustomerByEmailAndPasswordAsync(email, password);
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
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
            _repository.Update(customer);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SaveProfileImageAsync(IFormFile image, int customerId)
        {
            var customer = await _repository.GetByIdAsync(customerId);
            if (customer == null || image == null || image.Length == 0) return false;

            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                customer.ProfileImage = memoryStream.ToArray();
                _repository.Update(customer);
                await _repository.SaveChangesAsync();
            }

            return true;
        }

        public void AddPointsToCustomer(Customer customer, int points)
        {
            customer.PointBalance += points;
            _repository.Update(customer);
            _repository.SaveChangesAsync().Wait();  // Synchronously save changes
        }
    }

}
