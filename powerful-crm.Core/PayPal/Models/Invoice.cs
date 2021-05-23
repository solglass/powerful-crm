using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Core.PayPal.Models
{
    public class Invoice
    {
        String Id { get; set; }
        String Status { get; set; }
        dynamic Detail { get; set; }
        dynamic Invoicer { get; set; }
        dynamic Primary_recipients { get; set; }
        dynamic Items { get; set; }
        dynamic Configuration { get; set; }
        dynamic Amount { get; set; }
        dynamic Due_amount { get; set; }
        dynamic Links { get; set; }
    }
}
