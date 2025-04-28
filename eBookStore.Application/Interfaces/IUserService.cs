using eBookStore.Application.DTOs;
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
        Task<ProfileDto> GetUserProfileDataAsync();
        string GetUserId();
    }
}
