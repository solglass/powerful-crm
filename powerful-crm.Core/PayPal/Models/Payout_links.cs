using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace powerful_crm.Core.PayPal.Models
{
    public class Payout_links
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }
        [JsonPropertyName("rel")]
        public string Rel { get; set; }
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("encType")]
        public string EncType { get; set; }
    }
}
