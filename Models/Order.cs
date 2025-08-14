using Microsoft.Build.Construction;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace E_Commers_Adelia.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="Order No")]
        public string OrderNo { get; set; }
        public string CustomerId { get; set; }
        public string SellerId { get; set; }
        //public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public string ProductDescription { get; set; }
        public string SelectedOption { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotalPrice { get; set; }
        public DateTime PlaceDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int StatusId { get; set; }
        public int DeliveryId { get; set; }
        public decimal ExtraCharges { get; set; } // read onlyone time
    }

}
