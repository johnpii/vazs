using Firebase.Database;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using vazs.server.Models;

namespace vazs.server.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FirebaseClient _firebaseClient;

        public HomeController(ILogger<HomeController> logger, FirebaseClient firebaseClient)
        {
            _logger = logger;
            _firebaseClient = firebaseClient;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                var departments = await _firebaseClient
                .Child("departments")
                .OnceAsync<DepartmentModel>();

                return Ok(departments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
