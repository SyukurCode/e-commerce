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
        public required string StoreName { get; set;}
        [Display(Name = "Status")]
        public bool IsActive { get; set; } = true;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } 
    }
}
