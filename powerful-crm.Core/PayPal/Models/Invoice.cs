using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Core.PayPal.Models
{
    public class Invoice
    {
        public String Id { get; set; }
        public String Status { get; set; }
        public dynamic Detail { get; set; }
        public dynamic Invoicer { get; set; }
        public dynamic Primary_recipients { get; set; }
        public dynamic Items { get; set; }
        public dynamic Configuration { get; set; }
        public dynamic Amount { get; set; }
        public dynamic Due_amount { get; set; }
        public dynamic Links { get; set; }
    }
}
