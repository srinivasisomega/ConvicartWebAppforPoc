namespace ConvicartWebApp.Models
{
    public class CartViewModel
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        // Add a dictionary to store ProductId and ProductName for easy lookup
        public Dictionary<int, string> ProductNames { get; set; } = new Dictionary<int, string>();

        public decimal TotalAmount => CartItems.Sum(item => item.TotalPrice); // Calculate total amount based on items in the cart
    }


}
