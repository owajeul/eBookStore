using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.Common.Dto
{
    public class BookWithGenresDto
    {
        public List<BookDto> Books { get; set; }
        public List<string> Genres { get; set; }
    }
}
