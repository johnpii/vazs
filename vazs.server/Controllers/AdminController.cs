using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vazs.server.Models;

namespace vazs.server.Controllers
{
    //[Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly FirebaseClient _firebaseClient;

        public AdminController(FirebaseClient firebaseClient)
        {
            _firebaseClient = firebaseClient;
        }
        public async Task<ActionResult> Index()
        {
            try
            {
                var collections = await _firebaseClient
                    .Child("/")
                    .OnceAsync<object>();

                var collectionNames = collections.Select(c => c.Key);

                return Ok(collectionNames);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("[controller]/Departments")]
        public async Task<ActionResult> Departments()
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

        [HttpGet("[controller]/Departments/{uid}")]
        public async Task<ActionResult> GetDepartment(string uid)
        {
            try
            {
                var department = await _firebaseClient
                    .Child("departments")
                    .Child(uid)
                    .OnceSingleAsync<DepartmentModel>();

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

        [HttpPost("[controller]/Departments/Create")]
        public async Task<ActionResult> CreateDepartment([FromBody] DepartmentModel department)
        {
            try
            {
                await _firebaseClient
                    .Child("departments")
                    .PostAsync(department);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[controller]/Departments/Update/{uid}")]
        public async Task<ActionResult> UpdateDepartment(string uid, [FromBody] DepartmentModel department)
        {
            try
            {
                var departmentToUpdate = await _firebaseClient
                    .Child("departments")
                    .Child(uid)
                    .OnceSingleAsync<DepartmentModel>();

                if (departmentToUpdate != null)
                {
                    departmentToUpdate.Name = department.Name;
                    departmentToUpdate.Description = department.Description;

                    await _firebaseClient
                        .Child("departments")
                        .Child(uid)
                        .PutAsync(departmentToUpdate);

                    return Ok();
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

        [HttpPost("[controller]/Departments/Delete/{uid}")]
        public async Task<ActionResult> DeleteDepartment(string uid)
        {
            try
            {
                var departmentToDelete = await _firebaseClient
                    .Child("departments")
                    .Child(uid)
                    .OnceSingleAsync<DepartmentModel>();

                if (departmentToDelete != null)
                {
                    await _firebaseClient
                        .Child("departments")
                        .Child(uid)
                        .DeleteAsync();

                    return Ok();
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