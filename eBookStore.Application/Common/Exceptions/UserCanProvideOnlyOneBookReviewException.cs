﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.Common.Exceptions;
public class UserCanProvideOnlyOneBookReviewException: Exception
{
    public UserCanProvideOnlyOneBookReviewException(string message): base(message)
    {
        
    }
}
