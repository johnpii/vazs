using FirebaseAdmin.Auth;
using System.Net.Mail;
using System.Net;

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
            // Настройки SMTP-сервера
            string smtpHost = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = _configuration.GetValue<string>("SMTP_Username");
            string smtpPassword = _configuration.GetValue<string>("SMTP_Password");

            // Создание сообщения
            MailMessage message = new MailMessage();
            message.From = new MailAddress(_configuration.GetValue<string>("SMTP_Username"));
            message.To.Add(new MailAddress(recipientEmail));
            message.Subject = "Подтверждение регистрации";
            message.BodyEncoding = System.Text.Encoding.UTF8; // указание кодировки письма
            message.IsBodyHtml = true; // указание формата письма (true - HTML, false - не HTML)
            // Загрузка HTML-шаблона из файла или строки
            string htmlTemplate = System.IO.File.ReadAllText(Path.Combine("wwwroot", "html", "confirmation_email_template.html"));

            // Вставка данных в HTML-шаблон
            htmlTemplate = htmlTemplate.Replace("{{username}}", username);
            htmlTemplate = htmlTemplate.Replace("{{confirmationLink}}", confirmationLink);
            message.Body = htmlTemplate;
            // Настройка клиента SMTP
            SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network; // определяет метод отправки сообщений
            smtpClient.EnableSsl = true; // отключает необходимость использования защищенного соединения с сервером
            smtpClient.UseDefaultCredentials = false; // отключение использования реквизитов авторизации "по-умолчанию"
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

            // Отправка письма
            smtpClient.SendMailAsync(message);
        }

        public async Task<bool> CheckEmailVerified(string mail)
        {
            // Проверка статуса подтверждения почты
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
