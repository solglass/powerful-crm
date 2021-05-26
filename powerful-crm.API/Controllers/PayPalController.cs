using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using powerful_crm.API.Models.InputModels;
using powerful_crm.Business;
using powerful_crm.Business.Models;
using powerful_crm.Core.PayPal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PayPalController : ControllerBase
    {
        private IPayPalRequestService _payPalService;
        private const string _email_subject = "You have money from CRM!";
        private const string _email_message = "You received a payment from CRM. Thanks for using our service!";
        private const string _recipient_type = "EMAIL";
        private const string _sender_item_id = "201403140001";
        private const string _recipient_wallet = "PAYPAL";
        public PayPalController(IPayPalRequestService payPalService)
        {
            _payPalService = payPalService;
        }
        [HttpPost("payout/{sender_batch_id}/{receiverEmail}")]
        public async Task<ActionResult<List<PayoutResponse>>> CreateBatchPayoutAsync(string sender_batch_id, string receiverEmail, [FromBody] TransactionInputModel transaction)
        {
            var payout = new PayoutInputModel
            {
                SenderBatchHeader = new SenderBatchHeaderInputModel {
                    SenderBatchId = sender_batch_id,
                    RecipientType =  "EMAIL",
                    EmailMessage = _email_subject, 
                    EmailSubject = _email_message,
                },
                Items = new List<ItemInputModel> {
                    new  ItemInputModel{
                        Amount =
                            new AmountPayoutInputModel {
                            Value = transaction.Amount,
                            Currency = transaction.Currency.ToString() },

                        RecepientWallet = "test",
                        SenderItemId = _sender_item_id,
                        Receiver = receiverEmail
                     }
                }
            };

            var payoutResult = await _payPalService.CreateBatchPayoutAsync(payout);
            return Ok(payoutResult);
        }
        [AllowAnonymous]
        [HttpPost("order")]
        public async Task<BraintreeHttp.HttpResponse> CreateOrder()
        {
            return await(_payPalService.CreateOrder(false));
        }

    }
}
