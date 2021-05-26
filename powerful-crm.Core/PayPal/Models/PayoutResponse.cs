using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace powerful_crm.Core.PayPal.Models
{
    public class PayoutResponse
    {
        [JsonProperty("batch_header")]
        public PayoutBatchHeader BatchHeader { get; set; }
        
        
        [JsonProperty("links")]
        public List<Payout_links> Links { get; set; }

    }
}
