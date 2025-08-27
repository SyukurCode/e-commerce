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
        public string? CustomerId { get; set; }
        public string SellerId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public string ProductDescription { get; set; }
        public string SelectedOption { get; set; }
        public int Quantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal UnitPrice { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal SubTotalPrice { get; set; }
        [Display(Name = "Order Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:ddd, dd MM yyyy h:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime PlaceDateTime { get; set; }
        [Display(Name = "Last Update")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:ddd, dd MM yyyy h:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime UpdateDateTime { get; set; }
        public int StatusId { get; set; }
        public int DeliveryId { get; set; }
        public decimal ExtraCharges { get; set; } // read onlyone time
        public string SessionId { get; set; }
    }

}
