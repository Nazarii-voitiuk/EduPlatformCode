using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduCodePlatform.Data;
using EduCodePlatform.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EduCodePlatform.Controllers
{
    public class CodeSubmissionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CodeSubmissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CodeSubmissions
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Завантажуємо всі CodeSubmission з приєднаною мовою
            var submissions = await _context.CodeSubmissions
                .Include(cs => cs.ProgrammingLanguage)
                .ToListAsync();
            return View(submissions);
        }

        // GET: CodeSubmissions/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var submission = await _context.CodeSubmissions
                .Include(cs => cs.ProgrammingLanguage)
                .FirstOrDefaultAsync(m => m.CodeSubmissionId == id);
            if (submission == null)
                return NotFound();

            return View(submission);
        }

        // GET: CodeSubmissions/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Передаємо список мов для випадаючого списку
            ViewBag.LanguageList = await _context.ProgrammingLanguages
                .Select(pl => new { pl.LanguageId, pl.Name })
                .ToListAsync();

            return View();
        }

        // POST: CodeSubmissions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string userId, string codeText, int languageId)
        {
            // Перевіряємо валідацію вручну
            if (string.IsNullOrWhiteSpace(userId))
                ModelState.AddModelError("UserId", "UserId не може бути порожнім.");
            if (string.IsNullOrWhiteSpace(codeText))
                ModelState.AddModelError("CodeText", "CodeText не може бути порожнім.");
            if (languageId <= 0)
                ModelState.AddModelError("LanguageId", "Оберіть коректну мову.");

            // Якщо є помилки – повертаємо ту саму форму з повідомленнями
            if (!ModelState.IsValid)
            {
                // Знову завантажуємо список мов
                ViewBag.LanguageList = await _context.ProgrammingLanguages
                    .Select(pl => new { pl.LanguageId, pl.Name })
                    .ToListAsync();
                return View();
            }

            // Створюємо об’єкт вручну
            var submission = new CodeSubmission
            {
                UserId = userId,
                CodeText = codeText,
                LanguageId = languageId,
                CreatedAt = DateTime.Now
            };

            _context.CodeSubmissions.Add(submission);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: CodeSubmissions/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var submission = await _context.CodeSubmissions.FindAsync(id);
            if (submission == null)
                return NotFound();

            // Передаємо список мов у ViewBag
            ViewBag.LanguageList = await _context.ProgrammingLanguages
                .Select(pl => new { pl.LanguageId, pl.Name })
                .ToListAsync();

            return View(submission);
        }

        // POST: CodeSubmissions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int codeSubmissionId, string userId, string codeText, int languageId)
        {
            // Перевіряємо базові речі
            if (string.IsNullOrWhiteSpace(userId))
                ModelState.AddModelError("UserId", "UserId не може бути порожнім.");
            if (string.IsNullOrWhiteSpace(codeText))
                ModelState.AddModelError("CodeText", "CodeText не може бути порожнім.");
            if (languageId <= 0)
                ModelState.AddModelError("LanguageId", "Оберіть коректну мову.");

            var submission = await _context.CodeSubmissions.FindAsync(codeSubmissionId);
            if (submission == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                // Знову передаємо список мов
                ViewBag.LanguageList = await _context.ProgrammingLanguages
                    .Select(pl => new { pl.LanguageId, pl.Name })
                    .ToListAsync();

                // Повертаємо назад у в’юшку, заповнюючи поля, щоб користувач бачив поточні дані
                submission.UserId = userId;
                submission.CodeText = codeText;
                submission.LanguageId = languageId;
                return View(submission);
            }

            // Оновлюємо поля вручну
            submission.UserId = userId;
            submission.CodeText = codeText;
            submission.LanguageId = languageId;
            submission.UpdatedAt = DateTime.Now;

            _context.Update(submission);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: CodeSubmissions/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var submission = await _context.CodeSubmissions
                .Include(cs => cs.ProgrammingLanguage)
                .FirstOrDefaultAsync(m => m.CodeSubmissionId == id);
            if (submission == null)
                return NotFound();

            return View(submission);
        }

        // POST: CodeSubmissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var submission = await _context.CodeSubmissions.FindAsync(id);
            if (submission != null)
            {
                _context.CodeSubmissions.Remove(submission);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
