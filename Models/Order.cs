using Microsoft.Build.Construction;
using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class Order
    {
        [Key]
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public int ProductId { get; set; }
        public List<int> SelectedOptionId { get; set; }
        public int Quntity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime PlaceDateTime { get; set; }
        public string Status { get; set; }
        public Product Product { get; set; }
    }
}
