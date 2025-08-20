using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace E_Commers_Adelia.Models
{
    public class EUser : IdentityUser
    {
        // allow 10 characters for DisplayName
        [PersonalData]
        [MaxLength(20)]
        [Required(ErrorMessage = "Display Name is required.")]
        public required string DisplayName { get; set; }
        [Display(Name = "Store Name")]
        [MaxLength(50)]
        public string? StoreName { get; set; }
        [Display(Name = "Store Status")]
        public bool IsOpen { get; set; } = false;
        [Display(Name = "Status")]
        [MaxLength (100)]
        public string? Address { get; set; }
        public bool IsActive { get; set; } = true;
        public int AccountTypeId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdateAt { get; set; }
    }
}
