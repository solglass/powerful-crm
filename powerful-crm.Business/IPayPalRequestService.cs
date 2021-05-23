using powerful_crm.Business.Models;
using powerful_crm.Core.PayPal.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Business
{
    public interface IPayPalRequestService
    {
        Invoice CreateDraftInvoice();
        List<Payout> CreateBatchPayout(PayoutInputModel inputModel);
        string GetToken();
    }
}
