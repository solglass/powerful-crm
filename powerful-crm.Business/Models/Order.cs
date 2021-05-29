using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace powerful_crm.Business.Models
{
    public class Order
    {
        [JsonPropertyName("intent")]
        public string Intent { get; set; }
        [JsonPropertyName("purchase_units")]
        public List<PayPalPurchase> Purchase_units { get; set; }
    }
}
