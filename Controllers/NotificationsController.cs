using E_Commers_Adelia.Data;
using E_Commers_Adelia.Migrations;
using E_Commers_Adelia.Models;
using E_Commers_Adelia.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Threading.Tasks;

namespace E_Commers_Adelia.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotification _noti;
        private readonly UserManager<EUser> _userManager;
        public NotificationsController(INotification noti, UserManager<EUser> userManager) 
        { 
            _noti = noti;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index( long id = 0)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                if (id == 0)
                {
                    return View(await _noti.GetByUser(currentUser.Id));
                }
                var model = await _noti.GetById(id);
                if (model != null)
                {
                    await _noti.MarkAsRead(id);
                    return Redirect(model.ActionURL ?? "/Home");
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(long id)
        {
            var model = await _noti.Remove(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
