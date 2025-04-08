using eBookStore.Application.Common.Utilily;
using eBookStore.Infrastructure.Data.Identity;
using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            ILogger<AccountController> logger,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginVM { RedirectUrl = returnUrl ??= Url.Content("~/") });
        }

        public IActionResult Register(string returnUrl = null)
        {
            return View(new RegisterVM { RedirectUrl = returnUrl ??= Url.Content("~/") });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RememberMe, false);
                if (result.Succeeded)
                {
                    TempData["ToastrMessage"] = "Login successful. Welcome back!";
                    TempData["ToastrType"] = "success";

                    var user = await _userManager.FindByEmailAsync(loginVM.Email);
                    if(await _userManager.IsInRoleAsync(user, AppConstant.Role_Admin))
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                    return RedirectUser(loginVM.RedirectUrl);
                }
                _logger.LogWarning("Failed login attempt for {Email}.", loginVM.Email);
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(loginVM);
        }

        public async Task<IActionResult> Logout()
        {
            var userName = User.Identity?.Name;
            await _signInManager.SignOutAsync();
            TempData["ToastrMessage"] = $"Goodbye, {userName}. You have been logged out.";
            TempData["ToastrType"] = "success";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                var user = CreateApplicationUser(registerVM);
                var result = await _userManager.CreateAsync(user, registerVM.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["ToastrMessage"] = "Registration successful. Welcome!";
                    TempData["ToastrType"] = "success";

                    return RedirectUser(registerVM.RedirectUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(registerVM);
        }

        public IActionResult AccessDenied()
        {
            _logger.LogWarning("Access denied for user {User}.", User.Identity?.Name ?? "Anonymous");
            return View();
        }

        private ApplicationUser CreateApplicationUser(RegisterVM registerVm)
        {
            return new ApplicationUser
            {
                UserName = registerVm.Email,
                Email = registerVm.Email,
                PhoneNumber = registerVm.PhoneNumber,
                Name = registerVm.Name,
                CreatedAt = DateTime.UtcNow
            };
        }
        private IActionResult RedirectUser(string? redirectUrl)
        {
            if (string.IsNullOrEmpty(redirectUrl))
                return RedirectToAction("Index", "Home");
            return LocalRedirect(redirectUrl);
        }
    }
}
