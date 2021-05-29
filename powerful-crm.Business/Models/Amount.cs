using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace powerful_crm.Business.Models
{
    public class Amount
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
        [JsonPropertyName("currency_code")]
        public string Currency_code { get; set; }
    }
}
