using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace powerful_crm.Business.Models
{
   
   public class PayoutInputModel
    {
        [JsonPropertyName("sender_batch_header")]
        public SenderBatchHeaderInputModel SenderBatchHeader { get; set; }
        
        [JsonPropertyName("items")]
        public  List<ItemInputModel> Items { get; set; }
    }
}
