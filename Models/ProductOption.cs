using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class ProductOption
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        [Display(Name ="Name")]
        public required string OptionName { get; set; }
        [Display(Name="Addtional Price(RM)")]
        public decimal AdditionalPrice { get; set; }
        public Product? Product { get; set; }
    }
}
