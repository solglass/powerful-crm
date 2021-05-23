using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Business.Models
{
    public class SenderBatchHeaderInputModel
    {
        public string Sender_batch_id { get; set; }
        public string Recipient_type { get; set; }
        public string Email_subject { get; set; }
        public string Email_message { get; set; }
    }
}
