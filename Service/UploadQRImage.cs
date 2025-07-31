using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Commers_Adelia.Service
{
    public class UploadQRImage : IUploadQRImage
    {
        private readonly UserManager<EUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly SignInManager<EUser> _signInManager;   
        public UploadQRImage(UserManager<EUser> userManager, ApplicationDbContext db, SignInManager<EUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<bool> isQRExist(string userId)
        {
            var existingQrCode = await _db.QrCodes.FirstOrDefaultAsync(q => q.UserId == userId);
            if (existingQrCode != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UploadQrCodeAsync(IFormFile qrCodeFile, string userId)
        {
            if (userId == null || qrCodeFile == null || qrCodeFile.Length == 0)
                return false;

            using var ms = new MemoryStream();
            await qrCodeFile.CopyToAsync(ms);
            var imageBytes = ms.ToArray();

            var existingQrCode = await _db.QrCodes.FirstOrDefaultAsync(q => q.UserId == userId);

            if (existingQrCode != null)
            {
                existingQrCode.QrImage = imageBytes;
                existingQrCode.ImageContentType = qrCodeFile.ContentType;
                _db.Update(existingQrCode);
            }
            else
            {
                var _qrCode = new QrCode()
                {
                    UserId = userId,
                    QrImage = imageBytes,
                    ImageContentType = qrCodeFile.ContentType
                };
                _db.QrCodes.Add(_qrCode);
            }

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
