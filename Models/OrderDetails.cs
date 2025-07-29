using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        [Display(Name="Total")]
        public List<int>? SelectedOptionIds { get; set; }
    }
}
