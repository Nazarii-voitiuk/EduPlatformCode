using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduCodePlatform.Data;
using EduCodePlatform.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EduCodePlatform.Controllers
{
    public class EditorSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EditorSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EditorSettings
        public async Task<IActionResult> Index()
        {
            return View(await _context.EditorSettings.ToListAsync());
        }

        // GET: EditorSettings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var setting = await _context.EditorSettings
                .FirstOrDefaultAsync(m => m.EditorSettingId == id);
            if (setting == null) return NotFound();

            return View(setting);
        }

        // GET: EditorSettings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EditorSettings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EditorSettingId,UserId,Theme,TabSize")] EditorSetting editorSetting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(editorSetting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(editorSetting);
        }

        // GET: EditorSettings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var setting = await _context.EditorSettings.FindAsync(id);
            if (setting == null) return NotFound();

            return View(setting);
        }

        // POST: EditorSettings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EditorSettingId,UserId,Theme,TabSize")] EditorSetting editorSetting)
        {
            if (id != editorSetting.EditorSettingId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(editorSetting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EditorSettingExists(editorSetting.EditorSettingId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(editorSetting);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var setting = await _context.EditorSettings
                .FirstOrDefaultAsync(m => m.EditorSettingId == id);
            if (setting == null) return NotFound();

            return View(setting);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var setting = await _context.EditorSettings.FindAsync(id);
            if (setting != null)
            {
                _context.EditorSettings.Remove(setting);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        private bool EditorSettingExists(int id)
        {
            return _context.EditorSettings.Any(e => e.EditorSettingId == id);
        }
    }
}
