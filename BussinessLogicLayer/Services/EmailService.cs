using System.Net.Mail;
using ConvicartWebApp.BussinessLogicLayer.Interface;

namespace ConvicartWebApp.BussinessLogicLayer.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpHost = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "convicart6@gmail.com";
        private readonly string _smtpPass = "vrey nwgz rnln xvzk";

        public void SendResetCodeEmail(string email, string resetCode)
        {
            var smtpClient = new SmtpClient(_smtpHost)
            {
                Port = _smtpPort,
                Credentials = new System.Net.NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = "Password Reset Code",
                Body = $"Your password reset code is: {resetCode}",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);
            smtpClient.Send(mailMessage);
        }
    }

}
