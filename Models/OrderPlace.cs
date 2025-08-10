using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class OrderPlace
    {
        [Required(ErrorMessage = "Please choose payment option.")]
        public int PaymentTypeId { get; set; }
        [Required(ErrorMessage = "Please choose delivery option.")]
        public int DeliveryTypeId { get; set; }
    }
}
