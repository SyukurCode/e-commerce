using Microsoft.AspNetCore.Hosting;
using static System.Net.Mime.MediaTypeNames;
using PdfiumViewer;
using System.Drawing.Imaging;
using Sprache;

namespace E_Commers_Adelia.Common
{
    public static class UploadFileHelper
    {
        private static readonly string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
        public static async Task<string> Upload(IFormFile file, string folder, IWebHostEnvironment env, string LimitSize)
        {

            long lismitSizeInByte = SizeConverter.ConvertFromString(LimitSize);

            // Valication for empty file
            if (file == null && file.Length == 0)
            {
                throw new ArgumentException("File is empty.");
            }

            // Check extension
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                throw new InvalidOperationException("Fail type not allowed.");
            }

            // Hadkan saiz (contoh: 5MB)
            if (file.Length > 5 * 1024 * 1024)
                throw new InvalidOperationException("Fail must below than 5MB.");

            // Check and create path
            var uploadsFolder = Path.Combine(env.WebRootPath, "Upload", folder);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Guna nama fail unik
            var uniqueFileName = string.Empty;
            
            if (ext == ".pdf")
            {
                uniqueFileName = Guid.NewGuid().ToString() + ".png";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                // Convert PDF ke image
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    ms.Position = 0;

                    using (var pdfDocument = PdfiumViewer.PdfDocument.Load(ms))
                    {
                        var image = pdfDocument.Render(0, 300, 300, true); // page pertama, 300dpi
                        image.Save(filePath, ImageFormat.Png);
                    }
                }
            }
            else
            {
                uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            
            return Path.Combine("Upload", folder, uniqueFileName);
        }
    }
}
