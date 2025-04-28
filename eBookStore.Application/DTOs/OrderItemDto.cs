using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eBookStore.Domain.Entities;

namespace eBookStore.Application.DTOs
{
    public class OrderItemDto
    {
        public int BookId { get; set; }
        public BookDto Book { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
