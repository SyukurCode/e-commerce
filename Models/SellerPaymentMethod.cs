using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class SellerPaymentMethod
    {
        [Key]
        public int Id { get; set; }
        public required int PaymentMethodId { get; set; }
        public required string UserId { get; set; }
        public EUser? User { get; set; }
        [Display(Name ="Enable")]
        public bool isEnable { get; set; }
    }
}
