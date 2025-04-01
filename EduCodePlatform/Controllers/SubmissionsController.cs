using EduCodePlatform.Data;
using EduCodePlatform.Models.Entities;
using EduCodePlatform.Models.Identity;
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

        // ========== PART A: Index (internal list) ==========

        // GET: /Submissions/Index
        // Тут користувач (Admin бачить усі, User – свої).
        public IActionResult Index()
        {
            return View(); // Views/Submissions/Index.cshtml
        }

        // GET: /Submissions/GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = User.IsInRole("Admin");

            var query = _db.CodeSubmissions.AsQueryable();
            if (!isAdmin)
            {
                query = query.Where(x => x.UserId == currentUserId);
            }

            var list = await query
                .Select(c => new
                {
                    c.CodeSubmissionId,
                    c.UserId,
                    c.Title,
                    c.IsPublic,
                    c.CreatedAt,
                    c.UpdatedAt
                })
                .ToListAsync();

            return Json(list);
        }

        // DELETE: /Submissions/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool isAdmin = User.IsInRole("Admin");

                var entity = await _db.CodeSubmissions
                    .FirstOrDefaultAsync(c => c.CodeSubmissionId == id);
                if (entity == null)
                    return NotFound("Submission not found.");

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

        // ========== PART B: Gallery (публічні) ==========

        // [AllowAnonymous] => щоб гість міг бачити
        [AllowAnonymous]
        public IActionResult Gallery()
        {
            return View(); // Views/Submissions/Gallery.cshtml
        }

        // GET: /Submissions/GetPublic
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetPublic()
        {
            // беремо лише IsPublic = true
            var list = await _db.CodeSubmissions
                .Where(x => x.IsPublic)
                .Select(c => new
                {
                    c.CodeSubmissionId,
                    c.UserId,
                    c.Title,
                    c.IsPublic,
                    c.CreatedAt
                })
                .ToListAsync();

            return Json(list);
        }

        // ========== PART C: MyProfile (список лише своїх) ==========

        public IActionResult MyProfile()
        {
            return View(); // Views/Submissions/MyProfile.cshtml
        }

        // GET: /Submissions/GetMy
        [HttpGet]
        public async Task<IActionResult> GetMy()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var list = await _db.CodeSubmissions
                .Where(x => x.UserId == currentUserId)
                .Select(c => new
                {
                    c.CodeSubmissionId,
                    c.UserId,
                    c.Title,
                    c.IsPublic,
                    c.CreatedAt,
                    c.UpdatedAt
                })
                .ToListAsync();

            return Json(list);
        }

        // ========== PART D: Editor (Create/Edit) ==========

        // GET: /Submissions/Editor?submissionId=...
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

            // Якщо Admin -> дамо список користувачів
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
            return View(model); // Views/Submissions/Editor.cshtml
        }

        // POST: /Submissions/Save
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] EditorInputModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = User.IsInRole("Admin");

            // Визначимо власника
            var finalUserId = isAdmin && !string.IsNullOrEmpty(model.UserId)
                ? model.UserId
                : currentUserId;

            if (model.CodeSubmissionId.HasValue && model.CodeSubmissionId > 0)
            {
                // Edit
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
                // Create
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

    // ========== ViewModels/DTOs ==========
    public class EditorViewModel
    {
        public int? CodeSubmissionId { get; set; }
        public string Title { get; set; }
        public bool IsPublic { get; set; }
        public string HtmlCode { get; set; }
        public string CssCode { get; set; }
        public string JsCode { get; set; }

        public string SelectedUserId { get; set; }
        public List<ApplicationUser> AllUsers { get; set; } = new List<ApplicationUser>();
    }

    public class EditorInputModel
    {
        public int? CodeSubmissionId { get; set; }
        public string Title { get; set; }
        public bool IsPublic { get; set; }
        public string HtmlCode { get; set; }
        public string CssCode { get; set; }
        public string JsCode { get; set; }
        public string UserId { get; set; }
    }
}
