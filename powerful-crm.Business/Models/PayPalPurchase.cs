using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace powerful_crm.Business.Models
{
    public class PayPalPurchase
    {
        [JsonPropertyName("amount")]
        public Amount Amount { get; set; }
    }
}
