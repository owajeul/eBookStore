using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.Common.Exceptions
{
    public class AdminServiceException : Exception
    {
        public AdminServiceException(string message) : base(message)
        {
        }

        public AdminServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
