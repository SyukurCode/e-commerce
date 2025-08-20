using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class OrderHistory
    {
        [Key]
        public long Id { get; set; }
        public int StatusId { get; set; }
        public string OrderNo { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
    }
}
