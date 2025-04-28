using eBookStore.Application.Interfaces;
using eBookStore.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eBookStore.Application.DTOs;
using eBookStore.Infrastructure.Repositories;

namespace eBookStore.Infrastructure.Services;


public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOrderService _orderService;

    public UserService(
        UserManager<ApplicationUser> userManager, 
        IHttpContextAccessor httpContextAccessor,
        IOrderService orderService
        )
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _orderService = orderService;
    }

    public async Task<UserDto> GetUserAsync()
    {
        var userId = GetUserId();
        var user = await _userManager.FindByIdAsync(userId);
        return new UserDto
        {
            UserId = userId,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };
    }
    public string GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        return _userManager.GetUserId(user);
    }

    public async Task<ProfileDto> GetUserProfileDataAsync()
    {
        var user = await GetUserAsync();
        var orders = await _orderService.GetUserOrdersAsync(user.UserId);
        return new ProfileDto
        {
            UserInfo = user,
            OrderHistory = orders
        };
    }
}

