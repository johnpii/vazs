using AutoMapper;
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
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, FirebaseClient firebaseClient, IMapper mapper)
        {
            _logger = logger;
            _firebaseClient = firebaseClient;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            try
            {
                var departments = await _firebaseClient
                .Child("departments")
                .OnceAsync<DepartmentModelForDatabase>();

                var departmentListToView = _mapper.Map<List<DepartmentModelForDatabase>>(departments);

                return View(departmentListToView);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
