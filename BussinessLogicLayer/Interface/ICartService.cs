using ConvicartWebApp.PresentationLayer.ViewModels;
namespace ConvicartWebApp.BussinessLogicLayer.Interface
{
    public interface ICartService
    {
        bool AddToCartMain(int customerId, int productId, int quantity);
        bool RemoveFromCartMain(int customerId, int productId, int quantity);
        bool AddToCart(int customerId, int productId, int quantity);
        bool RemoveFromCart(int customerId, int productId, int quantity);
        CartViewModel GetCartDetails(int customerId);
    }
}

