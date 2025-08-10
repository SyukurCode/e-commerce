using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class SellerDeliveryOption
    {
        public int Id { get; set; }
        public required int DeliveryId { get; set; }
        public required string userId { get; set; }
        public EUser? User { get; set; }
        public decimal AdditionalPrice { get; set; }
        [Display(Name ="Enable")]
        public bool isEnable { get; set; }
    }
}
