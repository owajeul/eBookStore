using eBookStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.DTOs
{
    public class CartDto
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public List<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
