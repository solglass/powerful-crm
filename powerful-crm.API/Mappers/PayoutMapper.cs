using powerful_crm.Business.Models;
using powerful_crm.API.Models.InputModels;
using System.Collections.Generic;

namespace powerful_crm.Business.Mappers
{

    public class PayoutMapper
    {

        private const string _email_subject = "You have money from CRM!";
        private const string _email_message = "You received a payment from CRM. Thanks for using our service!";
        private const string _recipient_type = "EMAIL";
        private const string _sender_item_id = "201403140001";
        private const string _recipient_wallet = "PAYPAL";
        public PayoutInputModel FromTransactionInputModel(ExtendedTransactionInputModel inputModel)
        {
            var payout = new PayoutInputModel
            {
                SenderBatchHeader = new SenderBatchHeaderInputModel
                {
                    SenderBatchId = inputModel.IDPaypalBatch,
                    RecipientType = "EMAIL",
                    EmailMessage = _email_subject,
                    EmailSubject = _email_message,
                },
                Items = new List<ItemInputModel> {
                    new  ItemInputModel{
                        Amount =
                            new AmountPayoutInputModel {
                            Value = inputModel.Amount,
                            Currency = inputModel.Currency.ToString() },

                        RecepientWallet = "test",
                        SenderItemId = _sender_item_id,
                        Receiver = inputModel.LeadPayPalEmail
                     }
                }
            };
            return payout;
        }

    }

}
