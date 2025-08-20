using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class StoreConfig
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Store Name")]
        [MaxLength(50)]
        public string StoreName { get; set; }
        [Display(Name = "Status")]
        public bool IsOpen { get; set; }
        [Display(Name = "Owner")]
        public string OwnerName { get; set; }

    }
}
