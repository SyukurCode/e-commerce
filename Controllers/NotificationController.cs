using E_Commers_Adelia.Data;
using E_Commers_Adelia.Migrations;
using E_Commers_Adelia.Models;
using E_Commers_Adelia.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks;

namespace E_Commers_Adelia.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotification _noti;
        private readonly UserManager<EUser> _userManager;
        private readonly SignInManager<EUser> _signInManager;

        public NotificationController(INotification noti, SignInManager<EUser> signInManager, UserManager<EUser> userManager) 
        {
            _signInManager = signInManager;
            _noti = noti;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int id)
        {
            await _noti.MarkAsRead(id);
            var noti = await _noti.GetById(id);

            return Redirect(noti.ActionURL);
        }
        public async Task<IActionResult> MarkAllRead()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var notiList = await _noti.GetByUser(currentUser?.Id);
                if (notiList != null)
                {
                    foreach (var noti in notiList)
                    {
                        await _noti.MarkAsRead(noti.Id);
                    }
                }
            }
            return PartialView("_PartialNavBar");
        }
        public async Task<IActionResult> RemoveAll()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var notiList = await _noti.GetByUser(currentUser?.Id);
            if (notiList != null)
            {
                foreach (var noti in notiList)
                {
                    await _noti.Remove(noti.Id);
                }
            }
            
            return PartialView("_PartialNavBar");
        }
    }
}
