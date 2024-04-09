using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vazs.server.Models;
using vazs.server.ViewModels;

namespace vazs.server.Controllers
{
    [Authorize(Roles = "user")]
    public class AdminController : Controller
    {
        private readonly FirebaseClient _firebaseClient;

        public AdminController(FirebaseClient firebaseClient)
        {
            _firebaseClient = firebaseClient;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            try
            {
                var departments = await _firebaseClient
                .Child("departments")
                .OnceAsync<DepartmentModelForDatabase>();

                var departmentListToView = departments.Select(d => new DepartmentModelForIndex
                {
                    Id = d.Key,
                    Name = d.Object.Name,
                    Description = d.Object.Description,
                    Image = d.Object.Image
                }).ToList();

                return View(departmentListToView);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult CreateDepartment()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateDepartment(DepartmentModelForCreate department)
        {
            try
            {
                DepartmentModelForDatabase depart = new DepartmentModelForDatabase();
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
                    .OnceSingleAsync<DepartmentModelForDatabase>();

                if (department != null)
                {
                    var departmentModel = new DepartmentModelForUpdate
                    {
                        Name = department.Name,
                        Description = department.Description
                    };

                    return View(departmentModel);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[controller]/UpdateDepartment/{uid}")]
        public async Task<ActionResult> UpdateDepartment(string uid, DepartmentModelForUpdate department)
        {
            try
            {
                var departmentToUpdate = await _firebaseClient
                    .Child("departments")
                    .Child(uid)
                    .OnceSingleAsync<DepartmentModelForDatabase>();

                if (departmentToUpdate != null)
                {
                    departmentToUpdate.Name = department.Name;
                    departmentToUpdate.Description = department.Description;
                    using (var memoryStream = new MemoryStream())
                    {
                        department.Image.CopyTo(memoryStream);
                        departmentToUpdate.Image = memoryStream.ToArray();
                    }    

                    await _firebaseClient
                        .Child("departments")
                        .Child(uid)
                        .PutAsync(departmentToUpdate);

                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return NotFound();
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
                    .OnceSingleAsync<DepartmentModelForDatabase>();

                if (department != null)
                {       
                    return View(department);
                }
                else
                {
                    return NotFound();
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
                    .OnceSingleAsync<DepartmentModelForDatabase>();

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
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}