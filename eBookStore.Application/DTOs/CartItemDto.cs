using eBookStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.DTOs
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int BookId { get; set; }
        public BookDto Book { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
