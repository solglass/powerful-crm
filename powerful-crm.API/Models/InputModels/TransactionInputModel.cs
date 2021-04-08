using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API.Models.InputModels
{
    public class TransactionInputModel
    {
        [Range(0, int.MaxValue)]
        public int LeadId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
