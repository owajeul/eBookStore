using AutoMapper;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;
using eBookStore.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Security.Claims;

namespace eBookStore.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICartService _cartService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public AuthService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ICartService cartService,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _cartService = cartService;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
    {
        var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, loginDto.RememberMe, false);
        if (!result.Succeeded)
        {
            return new AuthResultDto { Succeeded = false, Errors = new List<string> { "Invalid login attempt."} };
        }
        return new AuthResultDto { Succeeded = true };
    }

    public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
    {
        var user = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            PhoneNumber = registerDto.PhoneNumber,
            Name = registerDto.Name,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            return new AuthResultDto
            {
                Succeeded = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        return new AuthResultDto { Succeeded = true };
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<AuthResultDto> HandleGoogleLoginAsync()
    {
        var result = await _httpContextAccessor.HttpContext!.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (result?.Principal == null)
            return new AuthResultDto { Succeeded = false, Errors = new List<string> { "Authentication failed." } };

        var email = result.Principal.FindFirstValue(ClaimTypes.Email);
        var name = result.Principal.FindFirstValue(ClaimTypes.Name);
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser == null)
        {
            var newUser = new ApplicationUser { UserName = email, Email = email, Name = name };
            var createResult = await _userManager.CreateAsync(newUser);
            if (!createResult.Succeeded)
            {
                return new AuthResultDto { Succeeded = false, Errors = new List<string> { "User registration failed." } };
            }

            await _signInManager.SignInAsync(newUser, isPersistent: false);
        }
        else
        {
            await _signInManager.SignInAsync(existingUser, isPersistent: false);
        }
        return new AuthResultDto { Succeeded = true };
    }
}