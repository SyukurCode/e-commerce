using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class OnlineTransferNote
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [Display(Name ="Account Number")]
        public required string AccountNumber { get; set; }
        [Display(Name ="Bank Name")]
        public required string BankName { get; set; }
        public required string AccounOwnerName { get; set; }
    }
}
