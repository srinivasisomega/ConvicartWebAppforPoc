namespace ConvicartWebApp.Models
{
    public class CustomerDiet
    {
        public int CustomerId { get; set; }
        public int DietId { get; set; }

        public Customer Customer { get; set; }
        public Diet Diet { get; set; }
    }

}
