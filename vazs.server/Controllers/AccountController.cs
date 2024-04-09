using Firebase.Auth;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using vazs.server.Services;
using System.Security.Claims;
using vazs.server.ViewModels;

namespace vazs.server.Controllers
{
    public class AccountController : Controller
    {

        private readonly FirebaseAuthClient _client;
        private readonly EmailService _emailService;
        public AccountController(FirebaseAuthClient firebaseAuthClient, EmailService emailService)
        {
            _client = firebaseAuthClient;
            _emailService = emailService;
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
                var userCredential = await _client.CreateUserWithEmailAndPasswordAsync(user.Email, user.Password, user.Username);

                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userCredential.User.Uid, new Dictionary<string, object>
                {
                    { "role", "user" }
                });

                var emailActionLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(user.Email);

                _emailService.SendConfirmationEmail(user.Email, emailActionLink, user.Username);

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
                var userCredential = await _client.SignInWithEmailAndPasswordAsync(user.Email, user.Password);
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
    }
}