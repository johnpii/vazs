using FirebaseAdmin.Auth;
using System.Net;
using System.Net.Mail;

namespace vazs.server.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendConfirmationEmail(string recipientEmail, string confirmationLink, string username)
        {
            string smtpHost = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = _configuration.GetValue<string>("SMTP_Username");
            string smtpPassword = _configuration.GetValue<string>("SMTP_Password");

            MailMessage message = new MailMessage();
            message.From = new MailAddress(_configuration.GetValue<string>("SMTP_Username"));
            message.To.Add(new MailAddress(recipientEmail));
            message.Subject = "Подтверждение регистрации";
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;

            string htmlTemplate = System.IO.File.ReadAllText(Path.Combine("wwwroot", "html", "confirmation_email_template.html"));
            htmlTemplate = htmlTemplate.Replace("{{username}}", username);
            htmlTemplate = htmlTemplate.Replace("{{confirmationLink}}", confirmationLink);
            message.Body = htmlTemplate;

            SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

            smtpClient.SendMailAsync(message);
        }

        public async Task<bool> CheckEmailVerified(string mail)
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(mail);
            if (userRecord.EmailVerified)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
