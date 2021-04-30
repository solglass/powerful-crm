namespace powerful_crm.API.Models.OutputModels
{
    public class TransactionOutputModel
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string Timestamp { get; set; }
    }
}
