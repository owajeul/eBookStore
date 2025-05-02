using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.DTOs
{
    public class UserPurchaseReportDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string MostPurchasedGenre { get; set; }
        public decimal TotalSpend { get; set; }
        public int TotalBooksPurchased { get; set; }
    }

}
