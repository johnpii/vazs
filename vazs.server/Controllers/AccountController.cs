using Firebase.Auth;
using Firebase.Auth.Providers;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using vazs.server.Models;

namespace vazs.server.Controllers
{
    public class AccountController : Controller
    {
        private static bool firebaseAppCreated = false;

        private readonly IConfiguration _configuration;

        FirebaseAuthClient client;
        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
            FirebaseAuthConfig config = new FirebaseAuthConfig
            {
                ApiKey = _configuration.GetValue<string>("Api_Key"),
                AuthDomain = _configuration.GetValue<string>("AuthDomain"),
                Providers = new FirebaseAuthProvider[]
                {
                    // Add and configure individual providers
                    new GoogleProvider().AddScopes("email"),
                    new EmailProvider()
                     // ...
                }
            };

            client = new FirebaseAuthClient(config);

            if (!firebaseAppCreated)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(@"firebase_auth.json")
                });
                firebaseAppCreated = true;
            }
        }

        [HttpGet]
        public IActionResult Regist()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Regist(RegistModel user)
        {
            try
            {
                var userCredential = await client.CreateUserWithEmailAndPasswordAsync(user.Email, user.Password, user.Username);

                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userCredential.User.Uid, new Dictionary<string, object>
                {
                    { "role", "user" }
                });

                // Генерация ссылки для подтверждения почты
                var emailActionLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(user.Email);

                // Отправка письма с ссылкой для подтверждения почты
                SendConfirmationEmail(user.Email, emailActionLink);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private void SendConfirmationEmail(string recipientEmail, string confirmationLink)
        {
            // Настройки SMTP-сервера (замените значения на свои)
            string smtpHost = "smtp.elasticemail.com";
            int smtpPort = 2525;
            string smtpUsername = _configuration.GetValue<string>("SMTP_Username");
            string smtpPassword = _configuration.GetValue<string>("SMTP_Password");

            // Создание сообщения
            MailMessage message = new MailMessage();
            message.From = new MailAddress(_configuration.GetValue<string>("SMTP_Username"));
            message.To.Add(new MailAddress(recipientEmail));
            message.Subject = "Подтверждение регистрации";
            message.Body = "Пожалуйста, подтвердите вашу регистрацию, перейдя по ссылке: " + confirmationLink;

            // Настройка клиента SMTP
            SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            smtpClient.EnableSsl = true;

            // Отправка письма
            smtpClient.Send(message);
        }

        [HttpGet("[controller]/[action]/{mail}")]
        public async Task<IActionResult> CheckEmailVerified(string mail)
        {
            // Проверка статуса подтверждения почты
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(mail);
            if (userRecord.EmailVerified)
            {
                return Ok("Почта подтверждена");
            }
            else
            {
                return BadRequest("Подтвердите вашу почту, чтобы завершить регистрацию.");
            }
        }

        [HttpGet("[controller]/[action]/{uid}")]
        public async Task<IActionResult> CheckRole(string uid)
        {
            try
            {
                var user = await FirebaseAuth.DefaultInstance.GetUserAsync(uid); // uid - идентификатор пользователя

                // Проверка наличия роли
                if (user.CustomClaims != null && user.CustomClaims.TryGetValue("role", out var roleValue))
                {
                    string role = roleValue?.ToString();
                    return Ok(role);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel user)
        {
            try
            {
                var userCredential = await client.SignInWithEmailAndPasswordAsync(user.Email, user.Password);
                string uid = userCredential.User.Uid;
                if (!string.IsNullOrEmpty(uid))
                {
                    var userData = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, userData.DisplayName),
                        new Claim(ClaimTypes.Email, userData.Email)
                    };

                    if (userData.CustomClaims != null && userData.CustomClaims.TryGetValue("role", out var roleValue))
                    {
                        string role = roleValue?.ToString();
                        claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
    }
}