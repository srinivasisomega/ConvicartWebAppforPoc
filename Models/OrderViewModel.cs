namespace ConvicartWebApp.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ImageUrl { get; set; } // To store the image URL from the first order item
    }

}
