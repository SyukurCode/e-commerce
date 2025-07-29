using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class Avatar
    {
        [Key]
        public int Id { get; set; }

        // Store image as BLOB
        public byte[]? AvatarImage { get; set; }

        // Optional MIME type
        public string? AvatarContentType { get; set; }

        public string? UserId { get; set; }

    }
}
