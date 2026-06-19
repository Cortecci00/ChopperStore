using System.Net;
using System.Net.Mail;

namespace ChopperStoreAngularTest.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPassword = _configuration["Email:SmtpPassword"];

            using var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPassword),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress(smtpUser!, "ChopperStore"),
                Subject = "Recuperar contraseña - ChopperStore",
                Body = $@"
                    <p>Recibimos una solicitud para restablecer tu contraseña en ChopperStore.</p>
                    <p><a href=""{resetLink}"">Click aquí para crear una nueva contraseña</a></p>
                    <p>El link expira en 1 hora. Si no fuiste vos, podés ignorar este correo.</p>",
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            await client.SendMailAsync(message);
        }
    }
}
