using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace powerful_crm.Core.PayPal.Models
{
    public class PayoutItems
    {
        [JsonPropertyName("payout_item_id")]
        public string  payoutItemId { get; set; }
        
        [JsonPropertyName("transaction_id")]
        public string transactionId { get; set; }
        
        [JsonPropertyName("transaction_status")]
        public string transactionStatus { get; set; }
        
        [JsonPropertyName("payout_batch_id ")]
        public string payoutBatchId { get; set; }
    }
}
