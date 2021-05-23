using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Core.PayPal.Models
{
    public class Payout
    {
        public dynamic Batch_header { get; set; }
        public dynamic Items { get; set; }
        public dynamic Links { get; set; }

    }
}
