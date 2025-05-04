using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eBookStore.Domain.Entities;

namespace eBookStore.Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string UserId { get; set; }
        public AddressDto ShippingAddress { get; set; }
        public AddressDto BillingAddress { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
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
