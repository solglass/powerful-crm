using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API.Models.InputModels
{
    public class TransferInputModel
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
