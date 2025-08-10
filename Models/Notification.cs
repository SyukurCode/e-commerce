using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class Notification
    {
        [Key]
        public long Id { get; set; }
        public string FaIcon { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
        public string? ActionURL { get; set; }
        public bool IsRead { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateRead { get; set; }
    }
}
