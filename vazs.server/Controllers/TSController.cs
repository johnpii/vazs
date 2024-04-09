using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using vazs.server.Models;
using vazs.server.ViewModels;

namespace vazs.server.Controllers
{
    [Authorize(Roles = "user")]
    public class TSController : Controller
    {

        private readonly FirebaseClient _firebaseClient;
        private readonly FirebaseStorage _firebaseStorage;

        public TSController(FirebaseClient firebaseClient, FirebaseStorage firebaseStorage)
        {
            _firebaseClient = firebaseClient;
            _firebaseStorage = firebaseStorage;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            try
            {
                var tsList = await _firebaseClient
                    .Child("ts")
                    .Child(HttpContext.User.FindFirstValue(ClaimTypes.Email).Replace(".", "_"))
                    .OnceAsync<TSModelForDelete>();

                foreach (var ts in tsList)
                {
                    var storageClient = _firebaseStorage.Child("ts").Child(HttpContext.User.FindFirstValue(ClaimTypes.Email).Replace(".", "_")).Child(ts.Key + ts.Object.DocumentExt);
                    var downloadUrl = await storageClient.GetDownloadUrlAsync();
                    ts.Object.DownloadUrl = downloadUrl.ToString();
                }

                var tsListToView = tsList.Select(d => new TSModelForIndex
                {
                    Id = d.Key,
                    Name = d.Object.Name,
                    Description = d.Object.Description,
                    CreationDate = d.Object.CreationDate,
                    Deadline = d.Object.Deadline,
                    Budget = d.Object.Budget,
                    DocumentExt = d.Object.DocumentExt,
                    DepartmentName = d.Object.DepartmentName,
                    DownloadUrl = d.Object.DownloadUrl
                }).ToList();

                return View(tsListToView);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult CreateTS(string DepartmentName)
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateTS(TSViewModelForCreate ts)
        {
            try
            {
                var stream = ts.Document.OpenReadStream();
                string extension = Path.GetExtension(ts.Document.FileName);

                ts.Document = null;
                ts.DocumentExt = extension;

                var postResponse = await _firebaseClient.Child("ts").Child(HttpContext.User.FindFirstValue(ClaimTypes.Email).Replace(".", "_")).PostAsync(ts);

                string fileName = postResponse.Key;
  
                await _firebaseStorage
                    .Child("ts")
                    .Child(HttpContext.User.FindFirstValue(ClaimTypes.Email).Replace(".", "_"))
                    .Child(fileName + extension)
                    .PutAsync(stream);

                return RedirectToAction("Index", "TS");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("[controller]/UpdateTS/{uid}")]
        public async Task<IActionResult> UpdateTS(string uid)
        {
            try
            {
                var ts = await _firebaseClient
                    .Child("ts")
                    .Child(HttpContext.User.FindFirstValue(ClaimTypes.Email).Replace(".", "_"))
                    .Child(uid)
                    .OnceSingleAsync<TSViewModelForUpdate>();

                if (ts != null)
                {
                    return View(ts);
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

        [HttpPost("[controller]/UpdateTS/{uid}")]
        public async Task<ActionResult> UpdateTS(string uid, TSViewModelForUpdate ts)
        {
            try
            {
                var tsToUpdate = await _firebaseClient
                    .Child("ts")
                    .Child(HttpContext.User.FindFirstValue(ClaimTypes.Email).Replace(".", "_"))
                    .Child(uid)
                    .OnceSingleAsync<TSModelForDatabase>();

                if (tsToUpdate != null)
                {
                    if (ts.Document != null)
                    {
                        string fileNamePrev = uid + tsToUpdate.DocumentExt;

                        await _firebaseStorage
                            .Child("ts")
                            .Child(HttpContext.User.FindFirstValue(ClaimTypes.Email).Replace(".", "_"))
                            .Child(fileNamePrev)
                            .DeleteAsync();

                        var stream = ts.Document.OpenReadStream();
                        string extension = Path.GetExtension(ts.Document.FileName);
                        string fileNamePres = uid + extension;

                        await _firebaseStorage
                            .Child("ts")
                            .Child(HttpContext.User.FindFirstValue(ClaimTypes.Email).Replace(".", "_"))
                            .Child(fileNamePres)
                            .PutAsync(stream);

                        tsToUpdate.DocumentExt = extension;
                    }

                    tsToUpdate.Name = ts.Name;
                    tsToUpdate.Description = ts.Description;
                    tsToUpdate.Deadline = ts.Deadline;
                    tsToUpdate.Budget = ts.Budget;

                    await _firebaseClient
                        .Child("ts")
                        .Child(HttpContext.User.FindFirstValue(ClaimTypes.Email).Replace(".", "_"))
                        .Child(uid)
                        .PutAsync(tsToUpdate);

                    return RedirectToAction("Index", "TS");
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

        [HttpGet("[controller]/DeleteTS/{uid}")]
        public async Task<ActionResult> DeleteTS(string uid)
        {
            var ts = await _firebaseClient
                    .Child("ts")
                    .Child(HttpContext.User.FindFirstValue(ClaimTypes.Email).Replace(".", "_"))
                    .Child(uid)
                    .OnceSingleAsync<TSModelForDelete>();

            if (ts != null)
            {
                var storageClient = _firebaseStorage.Child("ts").Child(HttpContext.User.FindFirstValue(ClaimTypes.Email).Replace(".", "_")).Child(uid + ts.DocumentExt);
                var downloadUrl = await storageClient.GetDownloadUrlAsync();
                ts.DownloadUrl = downloadUrl.ToString();
                return View(ts);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("[controller]/DeleteTS/{uid}")]
        public async Task<ActionResult> DeleteTSPost(string uid)
        {
            try
            {
                var tsToDelete = await _firebaseClient
                    .Child("ts")
                    .Child(HttpContext.User.FindFirstValue(ClaimTypes.Email).Replace(".", "_"))
                    .Child(uid)
                    .OnceSingleAsync<TSModelForDatabase>();

                if (tsToDelete != null)
                {
                    await _firebaseClient
                        .Child("ts")
                        .Child(HttpContext.User.FindFirstValue(ClaimTypes.Email).Replace(".", "_"))
                        .Child(uid)
                        .DeleteAsync();

                    string fileName = uid + tsToDelete.DocumentExt;

                    await _firebaseStorage
                        .Child("ts")
                        .Child(HttpContext.User.FindFirstValue(ClaimTypes.Email).Replace(".", "_"))
                        .Child(fileName)
                        .DeleteAsync();

                    return RedirectToAction("Index", "TS");
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
