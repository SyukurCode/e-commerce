using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commers_Adelia.Controllers
{
    public class QrCodeController : Controller
    {
        private readonly UserManager<EUser> _userManager;
        private readonly ApplicationDbContext _db;

        public QrCodeController(UserManager<EUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> UploadQrCode(IFormFile qrCodeFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || qrCodeFile == null || qrCodeFile.Length == 0)
                return false;

            using var ms = new MemoryStream();
            await qrCodeFile.CopyToAsync(ms);
            var imageBytes = ms.ToArray();

            var existingQrCode = await _db.QrCodes.FirstOrDefaultAsync(a => a.UserId == user.Id);

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
                    UserId = user.Id,
                    QrImage = imageBytes,
                    ImageContentType = qrCodeFile.ContentType
                };
                _db.QrCodes.Add(_qrCode);
            }

            await _db.SaveChangesAsync();
            return true;
        }

        [HttpGet]
        public async Task<IActionResult> GetQrCode(string userId)
        {
            var qrCode = await _db.QrCodes.FirstOrDefaultAsync(a => a.UserId == userId);
            if (qrCode != null && qrCode.QrImage != null)
            {
                return File(qrCode.QrImage, qrCode.ImageContentType ?? "image/jpeg");
            }

            var defaultAvatarPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "blank.jpg");
            if (System.IO.File.Exists(defaultAvatarPath))
            {
                var defaultImage = await System.IO.File.ReadAllBytesAsync(defaultAvatarPath);
                return File(defaultImage, "image/png");
            }
            return NotFound();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> DeleteQrCode()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return false;

            var qrCode = await _db.QrCodes.FirstOrDefaultAsync(a => a.UserId == user.Id);
            if (qrCode != null)
            {
                _db.QrCodes.Remove(qrCode);
                await _db.SaveChangesAsync();
            }
            return true;
        }
    }
}
