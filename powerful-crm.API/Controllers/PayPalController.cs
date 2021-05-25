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
        [HttpGet]
        public ActionResult GetToken()
        {
            return Ok(_payPalService.GetToken());
        }

        public async Task<ActionResult<List<Payout>>> CreateBatchPayoutAsync(string sender_batch_id, string receiverEmail, TransactionInputModel transaction)
        {
            var payout = new PayoutInputModel
            {
                SenderBatchHeader = new SenderBatchHeaderInputModel {
                    Sender_batch_id = sender_batch_id,
                    Recipient_type =  "EMAIL",
                    Email_message = _email_subject, 
                    Email_subject = _email_message,
                },
                Items = new List<ItemInputModel> {
                    new  ItemInputModel{
                        Amount =
                            new AmountInputModel {
                            Value = transaction.Amount,
                            Currency = transaction.Currency.ToString() },

                        Recepient_wallet = "test",
                        Sender_item_id = _sender_item_id,
                        Receiver = receiverEmail
                     }
                }
            };

            var payoutResult = await _payPalService.CreateBatchPayoutAsync(payout);
            return Ok(payoutResult);
        }


    }
}
