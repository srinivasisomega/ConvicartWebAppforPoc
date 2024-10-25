namespace ConvicartWebApp.BussinessLogicLayer.Interface
{
    public interface IPasswordResetService
    {
        void GenerateAndSendResetCode(string email);
        bool ValidateResetCode(string email, string code); // New method
        void RemoveUsedResetCode(string email); // Add this method to the interface

    }
}
