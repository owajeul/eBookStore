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
using SendGrid.Helpers.Errors.Model;

namespace eBookStore.Infrastructure.Services;


public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOrderService _orderService;
    private readonly IUnitOfWork _unitOfWork;   

    public UserService(
        
        UserManager<ApplicationUser> userManager, 
        IHttpContextAccessor httpContextAccessor,
        IOrderService orderService,
        IUnitOfWork unitOfWork
        )
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _orderService = orderService;
        _unitOfWork = unitOfWork;
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
            PhoneNumber = user.PhoneNumber,
            Address = user.Address
        };
    }

    public async Task<UserDto> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return new UserDto
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address
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

    public async Task UpdateUserProfileAsync(UserDto userDto)
    {
        var user = await _userManager.FindByIdAsync(userDto.UserId);
        if (user == null) throw new NotFoundException("User not found");
        user.Name = userDto.Name;
        user.PhoneNumber = userDto.PhoneNumber;
        user.Address = userDto.Address;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) throw new Exception("Failed to update user profile");

    }

    public async Task<bool> IsUserInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new NotFoundException("User not found");
        return await _userManager.IsInRoleAsync(user, role);
    }

}

