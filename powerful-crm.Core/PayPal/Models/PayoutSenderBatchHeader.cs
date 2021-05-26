using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace powerful_crm.Core.PayPal.Models
{
    public class PayoutSenderBatchHeader
    {
        [JsonPropertyName("sender_batch_id")]
        public string SenderBatchId { get; set; }
        
        [JsonPropertyName("recipient_type")]
        public string RecipientType { get; set; }

        [JsonPropertyName("email_subject")]
        public string EmailSubject { get; set; }

        [JsonPropertyName("email_message")]
        public string EmailMessage { get; set; }
    }
}
