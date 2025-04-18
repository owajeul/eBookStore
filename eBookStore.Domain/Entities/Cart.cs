using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Domain.Entities;

public class Cart
{
    public int Id { get; set; }
    [Required]
    public string UserId { get; set; }
    public ICollection<CartItem> CartItems { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

