using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace powerful_crm.Core.PayPal.Models
{
    public class PayoutItems
    {
        [JsonProperty("payout_item_id")]
        public string  payoutItemId { get; set; }
        
        [JsonProperty("transaction_id")]
        public string transactionId { get; set; }
        
        [JsonProperty("transaction_status")]
        public string transactionStatus { get; set; }
        
        [JsonProperty("payout_batch_id ")]
        public string payoutBatchId { get; set; }
    }
}
