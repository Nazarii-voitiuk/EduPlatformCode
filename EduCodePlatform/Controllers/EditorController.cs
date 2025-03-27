using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduCodePlatform.Controllers
{
    public class EditorController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize] // Авторизація для збереження
        [HttpPost]
        public IActionResult SaveCode([FromBody] EditorInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid code data.");
            }

            // Логіка збереження в БД або інші дії.
            // Для прикладу повертаємо штучний submissionId=123.
            return Ok(new
            {
                message = "Code saved successfully",
                submissionId = 123
            });
        }
    }

    public class EditorInputModel
    {
        public string HtmlCode { get; set; }
        public string CssCode { get; set; }
        public string JsCode { get; set; }
    }
}
