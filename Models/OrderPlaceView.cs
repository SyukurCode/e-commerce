using Newtonsoft.Json.Serialization;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Internal;
using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class OrderPlaceView
    {
        public string OrderNo {  get; set; }    
        public string SellerId { get; set; }
        [Required(ErrorMessage = "Please choose payment option.")]
        public int PaymentTypeId { get; set; }
        [Required(ErrorMessage = "Please choose delivery option.")]
        public int DeliveryTypeId { get; set; }
        public decimal Amount { get; set; }
        public CustomerDeliveryInfo CustomerDeliveryInfo { get; set; }
    }
}
