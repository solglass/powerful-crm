using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Business.Models
{
   public class PayoutInputModel
    {
        SenderBatchHeaderInputModel SenderBatchHeader { get; set; }
        List<ItemInputModel> Items { get; set; }
    }
}
