using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Core.Settings
{
    public class PayPalSettings
    {
        public string POWERFUL_CRM_PAYPAL_USERNAME { get; set; }
        public string POWERFUL_CRM_PAYPAL_PASSWORD { get; set; }
        public string POWERFUL_CRM_PAYPAL_BASE_URL { get; set; }       
        public decimal POWERFUL_CRM_COMMISSION_PERCENT { get; set; }
    }
}
