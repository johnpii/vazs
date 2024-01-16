using Firebase.Auth;
using Firebase.Auth.Providers;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<IActionResult> Regist(RegistModel user)
        {
            try
            {
                //var userCredential = await client.CreateUserWithEmailAndPasswordAsync("cetadi9870@grassdev.com", "dsakgdkayuvsv123", "John");
                var userCredential = await client.CreateUserWithEmailAndPasswordAsync(user.Email, user.Password, user.Username);

                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userCredential.User.Uid, new Dictionary<string, object>
                {
                    { "role", "user" }
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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

                return Ok(userCredential.User.Uid);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}