using eBookStore.Application.DTOs;

namespace eBookStore.Infrastructure.Services
{
    public interface IOrderNotificationService
    {
        Task SendOrderConfirmationEmailAsync(string email, OrderDto order);
    }
}