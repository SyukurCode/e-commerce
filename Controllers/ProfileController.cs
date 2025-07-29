using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commers_Adelia.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<EUser> _userManager;
        private readonly ApplicationDbContext _db;

        public ProfileController(UserManager<EUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAvatar(IFormFile avatarFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || avatarFile == null || avatarFile.Length == 0)
                return BadRequest();

            using var ms = new MemoryStream();
            await avatarFile.CopyToAsync(ms);
            var imageBytes = ms.ToArray();

            var existingAvatar = await _db.Avatars.FirstOrDefaultAsync(a => a.UserId == user.Id);

            if (existingAvatar != null)
            {
                existingAvatar.AvatarImage = imageBytes;
                existingAvatar.AvatarContentType = avatarFile.ContentType;
                _db.Update(existingAvatar);
            }
            else
            {
                var avatar = new Avatar
                {
                    UserId = user.Id,
                    AvatarImage = imageBytes,
                    AvatarContentType = avatarFile.ContentType
                };
                _db.Avatars.Add(avatar);
            }

            await _db.SaveChangesAsync();
            return Redirect("/Identity/Account/Manage");
        }

        [HttpGet]
        public async Task<IActionResult> GetAvatar(string userId)
        {
            var avatar = await _db.Avatars.FirstOrDefaultAsync(a => a.UserId == userId);
            if (avatar != null && avatar.AvatarImage != null)
            {
                return File(avatar.AvatarImage, avatar.AvatarContentType ?? "image/jpeg");
            }

            var defaultAvatarPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "default-avatar.png");
            if (System.IO.File.Exists(defaultAvatarPath))
            {
                var defaultImage = await System.IO.File.ReadAllBytesAsync(defaultAvatarPath);
                return File(defaultImage, "image/png");
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAvatar()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var avatar = await _db.Avatars.FirstOrDefaultAsync(a => a.UserId == user.Id);
            if (avatar != null)
            {
                _db.Avatars.Remove(avatar);
                await _db.SaveChangesAsync();
            }
            return Redirect("/Identity/Account/Manage");
        }
    }
}
