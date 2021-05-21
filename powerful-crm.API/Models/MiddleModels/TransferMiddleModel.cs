namespace powerful_crm.API.Models.MiddleModels
{
    public class TransferMiddleModel
    {
        public AccountMiddleModel SenderAccount { get; set; }
        public AccountMiddleModel RecipientAccount { get; set; }
        public decimal Amount { get; set; }
    }
}
