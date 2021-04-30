namespace powerful_crm.API.Models.MiddleModels
{
    public class TransactionMiddleModel
    {
        public int LeadId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyPair { get; set; }
    }
}
