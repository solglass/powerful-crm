using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace powerful_crm.Business.Models
{
    public class AmountPayoutInputModel
    {
        [JsonPropertyName("value")]
        public decimal Value { get; set; }
        
        [JsonPropertyName("currency")]
        public string Currency { get; set; }
    }
}
