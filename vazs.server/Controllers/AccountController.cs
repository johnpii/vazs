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
                SendConfirmationEmail(user.Email, emailActionLink, user.Username);

                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("EMAIL_EXISTS"))
                {
                    ViewBag.Error = "Пользователь с такой почтой уже существует !";
                    return View();
                }
                else
                {
                    return BadRequest(ex.Message);
                }
            }
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
            message.Body = "<!DOCTYPE html>\r\n<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\">\r\n\r\n    <head>\r\n        <meta charset=\"utf-8\">\r\n        <meta name=\"viewport\" content=\"width=device-width\">\r\n        <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\r\n        <meta name=\"x-apple-disable-message-reformatting\">\r\n        <meta name=\"format-detection\" content=\"telephone=no,address=no,email=no,date=no,url=no\">\r\n\r\n        <meta name=\"color-scheme\" content=\"light\">\r\n        <meta name=\"supported-color-schemes\" content=\"light\">\r\n\r\n        \r\n        <!--[if !mso]><!-->\r\n          \r\n          <link rel=\"preload\" as=\"style\" href=\"https://fonts.googleapis.com/css2?family=Open+Sans:ital,wght@0,400;0,700;1,400;1,700&family=Open+Sans:ital,wght@0,400;0,700;1,400;1,700&family=Open+Sans:ital,wght@0,400;0,700;1,400;1,700&family=Open+Sans:ital,wght@0,400;0,700;1,400;1,700&family=Open+Sans:ital,wght@0,400;0,700;1,400;1,700&display=swap\">\r\n          <link rel=\"stylesheet\" href=\"https://fonts.googleapis.com/css2?family=Open+Sans:ital,wght@0,400;0,700;1,400;1,700&family=Open+Sans:ital,wght@0,400;0,700;1,400;1,700&family=Open+Sans:ital,wght@0,400;0,700;1,400;1,700&family=Open+Sans:ital,wght@0,400;0,700;1,400;1,700&family=Open+Sans:ital,wght@0,400;0,700;1,400;1,700&display=swap\">\r\n\r\n          <style type=\"text/css\">\r\n          // TODO: fix me!\r\n            @import url(https://fonts.googleapis.com/css2?family=Open+Sans:ital,wght@0,400;0,700;1,400;1,700&family=Open+Sans:ital,wght@0,400;0,700;1,400;1,700&family=Open+Sans:ital,wght@0,400;0,700;1,400;1,700&family=Open+Sans:ital,wght@0,400;0,700;1,400;1,700&family=Open+Sans:ital,wght@0,400;0,700;1,400;1,700&display=swap);\r\n        </style>\r\n        \r\n        <!--<![endif]-->\r\n\r\n        <!--[if mso]>\r\n          <style>\r\n              // TODO: fix me!\r\n              * {\r\n                  font-family: sans-serif !important;\r\n              }\r\n          </style>\r\n        <![endif]-->\r\n    \r\n        \r\n        <!-- NOTE: the title is processed in the backend during the campaign dispatch -->\r\n        <title></title>\r\n\r\n        <!--[if gte mso 9]>\r\n        <xml>\r\n            <o:OfficeDocumentSettings>\r\n                <o:AllowPNG/>\r\n                <o:PixelsPerInch>96</o:PixelsPerInch>\r\n            </o:OfficeDocumentSettings>\r\n        </xml>\r\n        <![endif]-->\r\n        \r\n    <style>\r\n        :root {\r\n            color-scheme: light;\r\n            supported-color-schemes: light;\r\n        }\r\n\r\n        html,\r\n        body {\r\n            margin: 0 auto !important;\r\n            padding: 0 !important;\r\n            height: 100% !important;\r\n            width: 100% !important;\r\n\r\n            overflow-wrap: break-word;\r\n            -ms-word-break: break-all;\r\n            -ms-word-break: break-word;\r\n            word-break: break-all;\r\n            word-break: break-word;\r\n        }\r\n\r\n\r\n        \r\n  direction: undefined;\r\n  center,\r\n  #body_table {\r\n    \r\n  }\r\n\r\n  ul, ol {\r\n    padding: 0;\r\n    margin-top: 0;\r\n    margin-bottom: 0;\r\n  }\r\n\r\n  li {\r\n    margin-bottom: 0;\r\n  }\r\n\r\n  \r\n\r\n  .list-block-list-outside-left li {\r\n    margin-left: 20px !important;\r\n  }\r\n\r\n  .list-block-list-outside-right li {\r\n    margin-right: 20px !important;\r\n  }\r\n\r\n  \r\n    .paragraph {\r\n      font-size: 16px;\r\n      font-family: Open Sans, sans-serif;\r\n      font-weight: normal;\r\n      font-style: normal;\r\n      text-align: start;\r\n      line-height: 1;\r\n      text-decoration: none;\r\n      color: #5f5f5f;\r\n      \r\n    }\r\n  \r\n\r\n    .heading1 {\r\n      font-size: 32px;\r\n      font-family: Open Sans, sans-serif;\r\n      font-weight: normal;\r\n      font-style: normal;\r\n      text-align: start;\r\n      line-height: 1;\r\n      text-decoration: none;\r\n      color: #444444;\r\n      \r\n    }\r\n  \r\n\r\n    .heading2 {\r\n      font-size: 28px;\r\n      font-family: Open Sans, sans-serif;\r\n      font-weight: normal;\r\n      font-style: normal;\r\n      text-align: start;\r\n      line-height: 1;\r\n      text-decoration: none;\r\n      color: #444444;\r\n      \r\n    }\r\n  \r\n\r\n    .heading3 {\r\n      font-size: 20px;\r\n      font-family: Open Sans, sans-serif;\r\n      font-weight: normal;\r\n      font-style: normal;\r\n      text-align: start;\r\n      line-height: 1;\r\n      text-decoration: none;\r\n      color: #444444;\r\n      \r\n    }\r\n  \r\n\r\n    .list {\r\n      font-size: 16px;\r\n      font-family: Open Sans, sans-serif;\r\n      font-weight: normal;\r\n      font-style: normal;\r\n      text-align: start;\r\n      line-height: 1;\r\n      text-decoration: none;\r\n      color: #5f5f5f;\r\n      \r\n    }\r\n  \r\n\r\n  p a, \r\n  li a {\r\n    \r\n  display: inline-block;  \r\n    color: #3498db;\r\n    text-decoration: none;\r\n    font-style: normal;\r\n    font-weight: normal;\r\n\r\n  }\r\n\r\n  .button-table a {\r\n    text-decoration: none;\r\n    font-style: normal;\r\n    font-weight: normal;\r\n  }\r\n\r\n  .paragraph > span {text-decoration: none;}.heading1 > span {text-decoration: none;}.heading2 > span {text-decoration: none;}.heading3 > span {text-decoration: none;}.list > span {text-decoration: none;}\r\n\r\n\r\n        * {\r\n            -ms-text-size-adjust: 100%;\r\n            -webkit-text-size-adjust: 100%;\r\n        }\r\n\r\n        div[style*=\"margin: 16px 0\"] {\r\n            margin: 0 !important;\r\n        }\r\n\r\n        #MessageViewBody,\r\n        #MessageWebViewDiv {\r\n            width: 100% !important;\r\n        }\r\n\r\n        table {\r\n            border-collapse: collapse;\r\n            border-spacing: 0;\r\n            mso-table-lspace: 0pt !important;\r\n            mso-table-rspace: 0pt !important;\r\n        }\r\n        table:not(.button-table) {\r\n            border-spacing: 0 !important;\r\n            border-collapse: collapse !important;\r\n            table-layout: fixed !important;\r\n            margin: 0 auto !important;\r\n        }\r\n\r\n        th {\r\n            font-weight: normal;\r\n        }\r\n\r\n        tr td p {\r\n            margin: 0;\r\n        }\r\n\r\n        img {\r\n            -ms-interpolation-mode: bicubic;\r\n        }\r\n\r\n        a[x-apple-data-detectors],\r\n\r\n        .unstyle-auto-detected-links a,\r\n        .aBn {\r\n            border-bottom: 0 !important;\r\n            cursor: default !important;\r\n            color: inherit !important;\r\n            text-decoration: none !important;\r\n            font-size: inherit !important;\r\n            font-family: inherit !important;\r\n            font-weight: inherit !important;\r\n            line-height: inherit !important;\r\n        }\r\n\r\n        .im {\r\n            color: inherit !important;\r\n        }\r\n\r\n        .a6S {\r\n            display: none !important;\r\n            opacity: 0.01 !important;\r\n        }\r\n\r\n        img.g-img+div {\r\n            display: none !important;\r\n        }\r\n\r\n        @media only screen and (min-device-width: 320px) and (max-device-width: 374px) {\r\n            u~div .contentMainTable {\r\n                min-width: 320px !important;\r\n            }\r\n        }\r\n\r\n        @media only screen and (min-device-width: 375px) and (max-device-width: 413px) {\r\n            u~div .contentMainTable {\r\n                min-width: 375px !important;\r\n            }\r\n        }\r\n\r\n        @media only screen and (min-device-width: 414px) {\r\n            u~div .contentMainTable {\r\n                min-width: 414px !important;\r\n            }\r\n        }\r\n    </style>\r\n\r\n    <style>\r\n        @media only screen and (max-device-width: 600px) {\r\n            .contentMainTable {\r\n                width: 100% !important;\r\n                margin: auto !important;\r\n            }\r\n            .single-column {\r\n                width: 100% !important;\r\n                margin: auto !important;\r\n            }\r\n            .multi-column {\r\n                width: 100% !important;\r\n                margin: auto !important;\r\n            }\r\n            .imageBlockWrapper {\r\n                width: 100% !important;\r\n                margin: auto !important;\r\n            }\r\n        }\r\n        @media only screen and (max-width: 600px) {\r\n            .contentMainTable {\r\n                width: 100% !important;\r\n                margin: auto !important;\r\n            }\r\n            .single-column {\r\n                width: 100% !important;\r\n                margin: auto !important;\r\n            }\r\n            .multi-column {\r\n                width: 100% !important;\r\n                margin: auto !important;\r\n            }\r\n            .imageBlockWrapper {\r\n                width: 100% !important;\r\n                margin: auto !important;\r\n            }\r\n        }\r\n    </style>\r\n    <style></style>\r\n    \r\n<!--[if mso | IE]>\r\n    <style>\r\n        .list-block-outlook-outside-left {\r\n            margin-left: -18px;\r\n        }\r\n    \r\n        .list-block-outlook-outside-right {\r\n            margin-right: -18px;\r\n        }\r\n\r\n        a:link, span.MsoHyperlink {\r\n            mso-style-priority:99;\r\n            \r\n  display: inline-block;  \r\n    color: #3498db;\r\n    text-decoration: none;\r\n    font-style: normal;\r\n    font-weight: normal;\r\n\r\n        }\r\n    </style>\r\n<![endif]-->\r\n\r\n\r\n    </head>\r\n\r\n    <body width=\"100%\" style=\"margin: 0; padding: 0 !important; mso-line-height-rule: exactly; background-color: #ffffff;\">\r\n        <center role=\"article\" aria-roledescription=\"email\" lang=\"en\" style=\"width: 100%; background-color: #ffffff;\">\r\n            <!--[if mso | IE]>\r\n            <table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" id=\"body_table\" style=\"background-color: #ffffff;\">\r\n            <tbody>    \r\n                <tr>\r\n                    <td>\r\n                    <![endif]-->\r\n                        <table align=\"center\" role=\"presentation\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"600\" style=\"margin: auto;\" class=\"contentMainTable\">\r\n                            <tr class=\"wp-block-editor-headingblock-v1\"><td valign=\"top\" style=\"background-color:#fff;display:block;padding-top:50px;padding-right:20px;padding-bottom:20px;padding-left:20px;text-align:center\"><p style=\"font-family:Open Sans, sans-serif;text-align:center;line-height:36.80px;font-size:32px;background-color:#fff;color:#444444;margin:0;word-break:normal\" class=\"heading1\">Привет " + username + "</p></td></tr><tr class=\"wp-block-editor-paragraphblock-v1\"><td valign=\"top\" style=\"padding:20px 20px 20px 20px;background-color:#fff\"><p class=\"paragraph\" style=\"font-family:Open Sans, sans-serif;text-align:center;line-height:18.40px;font-size:16px;margin:0;color:#5f5f5f;word-break:normal\">Пожалуйста, подтвердите вашу регистрацию, перейдя по ссылке:</p></td></tr><tr class=\"wp-block-editor-buttonblock-v1\" align=\"center\"><td style=\"background-color:#fff;padding-top:20px;padding-right:20px;padding-bottom:20px;padding-left:20px;width:100%\" valign=\"top\"><table role=\"presentation\" cellspacing=\"0\" cellpadding=\"0\" class=\"button-table\"><tbody><tr><td valign=\"top\" class=\"button-td button-td-primary\" style=\"cursor:pointer;border:none;border-radius:4px;background-color:#3DBD61;font-size:16px;font-family:Open Sans, sans-serif;width:fit-content;color:#ffffff\"><a style=\"color:#ffffff\" href=\"" + confirmationLink + "\">\r\n    <table role=\"presentation\">\r\n    <tbody><tr>\r\n      <!-- Top padding -->\r\n      <td valign=\"top\" colspan=\"3\" height=\"12\" style=\"height: 12px; line-height: 1px; padding: 0;\">\r\n        <span style=\"display: inline-block;\">&nbsp;</span>\r\n      </td>\r\n    </tr>\r\n    <tr>\r\n      <!-- Left padding -->\r\n      <td valign=\"top\" width=\"16\" style=\"width: 16px; line-height: 1px; padding: 0;\">\r\n        <span style=\"display: inline-block;\">&nbsp;</span>\r\n      </td>\r\n      <!-- Content -->\r\n      <td valign=\"top\" style=\"\r\n        display: inline-block;\r\n        cursor: pointer; border: none; border-radius: 4px; background-color: #3DBD61; font-size: 16; font-family: Open Sans, sans-serif; width: fit-content; font-weight: null; letter-spacing: undefined;\r\n          color: #ffffff;\r\n          padding: 0;\r\n        \">\r\n        Подтвердить\r\n      </td>\r\n      <!-- Right padding -->\r\n      <td valign=\"top\" width=\"16\" style=\"width: 16px; line-height: 1px; padding: 0;\">\r\n        <span style=\"display: inline-block;\">&nbsp;</span>\r\n      </td>\r\n    </tr>\r\n    <!-- Bottom padding -->\r\n    <tr>\r\n      <td valign=\"top\" colspan=\"3\" height=\"12\" style=\"height: 12px; line-height: 1px; padding: 0;\">\r\n        <span style=\"display: inline-block;\">&nbsp;</span>\r\n      </td>\r\n    </tr>\r\n  </tbody></table>\r\n    </a></td></tr></tbody></table></td></tr><tr><td valign=\"top\" align=\"center\" style=\"padding:20px 20px 20px 20px;background-color:#fff\"><p aria-label=\"Unsubscribe\" class=\"paragraph\" style=\"font-family:Open Sans, sans-serif;text-align:center;line-height:13.80px;font-size:12px;margin:0;color:#5f5f5f;word-break:normal\">This email was sent by: VAZS</p></td></tr>\r\n                        </table>\r\n                    <!--[if mso | IE]>\r\n                    </td>\r\n                </tr>\r\n            </tbody>\r\n            </table>\r\n            <![endif]-->\r\n        </center>\r\n    </body>\r\n</html>";

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
                        new Claim(ClaimTypes.Email, userData.Email),
                        new Claim("clientUID", uid)
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
    }
}