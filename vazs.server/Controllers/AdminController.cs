using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vazs.server.Models;

namespace vazs.server.Controllers
{
    [Authorize(Roles = "admin")]
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

        [HttpGet("[controller]/Ts")]
        public async Task<ActionResult> TSs()
        {
            try
            {
                var tsList = await _firebaseClient
                    .Child("ts")
                    .OnceAsync<TSModel>();

                return Ok(tsList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("[controller]/Ts/{uid}")]
        public async Task<ActionResult> GetTS(string uid)
        {
            try
            {
                var ts = await _firebaseClient
                    .Child("ts")
                    .Child(uid)
                    .OnceSingleAsync<TSModel>();

                if (ts != null)
                {
                    return Ok(ts);
                }
                else
                {
                    return NotFound(); // Если ТЗ с указанным ID не найдено
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[controller]/Ts/Create")]
        public async Task<ActionResult> CreateTS([FromBody] TSModel ts)
        {
            try
            {
                await _firebaseClient
                    .Child("ts")
                    .PostAsync(ts);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[controller]/Ts/Update/{uid}")]
        public async Task<ActionResult> UpdateTS(string uid, [FromBody] TSModel ts)
        {
            try
            {
                var tsToUpdate = await _firebaseClient
                    .Child("ts")
                    .Child(uid)
                    .OnceSingleAsync<TSModel>();

                if (tsToUpdate != null)
                {
                    tsToUpdate.Name = ts.Name;
                    tsToUpdate.Description = ts.Description;
                    tsToUpdate.CreationDate = ts.CreationDate;
                    tsToUpdate.Deadline = ts.Deadline;
                    tsToUpdate.Budget = ts.Budget;

                    await _firebaseClient
                        .Child("ts")
                        .Child(uid)
                        .PutAsync(tsToUpdate);

                    return Ok();
                }
                else
                {
                    return NotFound(); // Если ТЗ с указанным ID не найдено
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[controller]/Ts/Delete/{uid}")]
        public async Task<ActionResult> DeleteTS(string uid)
        {
            try
            {
                var tsToDelete = await _firebaseClient
                    .Child("ts")
                    .Child(uid)
                    .OnceSingleAsync<TSModel>();

                if (tsToDelete != null)
                {
                    await _firebaseClient
                        .Child("ts")
                        .Child(uid)
                        .DeleteAsync();

                    return Ok();
                }
                else
                {
                    return NotFound(); // Если ТЗ с указанным ID не найдено
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}