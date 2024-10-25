namespace ConvicartWebApp.BussinessLogicLayer.Interface
{
    public interface IEmailService
    {
        void SendResetCodeEmail(string email, string resetCode);
    }

}
