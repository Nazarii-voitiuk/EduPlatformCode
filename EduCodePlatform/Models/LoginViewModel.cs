using System.ComponentModel.DataAnnotations;

namespace EduCodePlatform.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Вкажіть Email")]
        [EmailAddress(ErrorMessage = "Невірна електронна адреса")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Вкажіть пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Запам'ятати мене")]
        public bool RememberMe { get; set; }
    }
}
