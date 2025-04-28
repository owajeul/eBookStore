using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.DTOs
{
    public class TopSellingBookDto : BookDto
    {
        public int CopiesSold { get; set; }
        public decimal Revenue { get; set; }
    }
}
