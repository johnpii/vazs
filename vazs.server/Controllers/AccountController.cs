using ElasticEmail.Api;
using ElasticEmail.Client;
using ElasticEmail.Model;
using Firebase.Auth;
using Firebase.Auth.Providers;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Net;
using System.Net.Mail;
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
                    // Провайдеры
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
                SendConfirmationEmail(user.Email, emailActionLink, user.Username);

                ViewBag.ConfirmationMessage = "На вашу почту было отправлено письмо с подтверждением";
                return View();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("EMAIL_EXISTS"))
                {
                    ViewBag.Error = "Пользователь с такой почтой уже существует!";
                    return View();
                }
                else
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            if (returnUrl != null)
            {
                var options = new CookieOptions
                {
                    MaxAge = TimeSpan.FromMinutes(5),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };

                Response.Cookies.Append("returnUrl", returnUrl, options);
            }
            return View();
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
                    var returnUrl = Request.Cookies["returnUrl"];
                    Response.Cookies.Delete("returnUrl");
                    return Redirect(returnUrl ?? "/");
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("INVALID_LOGIN_CREDENTIALS"))
                {
                    ViewBag.Error = "Пользователь не найден или не правильно введены данные !";
                    return View();
                }
                else
                {
                    return BadRequest(ex.Message);
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private void SendConfirmationEmail(string recipientEmail, string confirmationLink, string username)
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
    }
}