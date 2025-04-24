using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eBookStore.Domain.Entities;

namespace eBookStore.Application.Common.Dto
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string UserId { get; set; }
        public string ShippingAddress { get; set; }
        public string PhoneNumber { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public decimal TotalPrice
        {
            get
            {
                return OrderItems.Sum(item => item.UnitPrice * item.Quantity);
            }
        }
    }
}
