using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Business.Models
{
    public class ItemInputModel
    {
        public AmountInputModel Amount { get; set; }
        public string Sender_item_id { get; set; }
        public string Recepient_wallet { get; set; }
        public  string Receiver { get; set; }
}
}
