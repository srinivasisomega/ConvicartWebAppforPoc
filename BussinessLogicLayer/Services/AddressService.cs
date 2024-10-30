using ConvicartWebApp.BussinessLogicLayer.Interface;
using ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface;
using ConvicartWebApp.DataAccessLayer.Models;
using ConvicartWebApp.PresentationLayer.ViewModels;
namespace ConvicartWebApp.BussinessLogicLayer.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ICustomerRepository _customerRepository;

        public AddressService(IAddressRepository addressRepository, ICustomerRepository customerRepository)
        {
            _addressRepository = addressRepository;
            _customerRepository = customerRepository;
        }

        public async Task SaveOrUpdateAddressAsync(int customerId, AddressViewModel viewModel)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new Exception("Customer not found.");
            }

            var existingAddress = await _addressRepository.GetAddressByCustomerIdAsync(customerId);

            if (existingAddress != null)
            {
                existingAddress.StreetAddress = viewModel.StreetAddress;
                existingAddress.City = viewModel.City;
                existingAddress.State = viewModel.State;
                existingAddress.PostalCode = viewModel.PostalCode;
                existingAddress.Country = viewModel.Country;

                _addressRepository.Update(existingAddress);
            }
            else
            {
                var newAddress = new Address
                {
                    AddressId = customerId,
                    StreetAddress = viewModel.StreetAddress,
                    City = viewModel.City,
                    State = viewModel.State,
                    PostalCode = viewModel.PostalCode,
                    Country = viewModel.Country
                };

                await _addressRepository.AddAsync(newAddress);
                customer.AddressId = newAddress.AddressId;
                _customerRepository.Update(customer);
            }

            await _addressRepository.SaveChangesAsync();
        }
    }

}