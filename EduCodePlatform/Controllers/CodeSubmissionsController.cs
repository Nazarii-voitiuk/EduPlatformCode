using EduCodePlatform.Data;                  // де лежить ApplicationDbContext
using EduCodePlatform.Models.Entities;       // де лежить CodeSubmission
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EduCodePlatform.Controllers
{
    public class CodeSubmissionsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CodeSubmissionsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /CodeSubmissions/Index
        public IActionResult Index()
        {
            // Просто повертаємо View з таблицею або інтерфейсом,
            // де через AJAX будемо викликати GetAll, Create, Edit, Delete
            return View();
        }

        // GET: /CodeSubmissions/GetAll
        // Повертає JSON зі списком сабмішенів
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Якщо немає зв'язку з ProgrammingLanguage,
            // просто обираємо дані з CodeSubmission
            var submissions = await _db.CodeSubmissions
                .Select(c => new
                {
                    c.CodeSubmissionId,
                    c.UserId,
                    HtmlCode = c.HtmlCode,
                    CssCode = c.CssCode,
                    JsCode = c.JsCode,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            return Json(submissions);
        }

        // POST: /CodeSubmissions/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CodeSubmission model)
        {
            // Перевірка, що model != null
            if (model == null)
            {
                return BadRequest("No data received.");
            }

            try
            {
                // (Опціонально) Якщо потрібно, щоб UserId був поточним логіном:
                // string currentUserId = User.Identity.Name; 
                // або ClaimTypes.NameIdentifier, якщо налаштовано Identity
                // model.UserId = currentUserId;

                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;

                _db.CodeSubmissions.Add(model);
                await _db.SaveChangesAsync();

                return Ok(new { success = true, message = "Code submission created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: /CodeSubmissions/Edit
        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] CodeSubmission model)
        {
            if (model == null || model.CodeSubmissionId <= 0)
            {
                return BadRequest("Invalid submission ID or no data provided.");
            }

            try
            {
                var entity = await _db.CodeSubmissions
                    .FirstOrDefaultAsync(c => c.CodeSubmissionId == model.CodeSubmissionId);

                if (entity == null)
                    return NotFound("CodeSubmission not found.");

                // Оновлюємо потрібні поля
                entity.HtmlCode = model.HtmlCode;
                entity.CssCode = model.CssCode;
                entity.JsCode = model.JsCode;
                entity.UpdatedAt = DateTime.UtcNow;

                // (Опціонально) Якщо треба оновлювати UserId — робимо це
                // entity.UserId = model.UserId;

                await _db.SaveChangesAsync();

                return Ok(new { success = true, message = "Updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: /CodeSubmissions/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entity = await _db.CodeSubmissions
                    .FirstOrDefaultAsync(c => c.CodeSubmissionId == id);

                if (entity == null)
                    return NotFound("CodeSubmission not found.");

                _db.CodeSubmissions.Remove(entity);
                await _db.SaveChangesAsync();

                return Ok(new { success = true, message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
