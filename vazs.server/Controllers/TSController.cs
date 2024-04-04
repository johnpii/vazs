using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using vazs.server.Models;
using Firebase.Storage;

namespace vazs.server.Controllers
{
    [Authorize(Roles = "user")]
    public class TSController : Controller
    {

        private readonly FirebaseClient _firebaseClient;
        private readonly IConfiguration _configuration;
        private readonly FirebaseStorage firebaseStorage;

        public TSController(FirebaseClient firebaseClient, IConfiguration configuration)
        {
            _firebaseClient = firebaseClient;            _configuration = configuration;
            firebaseStorage = new FirebaseStorage(_configuration.GetValue<string>("Storage_Path"));

        }

        [HttpGet("[controller]/CreateTS/{DepartmentName}")]
        public IActionResult CreateTS(string DepartmentName)
        {
            return View();
        }

        [HttpGet("[controller]/UpdateTS/{uid}")]
        public async Task<IActionResult> UpdateTS(string uid)
        {
            try
            {
                var ts = await _firebaseClient
                    .Child("ts")
                    .Child(uid)
                    .OnceSingleAsync<TSModel>();

                if (ts != null)
                {
                    return View(ts);
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

        [HttpGet("[controller]/")]
        public async Task<ActionResult> TSs()
        {
            try
            {
                var tsList = await _firebaseClient
                    .Child("ts")
                    .OrderBy("ClientID")
                    .EqualTo(HttpContext.User.FindFirstValue("clientUID"))
                    .OnceAsync<TSModel>();

                return Ok(tsList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("[controller]/{uid}")]
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

        [HttpPost]
        public async Task<ActionResult> CreateTS(TSModel ts)
        {
            try
            {
                var stream = ts.Document.OpenReadStream();
                string extension = Path.GetExtension(ts.Document.FileName);

                ts.Document = null;
                // Загружаем информацию о TS в Firebase Realtime Database
                var postResponse = await _firebaseClient.Child("ts").PostAsync(ts);

                string fileName = postResponse.Key;

                // Загружаем изображение в Firebase Storage
                var response = await firebaseStorage
                    .Child("ts")
                    .Child(fileName + extension)
                    .PutAsync(stream);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[controller]/UpdateTS/{uid}")]
        public async Task<ActionResult> UpdateTS(string uid, TSModel ts)
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

        [HttpPost("[controller]/DeleteTS/{uid}")]
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
