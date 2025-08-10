using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class QrCode
    {
        [Key]
        public int Id { get; set; }

        // Store image as BLOB
        public byte[]? QrImage { get; set; }

        // Optional MIME type
        public string? ImageContentType { get; set; }

        public string UserId { get; set; }
        public EUser? User { get; set; }

    }
}
