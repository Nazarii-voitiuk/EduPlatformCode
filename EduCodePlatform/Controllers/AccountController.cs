using EduCodePlatform.Models.Identity;
using EduCodePlatform.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduCodePlatform.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // === LOGIN ===
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Користувача з такою поштою не знайдено");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Невірний логін або пароль");
            return View(model);
        }

        // === REGISTER ===
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Ця електронна пошта вже використовується.");
                return View(model);
            }

            var newUser = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var createResult = await _userManager.CreateAsync(newUser, model.Password);
            if (createResult.Succeeded)
            {
                // Перевірити наявність ролі "User", якщо немає — створити
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                }

                // Додати до ролі "User"
                await _userManager.AddToRoleAsync(newUser, "User");

                // Залогінити
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var err in createResult.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
                return View(model);
            }
        }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Logout()
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account"); // 👈 Якщо хочеш саме на Login
            }


        // === ACCESS DENIED ===
        public IActionResult AccessDenied() => View();

        // === DEV PASSWORD RESET ===
        [HttpGet]
        public IActionResult DevResetPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DevResetPassword(string email, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("", "Користувача не знайдено");
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (resetResult.Succeeded)
            {
                ViewBag.Message = "✅ Пароль успішно скинуто!";
            }
            else
            {
                ViewBag.Message = "❌ Помилка при скиданні:";
                foreach (var err in resetResult.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return View();
        }

        // =================================================================
        //   GOOGLE LOGIN - два методи: ExternalLogin, ExternalLoginCallback
        // =================================================================

        // 1) Кнопка Google: переходимо сюди, викликаємо Challenge
        [HttpGet]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            // Повернемося сюди після успішного Google-логіну
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            // Ініціюємо Challenge (Google OAuth2)
            return Challenge(properties, provider);
        }

        // 2) Колбек від Google
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            if (!string.IsNullOrEmpty(remoteError))
            {
                // Якщо сталася помилка
                ModelState.AddModelError("", $"Помилка від провайдера: {remoteError}");
                return RedirectToAction(nameof(Login));
            }

            // Отримати інформацію про користувача від Google
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                // Якщо з якоїсь причини не вдалося отримати дані
                return RedirectToAction(nameof(Login));
            }

            // Спробувати залогінити користувача на основі Google Login
            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                loginProvider: info.LoginProvider,
                providerKey: info.ProviderKey,
                isPersistent: false, // якщо хочете, можна зробити true
                bypassTwoFactor: true
            );

            if (signInResult.Succeeded)
            {
                // Якщо успішно залогінили користувача (він уже існує й привʼязаний до Google)
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // Якщо користувача з таким Google-аккаунтом ще немає, треба створити
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    // Чи вже існує в базі користувач з таким email?
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        // Створюємо нового користувача
                        user = new ApplicationUser
                        {
                            UserName = email,
                            Email = email
                            // Можете додати інші поля
                        };
                        var createResult = await _userManager.CreateAsync(user);
                        if (createResult.Succeeded)
                        {
                            // Додати роль "User"
                            if (!await _roleManager.RoleExistsAsync("User"))
                            {
                                await _roleManager.CreateAsync(new IdentityRole("User"));
                            }
                            await _userManager.AddToRoleAsync(user, "User");
                        }
                        else
                        {
                            // Якщо помилки під час створення
                            foreach (var e in createResult.Errors)
                            {
                                ModelState.AddModelError("", e.Description);
                            }
                            return RedirectToAction(nameof(Login));
                        }
                    }

                    // Привʼязуємо Google-аккаунт до цього користувача
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (addLoginResult.Succeeded)
                    {
                        // Залогінити його
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                // Якщо ми не отримали email від Google або інша проблема
                return RedirectToAction(nameof(Login));
            }
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }
    }
}
