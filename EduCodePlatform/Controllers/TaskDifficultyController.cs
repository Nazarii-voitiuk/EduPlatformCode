using EduCodePlatform.Data;
using EduCodePlatform.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace EduCodePlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TaskDifficultyController : Controller
    {
        private readonly ApplicationDbContext _db;
        public TaskDifficultyController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /TaskDifficulty/Index => сторінка з таблицею (AJAX)
        public IActionResult Index()
        {
            return View();
        }

        // GET: /TaskDifficulty/GetAll => AJAX
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var diffs = await _db.TaskDifficulties
                .Select(d => new {
                    d.DifficultyId,
                    d.DifficultyName
                })
                .ToListAsync();

            return Json(diffs);
        }

        // POST: /TaskDifficulty/CreateAjax => AJAX
        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] TaskDifficulty model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.DifficultyName))
            {
                return BadRequest("DifficultyName is required");
            }

            _db.TaskDifficulties.Add(model);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Created successfully" });
        }

        // PUT: /TaskDifficulty/EditAjax => AJAX
        [HttpPut]
        public async Task<IActionResult> EditAjax([FromBody] TaskDifficulty model)
        {
            if (model == null || model.DifficultyId <= 0)
                return BadRequest("Invalid data.");

            var entity = await _db.TaskDifficulties
                .FirstOrDefaultAsync(d => d.DifficultyId == model.DifficultyId);

            if (entity == null)
                return NotFound("Difficulty not found.");

            entity.DifficultyName = model.DifficultyName;
            await _db.SaveChangesAsync();

            return Ok(new { message = "Updated successfully" });
        }

        // DELETE: /TaskDifficulty/DeleteAjax/5 => AJAX
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var diff = await _db.TaskDifficulties.FirstOrDefaultAsync(d => d.DifficultyId == id);
            if (diff == null)
                return NotFound("Difficulty not found.");

            _db.TaskDifficulties.Remove(diff);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Deleted successfully" });
        }
    }
}
