namespace powerful_crm.API.Models.MiddleModels
{
    public class TransferMiddleModel
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyPair { get; set; }
    }
}
