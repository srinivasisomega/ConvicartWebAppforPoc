using ConvicartWebApp.BussinessLogicLayer.Interface;

namespace ConvicartWebApp.BussinessLogicLayer.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IEmailService _emailService;
        private readonly Dictionary<string, string> _resetCodes = new Dictionary<string, string>();

        public PasswordResetService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public void GenerateAndSendResetCode(string email)
        {
            var resetCode = new Random().Next(100000, 999999).ToString();
            _resetCodes[email] = resetCode;
            _emailService.SendResetCodeEmail(email, resetCode);
        }
        public bool ValidateResetCode(string email, string code)
        {
            return _resetCodes.TryGetValue(email, out var storedCode) && storedCode == code;
        }

        public void RemoveUsedResetCode(string email)
        {
            _resetCodes.Remove(email);
        }
    }
}
