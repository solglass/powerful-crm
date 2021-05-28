using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace powerful_crm.Business.Models
{
    public class ItemInputModel
    {
        [JsonProperty("amount")]
        public AmountPayoutInputModel Amount { get; set; }
        
        [JsonProperty("sender_item_id")]
        public string SenderItemId { get; set; }
       
        [JsonProperty("recepient_wallet")]
        public string RecepientWallet { get; set; }
       
        [JsonProperty("receiver")]
        public  string Receiver { get; set; }
}
}
