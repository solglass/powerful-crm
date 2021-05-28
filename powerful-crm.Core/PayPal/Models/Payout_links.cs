using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace powerful_crm.Core.PayPal.Models
{
    public class Payout_links
    {
        [JsonProperty("href")]
        public string Href { get; set; }
        [JsonProperty("rel")]
        public string Rel { get; set; }
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("encType")]
        public string EncType { get; set; }
    }
}
