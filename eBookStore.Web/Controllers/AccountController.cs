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
using eBookStore.Infrastructure.Services;

namespace eBookStore.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ICartService _cartService;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AccountController(
            ILogger<AccountController> logger,
            ICartService cartService,
            IUserService userService,
            IAuthService authService,
            IMapper mapper)
        {
            _logger = logger;
            _cartService = cartService;
            _userService = userService;
            _authService = authService;
            _mapper = mapper;
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
                var loginDto = _mapper.Map<LoginDto>(loginVM);  
                var result = await _authService.LoginAsync(loginDto);
                if (result.Succeeded)
                {
                    TempData["ToastrMessage"] = "Login successful. Welcome back!";
                    TempData["ToastrType"] = "success";
                    await MergeCartOnOrRegisterLogin();
                    return RedirectUser(loginVM.RedirectUrl);
                }
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
               var registerDto = _mapper.Map<RegisterDto>(registerVM);
                var result = await _authService.RegisterAsync(registerDto);
                if (result.Succeeded)
                {
                    TempData["ToastrMessage"] = "Registration successful. Welcome!";
                    TempData["ToastrType"] = "success";
                    await MergeCartOnOrRegisterLogin();
                    return RedirectUser(registerVM.RedirectUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            return View(registerVM);
        }

        public async Task<IActionResult> Logout()
        {
            var userName = User.Identity?.Name;
            await _authService.LogoutAsync();
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
            var result = await _authService.HandleGoogleLoginAsync();
            if (result.Succeeded)
            {
                TempData["ToastrMessage"] = "Google login successful. Welcome!";
                TempData["ToastrType"] = "success";
                await MergeCartOnOrRegisterLogin();
                return LocalRedirect(returnUrl);
            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
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
            var userId = _userService.GetUserId();
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
                var userDto = _mapper.Map<UserDto>(user);
                await _userService.UpdateUserProfileAsync(userDto);

                TempData["ToastrMessage"] = "Profile updated successfully.";
                TempData["ToastrType"] = "success";
                return RedirectToAction(nameof(Profile));
            }
            return View(user);
        }
    }
}
