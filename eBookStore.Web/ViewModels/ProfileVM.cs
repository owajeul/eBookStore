using eBookStore.Application.DTOs;

namespace eBookStore.Web.ViewModels
{
    public class ProfileVM
    {
        public UserVM UserInfo { get; set; }
        public List<OrderDto> OrderHistory { get; set; }
    }
}
