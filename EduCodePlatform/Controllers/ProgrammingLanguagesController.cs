using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduCodePlatform.Data;
using EduCodePlatform.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EduCodePlatform.Controllers
{
    public class ProgrammingLanguagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProgrammingLanguagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProgrammingLanguages
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var languages = await _context.ProgrammingLanguages.ToListAsync();
            return View(languages);
        }

        // GET: ProgrammingLanguages/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var language = await _context.ProgrammingLanguages.FindAsync(id);
            if (language == null)
                return NotFound();

            return View(language);
        }

        // GET: ProgrammingLanguages/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProgrammingLanguages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Name)
        {
            // 1. Перевіряємо, що поле Name не порожнє
            if (string.IsNullOrWhiteSpace(Name))
            {
                ModelState.AddModelError("Name", "Назва мови обов’язкова!");
                return View(); // Повертаємо ту саму форму з помилкою
            }

            // 2. Перевіряємо, чи не існує дубліката (за бажанням, якщо унікальний індекс)
            bool exists = await _context.ProgrammingLanguages.AnyAsync(pl => pl.Name == Name);
            if (exists)
            {
                ModelState.AddModelError("Name", "Така мова вже існує!");
                return View();
            }

            // 3. Створюємо модель вручну
            var language = new ProgrammingLanguage
            {
                Name = Name
            };

            // 4. Додаємо в контекст і зберігаємо
            _context.ProgrammingLanguages.Add(language);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ProgrammingLanguages/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var language = await _context.ProgrammingLanguages.FindAsync(id);
            if (language == null)
                return NotFound();

            return View(language);
        }

        // POST: ProgrammingLanguages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Name)
        {
            // 1. Перевірка поля Name
            if (string.IsNullOrWhiteSpace(Name))
            {
                ModelState.AddModelError("Name", "Назва мови обов’язкова!");
                // Створюємо об’єкт, щоб знову передати у в’юшку
                var language2 = new ProgrammingLanguage { LanguageId = id, Name = Name };
                return View(language2);
            }

            // 2. Шукаємо в БД
            var language = await _context.ProgrammingLanguages.FindAsync(id);
            if (language == null)
                return NotFound();

            // 3. Перевірка дублікатів
            bool exists = await _context.ProgrammingLanguages.AnyAsync(pl => pl.Name == Name && pl.LanguageId != id);
            if (exists)
            {
                ModelState.AddModelError("Name", "Така мова вже існує!");
                var language2 = new ProgrammingLanguage { LanguageId = id, Name = Name };
                return View(language2);
            }

            // 4. Оновлюємо поле
            language.Name = Name;
            _context.Update(language);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ProgrammingLanguages/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var language = await _context.ProgrammingLanguages.FindAsync(id);
            if (language == null)
                return NotFound();

            return View(language);
        }

        // POST: ProgrammingLanguages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var language = await _context.ProgrammingLanguages.FindAsync(id);
            if (language != null)
            {
                _context.ProgrammingLanguages.Remove(language);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
