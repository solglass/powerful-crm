using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace powerful_crm.Business.Models
{
   
   public class PayoutInputModel
    {
        [JsonProperty("sender_batch_header")]
        public SenderBatchHeaderInputModel SenderBatchHeader { get; set; }
        
        [JsonProperty("items")]
        public  List<ItemInputModel> Items { get; set; }
    }
}
