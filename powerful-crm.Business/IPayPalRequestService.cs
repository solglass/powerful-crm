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
        Task <PayoutResponse> CreateBatchPayoutAsync(PayoutInputModel inputModel);
        Task<OrderOutPutModel> CreateOrder(PayPalOrderInputModel payPalOrderInputModel);
        void  TakeComission(ref PayoutInputModel inputModel);
    }
}
