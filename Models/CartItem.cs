using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class CartItem
    {
        [Key]
        public Guid Id { get; set; }
        public Product Product { get; set; }
        public List<ProductOption> SelectedOptions { get; set; } = new();
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
    }
}
