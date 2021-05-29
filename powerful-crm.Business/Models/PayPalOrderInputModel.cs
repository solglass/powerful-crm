using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace powerful_crm.Business.Models
{
    public class PayPalOrderInputModel
    {
        [JsonPropertyName("amount")]
        public int Amount { get; set; }
        public string Currency { get; set; }
    }
}
