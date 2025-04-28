using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.Common.Exceptions
{
    public class BookServiceException : Exception
    {
        public BookServiceException(string message) : base(message)
        {
        }

        public BookServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
