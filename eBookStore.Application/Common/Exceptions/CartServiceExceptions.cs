using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.Common.Exceptions
{
    public class CartServiceException : Exception
    {
        public CartServiceException(string message) : base(message)
        {
        }

        public CartServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
