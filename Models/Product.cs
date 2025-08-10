using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commers_Adelia.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Display(Name="User Id")]
        public required string userId { get; set;  }
        public EUser? User { get; set; }
        [MaxLength(20)]
        public required string Name { get; set; }
        [MaxLength(50, ErrorMessage = "Description can't exceed 30 character")]
        public string? Description { get; set; }
        [Display(Name = "Picture")]
        public string? ImageUrl { get; set; }
        [Display(Name="Price(RM)")]
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public List<ProductOption>? Options { get; set; }
        [Display(Name="Status")]
        public bool isEnable { get; set; }
        [Display(Name= "Hide")]
        public bool isHide { get; set; }
        [Display(Name="Add Date")]
        public DateTime CreateDate { get; set; }
        [Display(Name="Update Date")]
        public DateTime UpdateDate { get; set; }
    }
}
