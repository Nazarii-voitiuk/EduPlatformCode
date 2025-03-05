using EduCodePlatform.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EduCodePlatform.ViewModels
{
    public class CodeSubmissionViewModel
    {
        public CodeSubmission CodeSubmission { get; set; }
        public IEnumerable<SelectListItem> LanguageList { get; set; }
    }
}
