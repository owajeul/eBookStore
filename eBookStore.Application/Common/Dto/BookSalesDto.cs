using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.Common.Dto
{
    public class BookSalesDto
    {
        public int BookId { get; set; }
        public int CopiesSold { get; set; }
        public decimal Revenue { get; set; }
    }
}
