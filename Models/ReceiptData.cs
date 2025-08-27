using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class ReceiptData
    {
        [Display(Name = "Recipient")]
        public string RecipientName { get; set; }
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }
        public decimal Amount { get; set; }
        public DateTime Datetime { get; set; }
        public string ReferenceNo { get; set; }
        [Display(Name = "Valid")]
        public bool isValid { get; set; }
        public string Explanation { get; set; }
    }
}
