using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace powerful_crm.Core.PayPal.Models
{
    public class PayoutBatchHeader
    {
        [JsonProperty("payout_batch_id")]
        public string PayoutBatchId { get; set; }

        [JsonProperty("batch_status")]
        public string BatchStatus { get; set; }
        
        
        [JsonProperty("sender_batch_header")]
        public PayoutSenderBatchHeader SenderBatchHeader { get; set; }
    }
}
