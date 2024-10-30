using ConvicartWebApp.DataAccessLayer.Models;
using ConvicartWebApp.PresentationLayer.ViewModels;
namespace ConvicartWebApp.BussinessLogicLayer.Interface
{
    
    public interface IOrderService
    {
        Task<List<Order>> GetOrderHistoryAsync(int customerId);
        bool Purchase(int customerId, CartViewModel cartViewModel);
        Task<bool> CancelOrderAsync(int orderId, int customerId);

    }

}
