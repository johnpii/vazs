using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.AspNetCore.Mvc;
using vazs.Server.Models;

namespace vazs.Server.Controllers
{
    public class AccountController : Controller
    {
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
        }
        [HttpPost]
        public async Task Regist(RegistModel user)
        {
            //var userCredential = await client.CreateUserWithEmailAndPasswordAsync("cetadi9870@grassdev.com", "dsakgdkayuvsv123", "John");
            var userCredential = await client.CreateUserWithEmailAndPasswordAsync(user.Email, user.Password, user.Username);
        }

    }
}