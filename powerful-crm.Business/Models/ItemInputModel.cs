using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Business.Models
{
    class ItemInputModel
    {
        AmountInputModel Amount { get; set; }
        string Sender_item_id { get; set; }
        string recepient_wallet { get; set; }
        string receiver { get; set; }
}
}
