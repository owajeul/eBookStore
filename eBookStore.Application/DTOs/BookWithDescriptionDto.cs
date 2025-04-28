using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.DTOs
{
    public class BookWithDescriptionDto: BookDto
    {
        public string Description { get; set; }
    }
}
