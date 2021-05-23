using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Core.PayPal.Models
{
    public class Payout
    {
        dynamic Batch_header { get; set; }
        dynamic Items { get; set; }
        dynamic Links { get; set; }

    }
}
