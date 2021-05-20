using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Core.PayPal
{
    class PayPalAccessToken
    {
        public string Scope { get; set; }
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string AppId { get; set; }
        public int ExpiresIn { get; set; }
        public string Nonce { get; set; }
    }
}
