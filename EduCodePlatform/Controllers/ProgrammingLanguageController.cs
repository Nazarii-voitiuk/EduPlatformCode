using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduCodePlatform.Data;
using EduCodePlatform.Data.Entities;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace EduCodePlatform.Controllers
{
    public class ProgrammingLanguageController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProgrammingLanguageController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /ProgrammingLanguage/Index
        public IActionResult Index()
        {
            // Повертаємо просто View з таблицею (у якій за допомогою AJAX будемо вантажити дані)
            return View();
        }

        // GET: /ProgrammingLanguage/GetAll
        // Віддаємо JSON зі списком мов
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var languages = await _db.ProgrammingLanguages
                .Select(p => new
                {
                    p.LanguageId,
                    p.Name
                })
                .ToListAsync();

            return Json(languages);
        }

        // POST: /ProgrammingLanguage/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProgrammingLanguage model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Name))
            {
                return BadRequest("Name is required.");
            }

            try
            {
                // Перевірка, чи немає вже з такою назвою
                bool exists = await _db.ProgrammingLanguages
                    .AnyAsync(p => p.Name == model.Name);
                if (exists)
                {
                    return BadRequest("Language with this name already exists.");
                }

                var newLang = new ProgrammingLanguage
                {
                    Name = model.Name
                };
                _db.ProgrammingLanguages.Add(newLang);
                await _db.SaveChangesAsync();

                return Ok(new { success = true, message = "Created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: /ProgrammingLanguage/Edit
        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] ProgrammingLanguage model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Name))
            {
                return BadRequest("Name is required.");
            }

            try
            {
                var lang = await _db.ProgrammingLanguages
                    .FirstOrDefaultAsync(p => p.LanguageId == model.LanguageId);

                if (lang == null)
                {
                    return NotFound("Language not found.");
                }

                // Перевірка на дублікати
                bool duplicate = await _db.ProgrammingLanguages
                    .AnyAsync(p => p.Name == model.Name && p.LanguageId != model.LanguageId);
                if (duplicate)
                {
                    return BadRequest("Language with this name already exists.");
                }

                lang.Name = model.Name;
                await _db.SaveChangesAsync();

                return Ok(new { success = true, message = "Updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: /ProgrammingLanguage/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var lang = await _db.ProgrammingLanguages
                    .FirstOrDefaultAsync(p => p.LanguageId == id);

                if (lang == null)
                    return NotFound("Language not found");

                _db.ProgrammingLanguages.Remove(lang);
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
