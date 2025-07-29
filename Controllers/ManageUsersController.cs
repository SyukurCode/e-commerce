using E_Commers_Adelia.Migrations;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Serilog;

namespace E_Commers_Adelia.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageUsersController : Controller
    {
        private readonly UserManager<EUser> _userManager;
        public ManageUsersController(UserManager<EUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var dbUser = await _userManager.Users.ToListAsync();
            return View(dbUser);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    Log.Error("Unexpected error occurred deleting user.");
                    TempData["SuccessMessage"] = "Unexpected error occurred deleting user.";
                }
                else
                {
                    Log.Information("User {Email} was deleted successfully.", user.Email);
                    TempData["SuccessMessage"] = string.Format("User {0} was deleted successfully", user.Email);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatusUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (user.IsActive)
            {
                user.IsActive = false; // Disable the user
            }
            else
            {
                user.IsActive = true; // Enable the user
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                if (user.IsActive)
                {
                    TempData["SuccessMessage"] = string.Format("Account {0} was enabled", user.Email);
                }
                else
                {
                    TempData["SuccessMessage"] = string.Format("Account {0} was disabled", user.Email);
                }   
                return RedirectToAction("Index");

            }
            TempData["SuccessMessage"] = string.Format("Error disable user, {0}",result.Errors);
            return RedirectToAction("Index");
        }
    }
}
