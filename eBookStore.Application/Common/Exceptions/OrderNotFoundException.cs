﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.Common.Exceptions
{
    public class OrderNotFoundException : Exception
    {
        public OrderNotFoundException(string message) : base(message)
        {
        }
    }
}
