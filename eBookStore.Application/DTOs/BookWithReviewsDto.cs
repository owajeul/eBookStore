using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.DTOs
{
    public class BookWithReviewsDto: BookWithDescriptionDto
    {
        public int ReviewCount { get; set; }
        public double AverageRating { get; set; }
        public List<BookReviewDto> Reviews { get; set; } = new List<BookReviewDto>();
    }
}
