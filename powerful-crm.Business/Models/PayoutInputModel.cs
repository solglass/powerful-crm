using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Business.Models
{
   public class PayoutInputModel
    {
        public SenderBatchHeaderInputModel SenderBatchHeader { get; set; }
        public  List<ItemInputModel> Items { get; set; }
    }
}
