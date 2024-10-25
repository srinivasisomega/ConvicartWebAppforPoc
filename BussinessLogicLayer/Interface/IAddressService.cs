using ConvicartWebApp.PresentationLayer.ViewModels;

namespace ConvicartWebApp.BussinessLogicLayer.Interface
{
    

    public interface IAddressService
    {
        Task SaveOrUpdateAddressAsync(int customerId, AddressViewModel viewModel);
    }


}
