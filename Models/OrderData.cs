using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class OrderData
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
