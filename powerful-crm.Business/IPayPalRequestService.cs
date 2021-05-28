using powerful_crm.Business.Models;
using powerful_crm.Core.PayPal.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace powerful_crm.Business
{
    public interface IPayPalRequestService
    {
        Invoice CreateDraftInvoice();
        Task <PayoutResponse> CreateBatchPayoutAsync(PayoutInputModel inputModel);      
        Task<BraintreeHttp.HttpResponse> CreateOrder(bool debug);
    }
}
