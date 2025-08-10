using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class UserChat
    {
        [Key]
        public long Id { get; set; }
        public string UserId { get; set; }
        public EUser? User { get; set; }
        public string Receiver { get; set; }
        public bool IsRead { get; set; }
        public DateTime DateSend { get; set; }
        public DateTime DateRead { get; set; }
    }
}
