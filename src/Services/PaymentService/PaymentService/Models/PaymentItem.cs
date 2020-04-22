using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentService.Models
{
    public class PaymentItem
    {
        public long Id { get; set; }
        public long UserID { get; set; }
        public long OrderID { get; set; }

        public PaymentItem()
        { }

    }
}
