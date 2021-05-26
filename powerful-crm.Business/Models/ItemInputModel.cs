using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace powerful_crm.Business.Models
{
    public class ItemInputModel
    {
        [JsonPropertyName("amount")]
        public AmountPayoutInputModel Amount { get; set; }
        
        [JsonPropertyName("sender_item_id")]
        public string SenderItemId { get; set; }
       
        [JsonPropertyName("recepient_wallet")]
        public string RecepientWallet { get; set; }
       
        [JsonPropertyName("receiver")]
        public  string Receiver { get; set; }
}
}
