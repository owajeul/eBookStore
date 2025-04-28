using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.DTOs
{
    public class ProfileDto
    {
        public UserDto UserInfo { get; set; }
        public List<OrderDto> OrderHistory { get; set; }
    }
}
