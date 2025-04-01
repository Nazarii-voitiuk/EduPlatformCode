using EduCodePlatform.Models.Identity;

namespace EduCodePlatform.Models
{
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

}
