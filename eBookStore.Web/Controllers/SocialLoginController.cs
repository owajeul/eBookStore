using eBookStore.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.Controllers;

public class SocialLoginController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    public SocialLoginController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }
    public async Task Login()
    {
        await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
        {
            RedirectUri = Url.Action("GoogleResponse")
        });
    }

    public async Task<IActionResult> GoogleResponse()
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

            return RedirectToAction("Index", "Home");
        }

        return BadRequest("Authentication failed.");
    }
}
