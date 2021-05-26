using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace powerful_crm.Business.Models
{
    public class SenderBatchHeaderInputModel
    {
        [JsonProperty("sender_batch_id")]
        public string SenderBatchId { get; set; }
        
        [JsonProperty("recipient_type")]
        public string RecipientType { get; set; }
        
        [JsonProperty("email_subject")]
        public string EmailSubject { get; set; }
        
        [JsonProperty("email_message")]
        public string EmailMessage { get; set; }
    }
}
