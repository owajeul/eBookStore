﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.Common.Utilily
{
    public static class AppConstant
    {
        public const string Role_Customer = "Customer";
        public const string Role_Admin = "Admin";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusDelivered = "Delivered";
        public const string StatusCancelled = "Cancelled";

        public static List<string> ValidStatuses = new List<string>
        {
            StatusPending,
            StatusApproved,
            StatusDelivered,
            StatusCancelled
        };
    }
}

