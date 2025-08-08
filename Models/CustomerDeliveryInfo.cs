using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace E_Commers_Adelia.Models
{
    public class CustomerDeliveryInfo
    {
        [Key]
        public int Id { get; set; }
        public string CustomerId { get; set; }
        [Display(Name = "Name")]
        public string CustomerName { get; set; }
        [Display(Name ="Phone")]
        public string CustomerPhone { get; set; }
        [Display(Name ="Email Address")]
        public string CustomerEmail { get; set; }
        [Display(Name ="Address (Unit no)")]
        public string Address { get; set; }
    }
}
