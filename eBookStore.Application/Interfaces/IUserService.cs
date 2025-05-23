﻿using eBookStore.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserAsync();
        Task<UserDto> GetUserByIdAsync(string userId);
        Task<ProfileDto> GetUserProfileDataAsync();
        string GetUserId();
        Task UpdateUserProfileAsync(UserDto userDto);
        Task<bool> IsUserInRoleAsync(string userId, string role);
    }
}
