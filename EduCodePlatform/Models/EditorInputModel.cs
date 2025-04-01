namespace EduCodePlatform.Models
{
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
