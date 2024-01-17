using Firebase.Database;
using Firebase.Database.Query;
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

        public void Index()
        {
            // Создаем и заполняем департаменты
            var departments = new List<DepartmentModel>
            {
                new DepartmentModel { Name = "Department 1", Description = "Description 1" },
                new DepartmentModel { Name = "Department 2", Description = "Description 2" },
                new DepartmentModel { Name = "Department 3", Description = "Description 3" }
            };

            // Записываем департаменты в базу данных Firebase
            foreach (var department in departments)
            {
                _firebaseClient.Child("departments").PostAsync(department);
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
