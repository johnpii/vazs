using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vazs.server.ViewModels;
using vazs.server.Models;
using Firebase.Storage;

namespace vazs.server.Controllers
{
    [Authorize(Roles = "user")]
    public class AdminController : Controller
    {
        private readonly FirebaseClient _firebaseClient;
        private readonly FirebaseStorage _firebaseStorage;

        public AdminController(FirebaseClient firebaseClient, FirebaseStorage firebaseStorage)
        {
            _firebaseClient = firebaseClient;
            _firebaseStorage = firebaseStorage;
        }
        public async Task<ActionResult> Index()
        {
            try
            {
                var departments = await _firebaseClient
                .Child("departments")
                .OnceAsync<Department>();

                var departmentList = departments.Select(d => new DepartmentWithId
                {
                    Id = d.Key,
                    Name = d.Object.Name,
                    Description = d.Object.Description,
                    Image = d.Object.Image
                }).ToList();

                return View(departmentList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetDepartment(string uid)
        {
            try
            {
                var department = await _firebaseClient
                    .Child("departments")
                    .Child(uid)
                    .OnceSingleAsync<Department>();

                if (department != null)
                {
                    return Ok(department);
                }
                else
                {
                    return NotFound(); // Если департамент с указанным ID не найден
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public ActionResult CreateDepartment()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateDepartment(DepartmentModel department)
        {
            try
            {
                Department depart = new Department();
                depart.Name = department.Name;
                depart.Description = department.Description;
                using (var memoryStream = new MemoryStream())
                {
                    department.Image.CopyTo(memoryStream);
                    depart.Image = memoryStream.ToArray();
                }
                await _firebaseClient
                    .Child("departments")
                    .PostAsync(depart);

                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("[controller]/UpdateDepartment/{uid}")]
        public async Task<ActionResult> UpdateDepartment(string uid)
        {
            try 
            {
                var department = await _firebaseClient
                    .Child("departments")
                    .Child(uid)
                    .OnceSingleAsync<Department>();

                if (department != null)
                {
                    var departmentModel = new DepartmentModel
                    {
                        Name = department.Name,
                        Description = department.Description
                    };

                    return View(departmentModel);
                }
                else
                {
                    return NotFound(); // Если департамент с указанным ID не найден
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[controller]/UpdateDepartment/{uid}")]
        public async Task<ActionResult> UpdateDepartment(string uid, DepartmentModel department)
        {
            try
            {
                var departmentToUpdate = await _firebaseClient
                    .Child("departments")
                    .Child(uid)
                    .OnceSingleAsync<Department>();

                if (departmentToUpdate != null)
                {
                    departmentToUpdate.Name = department.Name;
                    departmentToUpdate.Description = department.Description;
                    if (department.Image != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            department.Image.CopyTo(memoryStream);
                            departmentToUpdate.Image = memoryStream.ToArray();
                        }
                    }
                    await _firebaseClient
                        .Child("departments")
                        .Child(uid)
                        .PutAsync(departmentToUpdate);
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return NotFound(); // Если департамент с указанным ID не найден
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("[controller]/DeleteDepartment/{uid}")]
        public async Task<ActionResult> DeleteDepartment(string uid)
        {
            try
            {
                var department = await _firebaseClient
                    .Child("departments")
                    .Child(uid)
                    .OnceSingleAsync<Department>();

                if (department != null)
                {
                    var departmentModel = new Department
                    {
                        Name = department.Name,
                        Description = department.Description,
                        Image = department.Image
                    };

                    return View(departmentModel);
                }
                else
                {
                    return NotFound(); // Если департамент с указанным ID не найден
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[controller]/DeleteDepartment/{uid}")]
        public async Task<ActionResult> DeleteDepartmentPost(string uid)
        {
            try
            {
                var departmentToDelete = await _firebaseClient
                    .Child("departments")
                    .Child(uid)
                    .OnceSingleAsync<Department>();

                if (departmentToDelete != null)
                {
                    await _firebaseClient
                        .Child("departments")
                        .Child(uid)
                        .DeleteAsync();

                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return NotFound(); // Если департамент с указанным ID не найден
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}