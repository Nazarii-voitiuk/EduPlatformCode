using EduCodePlatform.Data;
using EduCodePlatform.Models.Entities;
using EduCodePlatform.Models.Identity;
using EduCodePlatform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EduCodePlatform.Controllers
{
    [Authorize]
    public class SubmissionsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubmissionsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ========== A: Internal List (Index) ==========

        // /Submissions/Index
        public IActionResult Index()
        {
            // Повертає в’ю Views/Submissions/Index.cshtml
            return View();
        }

        // /Submissions/GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = User.IsInRole("Admin");

            // Приєднуємо .Include(c => c.User), щоб дістати email
            var query = _db.CodeSubmissions.Include(c => c.User).AsQueryable();

            // Якщо не адмін, то фільтруємо лише свої
            if (!isAdmin)
            {
                query = query.Where(x => x.UserId == currentUserId);
            }

            var list = await query
                .Select(c => new
                {
                    c.CodeSubmissionId,
                    UserEmail = c.User.Email,   // <-- замість UserId
                    c.Title,
                    c.IsPublic,
                    c.CreatedAt,
                    c.UpdatedAt
                })
                .ToListAsync();

            return Json(list);
        }

        // DELETE /Submissions/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool isAdmin = User.IsInRole("Admin");

                var entity = await _db.CodeSubmissions.FirstOrDefaultAsync(c => c.CodeSubmissionId == id);
                if (entity == null)
                    return NotFound("Submission not found.");

                // Перевіряємо права
                if (!isAdmin && entity.UserId != currentUserId)
                    return Forbid();

                _db.CodeSubmissions.Remove(entity);
                await _db.SaveChangesAsync();

                return Ok(new { success = true, message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        // ========== B: Public Gallery ==========

        // /Submissions/Gallery
        [AllowAnonymous]
        public IActionResult Gallery()
        {
            return View(); // Views/Submissions/Gallery.cshtml
        }

        // /Submissions/GetPublic
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetPublic()
        {
            var list = await _db.CodeSubmissions
                .Where(x => x.IsPublic)
                .Include(c => c.User)
                .Select(c => new
                {
                    c.CodeSubmissionId,
                    UserEmail = c.User.Email,
                    c.Title,
                    c.IsPublic,
                    c.CreatedAt
                })
                .ToListAsync();

            return Json(list);
        }

        // ========== C: MyProfile (список своїх) ==========

        // /Submissions/MyProfile
        public IActionResult MyProfile()
        {
            return View(); // Views/Submissions/MyProfile.cshtml
        }

        // /Submissions/GetMy
        [HttpGet]
        public async Task<IActionResult> GetMy()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var list = await _db.CodeSubmissions
                .Where(x => x.UserId == currentUserId)
                .Include(c => c.User)
                .Select(c => new
                {
                    c.CodeSubmissionId,
                    UserEmail = c.User.Email,
                    c.Title,
                    c.IsPublic,
                    c.CreatedAt,
                    c.UpdatedAt
                })
                .ToListAsync();

            return Json(list);
        }

        // ========== D: Editor (Create/Edit) ==========

        [HttpGet]
        public async Task<IActionResult> Editor(int? submissionId)
        {
            CodeSubmission existing = null;
            if (submissionId.HasValue && submissionId.Value > 0)
            {
                existing = await _db.CodeSubmissions
                    .FirstOrDefaultAsync(c => c.CodeSubmissionId == submissionId.Value);
                if (existing == null)
                    return NotFound("Submission not found.");
            }

            var allUsers = new List<ApplicationUser>();
            if (User.IsInRole("Admin"))
            {
                allUsers = await _userManager.Users.ToListAsync();
            }

            var model = new EditorViewModel
            {
                CodeSubmissionId = existing?.CodeSubmissionId,
                Title = existing?.Title,
                IsPublic = existing?.IsPublic ?? false,
                HtmlCode = existing?.HtmlCode,
                CssCode = existing?.CssCode,
                JsCode = existing?.JsCode,
                SelectedUserId = existing?.UserId,
                AllUsers = allUsers
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] EditorInputModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = User.IsInRole("Admin");

            var finalUserId = isAdmin && !string.IsNullOrEmpty(model.UserId)
                ? model.UserId
                : currentUserId;

            if (model.CodeSubmissionId.HasValue && model.CodeSubmissionId > 0)
            {
                var entity = await _db.CodeSubmissions
                    .FirstOrDefaultAsync(x => x.CodeSubmissionId == model.CodeSubmissionId.Value);
                if (entity == null)
                    return NotFound("Submission not found.");

                if (!isAdmin && entity.UserId != currentUserId)
                    return Forbid();

                entity.Title = model.Title;
                entity.IsPublic = model.IsPublic;
                entity.HtmlCode = model.HtmlCode;
                entity.CssCode = model.CssCode;
                entity.JsCode = model.JsCode;
                entity.UpdatedAt = DateTime.UtcNow;

                if (isAdmin) entity.UserId = finalUserId;

                await _db.SaveChangesAsync();
                return Ok(new { message = "Updated successfully", submissionId = entity.CodeSubmissionId });
            }
            else
            {
                var newSubmission = new CodeSubmission
                {
                    UserId = finalUserId,
                    Title = model.Title,
                    IsPublic = model.IsPublic,
                    HtmlCode = model.HtmlCode,
                    CssCode = model.CssCode,
                    JsCode = model.JsCode,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _db.CodeSubmissions.Add(newSubmission);
                await _db.SaveChangesAsync();

                return Ok(new { message = "Created successfully", submissionId = newSubmission.CodeSubmissionId });
            }
        }
    }
}
