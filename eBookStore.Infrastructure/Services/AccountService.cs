using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using eBookStore.Application.Common.Dto;
using eBookStore.Application.Common.Interfaces;
using eBookStore.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;

namespace eBookStore.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private UserManager<ApplicationUser> _userManager;

        public AccountService(IOrderRepository orderRepository, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<ProfileDto> GetUserProfileDataAsync(string userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            return new ProfileDto
            {
                UserInfo = new UserDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                },
                OrderHistory = _mapper.Map<List<OrderDto>>(orders)
            };
        }
    }
}
