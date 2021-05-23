using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Business.Models
{
    public class SenderBatchHeaderInputModel
    {
        string sender_batch_id { get; set; }
        string recipient_type { get; set; }
        string email_subject { get; set; }
        string email_message { get; set; }
    }
}
