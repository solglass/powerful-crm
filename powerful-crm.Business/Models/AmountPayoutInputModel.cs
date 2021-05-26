using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace powerful_crm.Business.Models
{
    public class AmountPayoutInputModel
    {
        [JsonProperty("value")]
        public decimal Value { get; set; }
        
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}
