using eBookStore.Application.Common.Interfaces;
using eBookStore.Application.Common.Utilily;
using eBookStore.Domain.Entities;
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
        private readonly IOrderService _orderService;

        public AccountController(
            ILogger<AccountController> logger,
            ICartService cartService,
            IOrderService orderService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _cartService = cartService;
            _orderService = orderService;
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
                    await MergeCartOnLogin();
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

        private async Task MergeCartOnLogin()
        {
            var userId = _userManager.GetUserId(User);
            var sessionCartJson = HttpContext.Session.GetString("Cart");
            var dbCart = await _cartService.GetUserCartAsync(userId);

            if (!string.IsNullOrEmpty(sessionCartJson))
            {
                var sessionCart = JsonConvert.DeserializeObject<Cart>(sessionCartJson);

                if (dbCart == null)
                {
                    sessionCart.UserId = userId;
                    sessionCart.CreatedAt = DateTime.UtcNow;
                    _cartService.UpdateCart(sessionCart);
                }
                else
                {
                    foreach (var item in sessionCart.CartItems)
                    {
                        if (await _cartService.IsBookInCartAsync(userId, item.BookId))
                        {
                            await _cartService.AddQuantityAsync(dbCart.Id, item.BookId, item.Quantity);
                        }
                        else
                        {
                            await _cartService.AddToCartAsync(userId, item.BookId, item.Quantity);
                        }
                    }
                }

                HttpContext.Session.Remove("Cart");
            }
        }

        public async Task<IActionResult> Profile()
        {
            var orderHistory = await _orderService.GetUserOrdersAsync(_userManager.GetUserId(User));
            var user = await _userManager.GetUserAsync(User);
            var userInfoVM = new UserVM
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };

            var profileViewModel = new ProfileVM
            {
                UserInfo = userInfoVM,
                OrderHistory = orderHistory
            };
            return View(profileViewModel);
        }

    }
}
