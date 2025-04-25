using eBookStore.Application.Common.Dto;

namespace eBookStore.Infrastructure.Services
{
    public interface IAccountService
    {
        Task<ProfileDto> GetUserProfileDataAsync(string userId);
    }
}