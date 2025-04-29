using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.Common.Exceptions
{
    public class OrderServiceException : Exception
    {
        public OrderServiceException(string message) : base(message)
        {
        }

        public OrderServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
