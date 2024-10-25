using ConvicartWebApp.BussinessLogicLayer.Interface;

namespace ConvicartWebApp.BussinessLogicLayer.Services
{
    public class PointsService : IPointsService
    {
        private const decimal PointsToCurrencyRate = 20; // 1 point = 0.10 currency unit

        public decimal CalculateAmountToPay(int points)
        {
            return points * PointsToCurrencyRate;
        }
    }
}
