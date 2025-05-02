using System.Security.Claims;
using AutoMapper;
using eBookStore.Application.Common.Utilily;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;
using eBookStore.Infrastructure.Data.Identity;
using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
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

                    await MergeCartOnOrRegisterLogin();
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
                    await MergeCartOnOrRegisterLogin();
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


        public async Task GoogleLogin(string returnUrl = "/")
        {
            var redirectUrl = Url.Action("GoogleResponse", new { returnUrl });

            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, properties);
        }

        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                var email = result.Principal.FindFirstValue(ClaimTypes.Email);
                var name = result.Principal.FindFirstValue(ClaimTypes.Name);

                var existingUser = await _userManager.FindByEmailAsync(email);

                if (existingUser == null)
                {
                    var newUser = new ApplicationUser { UserName = email, Email = email, Name = name };

                    var createResult = await _userManager.CreateAsync(newUser);

                    if (createResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(newUser, isPersistent: false);
                    }
                    else
                    {
                        return BadRequest("User registration failed.");
                    }
                }
                else
                {
                    await _signInManager.SignInAsync(existingUser, isPersistent: false);
                }
                await MergeCartOnOrRegisterLogin();
                return LocalRedirect(returnUrl);
            }

            return BadRequest("Authentication failed.");
        }


        public IActionResult AccessDenied()
        {
            _logger.LogWarning("Access denied for user {User}.", User.Identity?.Name ?? "Anonymous");
            return View();
        }


        private async Task MergeCartOnOrRegisterLogin()
        {
            var userId = _userManager.GetUserId(User);
            var sessionCart = GetSessionCart();
            var dbCart = await _cartService.GetUserCartAsync(userId);
            var sessionCartDto = _mapper.Map<CartDto>(sessionCart);
            await _cartService.MergeSessionCartWithDbCartAsync(
                userId,
                sessionCartDto,
                dbCart
            );

            ClearSessionCart();
        }
        private CartVM? GetSessionCart()
        {
            var sessionCartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(sessionCartJson)) return null;
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
        public async Task<IActionResult> EditProfile(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            return View(_mapper.Map<UserVM>(user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UserVM user)
        {
            if (ModelState.IsValid)
            {
                var userDto = _mapper.Map<ApplicationUser>(user);
                var result = await _userService.u
                if (result.Succeeded)
                {
                    TempData["ToastrMessage"] = "Profile updated successfully.";
                    TempData["ToastrType"] = "success";
                    return RedirectToAction(nameof(Profile));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(userVm);
        }
    }
}
