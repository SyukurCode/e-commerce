using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class CashNote
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Note { get; set; }
    }
}
