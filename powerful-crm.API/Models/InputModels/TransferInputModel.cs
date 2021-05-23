using powerful_crm.API.Attributes;
using System.ComponentModel.DataAnnotations;

namespace powerful_crm.API.Models.InputModels
{
    public class TransferInputModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int SenderAccountId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int RecipientAccountId { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
