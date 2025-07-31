using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace E_Commers_Adelia.Models
{
    public class SelfPickupAddress
    {
        [Key]
        public int id { get; set; }
        public string UserId { get; set; }
        public required string Address { get; set; }
        [Display(Name ="Phone No")]
        public string PhoneNo { get; set; }
    }
}
