using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.Common.Exceptions
{
    public class BookOutOfStockException: Exception
    {
        public BookOutOfStockException(string message) : base(message)
        {

        }
    }
}
