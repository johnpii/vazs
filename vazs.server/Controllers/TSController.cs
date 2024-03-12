using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Mvc;
using vazs.server.Models;

namespace vazs.server.Controllers
{
    public class TSController : Controller
    {

        private readonly FirebaseClient _firebaseClient;

        public TSController(FirebaseClient firebaseClient)
        {
            _firebaseClient = firebaseClient;
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
