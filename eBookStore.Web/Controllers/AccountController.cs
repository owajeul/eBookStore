using AutoMapper;
using eBookStore.Application.Common.Interfaces;
using eBookStore.Application.Common.Utilily;
using eBookStore.Domain.Entities;
using eBookStore.Infrastructure.Data.Identity;
using eBookStore.Infrastructure.Services;
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
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public AccountController(
            ILogger<AccountController> logger,
            ICartService cartService,
            IAccountService accountService,
            IMapper mapper,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _cartService = cartService;
            _accountService = accountService;
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
            var dbCart = await GetUserCartFromDatabase(userId);
            if (sessionCart == null) return;

            await MergeCartsAsync(userId, sessionCart, dbCart);
            ClearSessionCart();
        }
        private Cart? GetSessionCart()
        {
            var sessionCartJson = HttpContext.Session.GetString("Cart");
            if(string.IsNullOrEmpty(sessionCartJson)) return null;
            return JsonConvert.DeserializeObject<Cart>(sessionCartJson);
        }
        private async Task<Cart> GetUserCartFromDatabase(string userId)
        {
            return await _cartService.GetUserCartAsync(userId);
        }
        private async Task MergeCartsAsync(string userId, Cart sessionCart, Cart? dbCart)
        {
            if (dbCart == null)
            {
                CreateNewCartFromSession(userId, sessionCart);
            }
            else
            {
                await MergeSessionCartItemsIntoDbCartAsync(userId, dbCart.Id, sessionCart);
            }
        }
        private void CreateNewCartFromSession(string userId, Cart sessionCart)
        {
            sessionCart.UserId = userId;
            sessionCart.CreatedAt = DateTime.UtcNow;
            _cartService.UpdateCart(sessionCart);
        }
        private async Task MergeSessionCartItemsIntoDbCartAsync(string userId, int dbCartId, Cart sessionCart)
        {
           foreach(var item in sessionCart.CartItems)
            {
                if (await _cartService.IsBookInCartAsync(userId, item.BookId))
                {
                    await _cartService.AddQuantityAsync(dbCartId, item.BookId, item.Quantity);
                }
                else
                {
                    await _cartService.AddToCartAsync(userId, item.BookId, item.Quantity);
                }
            }
        }
        private void ClearSessionCart()
        {
            HttpContext.Session.Remove("Cart");
        }

        public async Task<IActionResult> Profile()
        {
            var userProfileData = await _accountService.GetUserProfileDataAsync(_userManager.GetUserId(User));
            return View(_mapper.Map<ProfileVM>(userProfileData));
        }
    }
}
