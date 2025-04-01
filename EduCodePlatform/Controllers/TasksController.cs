using EduCodePlatform.Data;
using EduCodePlatform.Data.Entities;
using EduCodePlatform.Models.Entities;
using EduCodePlatform.Models.Identity;
using EduCodePlatform.Services;
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
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CodeCheckService _checkService;

        public TasksController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            CodeCheckService checkService)
        {
            _db = db;
            _userManager = userManager;
            _checkService = checkService;
        }

        // =========== A) INDEX ============

        // GET: /Tasks/Index
        // [AllowAnonymous] -> гість може бачити список
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(); // Views/Tasks/Index.cshtml
        }

        // GET: /Tasks/GetById?id=5 (або /Tasks/GetById/5, за дефолтним маршрутом)
        [Authorize(Roles = "Admin")]
        [HttpGet] // без "(GetById/{id})"
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _db.Tasks
                .FirstOrDefaultAsync(t => t.TaskId == id);
            if (entity == null)
                return NotFound("Task not found.");

            return Ok(new
            {
                taskId = entity.TaskId,
                title = entity.Title,
                description = entity.Description,
                difficultyId = entity.DifficultyId,
                isAIgenerated = entity.IsAIgenerated,
                referenceHtml = entity.ReferenceHtml,
                referenceCss = entity.ReferenceCss,
                referenceJs = entity.ReferenceJs
            });
        }

        // GET: /Tasks/GetAll
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _db.Tasks
                .Include(t => t.Difficulty)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new {
                    t.TaskId,
                    t.Title,
                    DifficultyName = t.Difficulty.DifficultyName,
                    t.CreatedAt,
                    t.IsAIgenerated
                })
                .ToListAsync();

            return Json(tasks);
        }

        // =========== B) CREATE/EDIT/DELETE (ADMIN) ============

        // POST: /Tasks/CreateAjax
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] CodingTask model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Title))
                return BadRequest("Invalid data.");

            model.CreatedAt = DateTime.UtcNow;
            model.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _db.Tasks.Add(model);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Task created" });
        }

        // PUT: /Tasks/EditAjax
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> EditAjax([FromBody] CodingTask model)
        {
            if (model == null || model.TaskId <= 0)
                return BadRequest("Invalid data.");

            var entity = await _db.Tasks.FirstOrDefaultAsync(t => t.TaskId == model.TaskId);
            if (entity == null) return NotFound("Task not found.");

            entity.Title = model.Title;
            entity.Description = model.Description;
            entity.DifficultyId = model.DifficultyId;
            entity.IsAIgenerated = model.IsAIgenerated;
            entity.ReferenceHtml = model.ReferenceHtml;
            entity.ReferenceCss = model.ReferenceCss;
            entity.ReferenceJs = model.ReferenceJs;
            // entity.CreatedAt => no change
            // entity.CreatedBy => no change

            await _db.SaveChangesAsync();
            return Ok(new { message = "Task updated" });
        }

        // DELETE: /Tasks/DeleteAjax/5
        [Authorize(Roles = "Admin")]
        [HttpDelete] // без ("{id}")
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var task = await _db.Tasks.FirstOrDefaultAsync(t => t.TaskId == id);
            if (task == null) return NotFound("Task not found.");

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Task deleted" });
        }

        // =========== C) MANAGE TESTCASES (ADMIN) ===========

        [Authorize(Roles = "Admin")]
        public IActionResult ManageTestCases(int taskId)
        {
            ViewBag.TaskId = taskId;
            return View(); // Views/Tasks/ManageTestCases.cshtml
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetTestCases(int taskId)
        {
            var list = await _db.TaskTestCases
                .Where(tc => tc.TaskId == taskId)
                .Select(tc => new {
                    tc.TestCaseId,
                    tc.TaskId,
                    tc.HtmlRules,
                    tc.CssRules,
                    tc.InputData,
                    tc.ExpectedJsOutput,
                    tc.TimeLimitSeconds,
                    tc.Points
                })
                .ToListAsync();
            return Json(list);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateTestCaseAjax([FromBody] TaskTestCase model)
        {
            if (model == null || model.TaskId <= 0)
                return BadRequest("Invalid data.");

            _db.TaskTestCases.Add(model);
            await _db.SaveChangesAsync();
            return Ok(new { message = "TestCase created" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> EditTestCaseAjax([FromBody] TaskTestCase model)
        {
            if (model == null || model.TestCaseId <= 0)
                return BadRequest("Invalid data.");

            var entity = await _db.TaskTestCases.FirstOrDefaultAsync(x => x.TestCaseId == model.TestCaseId);
            if (entity == null) return NotFound("TestCase not found.");

            entity.HtmlRules = model.HtmlRules;
            entity.CssRules = model.CssRules;
            entity.InputData = model.InputData;
            entity.ExpectedJsOutput = model.ExpectedJsOutput;
            entity.TimeLimitSeconds = model.TimeLimitSeconds;
            entity.Points = model.Points;

            await _db.SaveChangesAsync();
            return Ok(new { message = "TestCase updated" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> DeleteTestCaseAjax(int id)
        {
            var entity = await _db.TaskTestCases.FirstOrDefaultAsync(x => x.TestCaseId == id);
            if (entity == null) return NotFound("TestCase not found.");

            _db.TaskTestCases.Remove(entity);
            await _db.SaveChangesAsync();
            return Ok(new { message = "TestCase deleted" });
        }

        // =========== D) SOLVE + CheckSolution ===========

        // GET: /Tasks/Solve?taskId=...
        [Authorize]
        [HttpGet]
        public IActionResult Solve(int taskId)
        {
            ViewBag.TaskId = taskId;
            return View(); // Views/Tasks/Solve.cshtml
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CheckSolution([FromBody] SolveTaskInput input)
        {
            var testCases = await _db.TaskTestCases
                .Where(tc => tc.TaskId == input.TaskId)
                .ToListAsync();
            if (!testCases.Any())
                return BadRequest("No test-cases found for this task.");

            bool allPassed = true;
            float totalPoints = 0f;
            float gainedPoints = 0f;

            foreach (var tc in testCases)
            {
                totalPoints += tc.Points;

                // 1) HTML
                var htmlOk = _checkService.CheckHtml(input.HtmlCode, tc.HtmlRules);
                if (!htmlOk) { allPassed = false; continue; }

                // 2) CSS
                var cssOk = _checkService.CheckCss(input.CssCode, tc.CssRules);
                if (!cssOk) { allPassed = false; continue; }

                // 3) JS
                try
                {
                    var consoleOutput = _checkService.RunJsWithJint(
                        input.JsCode,
                        tc.InputData,
                        tc.TimeLimitSeconds
                    );
                    if (!string.IsNullOrEmpty(tc.ExpectedJsOutput))
                    {
                        if (consoleOutput.Trim() != tc.ExpectedJsOutput.Trim())
                        {
                            allPassed = false;
                            continue;
                        }
                    }
                }
                catch
                {
                    allPassed = false;
                    continue;
                }

                gainedPoints += tc.Points;
            }

            float finalScore = (totalPoints == 0) ? 0 : (gainedPoints / totalPoints) * 100;

            // Зберігаємо CodeSubmission
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var codeSub = new CodeSubmission
            {
                UserId = currentUserId,
                Title = "Solution for Task#" + input.TaskId,
                IsPublic = false,
                HtmlCode = input.HtmlCode,
                CssCode = input.CssCode,
                JsCode = input.JsCode,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.CodeSubmissions.Add(codeSub);
            await _db.SaveChangesAsync();

            // Зберігаємо TaskSubmission
            var taskSub = new TaskSubmission
            {
                TaskId = input.TaskId,
                CodeSubmissionId = codeSub.CodeSubmissionId,
                Score = finalScore,
                SubmittedAt = DateTime.UtcNow,
                PassedAllTests = (gainedPoints == totalPoints)
            };
            _db.TaskSubmissions.Add(taskSub);
            await _db.SaveChangesAsync();

            // Гейміфікація
            if (taskSub.PassedAllTests)
            {
                var userProg = await _db.UserProgresses
                    .FirstOrDefaultAsync(up => up.UserId == currentUserId);
                if (userProg == null)
                {
                    userProg = new UserProgress
                    {
                        UserId = currentUserId,
                        XP = 0,
                        Level = 1,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _db.UserProgresses.Add(userProg);
                }
                int xpGained = (int)totalPoints;
                userProg.XP += xpGained;
                int newLvl = userProg.XP / 1000 + 1;
                if (newLvl > userProg.Level)
                    userProg.Level = newLvl;

                userProg.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            return Ok(new { passed = (gainedPoints == totalPoints), score = finalScore });
        }
    }

    // DTO
    public class SolveTaskInput
    {
        public int TaskId { get; set; }
        public string HtmlCode { get; set; }
        public string CssCode { get; set; }
        public string JsCode { get; set; }
    }
}
