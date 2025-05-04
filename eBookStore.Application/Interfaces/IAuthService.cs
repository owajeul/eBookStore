using eBookStore.Application.DTOs;

namespace eBookStore.Infrastructure.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto> HandleGoogleLoginAsync();
        Task<AuthResultDto> LoginAsync(LoginDto loginDto);
        Task LogoutAsync();
        Task<AuthResultDto> RegisterAsync(RegisterDto registerDto);
    }
}