using AutoMapper;
using eBookStore.Application.Common.Utilily;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;
using eBookStore.Infrastructure.Data.Identity;
using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eBookStore.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ICartService _cartService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AccountController(
            ILogger<AccountController> logger,
            ICartService cartService,
            IUserService userService,
            IMapper mapper,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _cartService = cartService;
            _userService = userService;
            _mapper = mapper;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginVM { RedirectUrl = returnUrl ??= Url.Content("~/") });
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
                    
                    await MergeCartOnLogin();
                    return RedirectUser(loginVM.RedirectUrl);
                }
                _logger.LogWarning("Failed login attempt for {Email}.", loginVM.Email);
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(loginVM);
        }
        private IActionResult RedirectUser(string? redirectUrl)
        {
            if (string.IsNullOrEmpty(redirectUrl))
                return RedirectToAction("Index", "Home");
            return LocalRedirect(redirectUrl);
        }
        public IActionResult Register(string returnUrl = null)
        {
            return View(new RegisterVM { RedirectUrl = returnUrl ??= Url.Content("~/") });
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
        public async Task<IActionResult> Logout()
        {
            var userName = User.Identity?.Name;
            await _signInManager.SignOutAsync();
            TempData["ToastrMessage"] = $"Goodbye, {userName}. You have been logged out.";
            TempData["ToastrType"] = "success";
            return RedirectToAction("Index", "Home");
        }


        public IActionResult AccessDenied()
        {
            _logger.LogWarning("Access denied for user {User}.", User.Identity?.Name ?? "Anonymous");
            return View();
        }


        private async Task MergeCartOnLogin()
        {
            var userId = _userManager.GetUserId(User);
            var sessionCart = GetSessionCart();
            var dbCart =await _cartService.GetUserCartAsync(userId);
            await _cartService.MergeSessionCartWithDbCartAsync(
                userId, 
                _mapper.Map<CartDto>(sessionCart), 
                dbCart
            );

            ClearSessionCart();
        }
        private CartVM? GetSessionCart()
        {
            var sessionCartJson = HttpContext.Session.GetString("Cart");
            if(string.IsNullOrEmpty(sessionCartJson)) return null;
            return JsonConvert.DeserializeObject<CartVM>(sessionCartJson);
        }

        private void ClearSessionCart()
        {
            HttpContext.Session.Remove("Cart");
        }

        public async Task<IActionResult> Profile()
        {
            var userProfileData = await _userService.GetUserProfileDataAsync();
            return View(_mapper.Map<ProfileVM>(userProfileData));
        }
    }
}
