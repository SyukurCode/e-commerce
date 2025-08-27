using E_Commers_Adelia.Data;
using E_Commers_Adelia.Migrations;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NuGet.Packaging.Signing;
using Serilog;

namespace E_Commers_Adelia.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageUsersController : Controller
    {
        private readonly UserManager<EUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ManageUsersController(UserManager<EUser> userManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _db = db;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var dbUser = await _userManager.Users.ToListAsync();
            return View(dbUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
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
            TempData["SuccessMessage"] = string.Format("Error disable user, {0}", result.Errors);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> ChangeRole(string id)
        {
            var allRoles = _roleManager.Roles.Select(x => x.Name).ToList();
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userRoles = new UserRole
                {
                    EUser = user,
                    Roles = roles.ToList()
                };

                // hantar senarai roles ke View
                ViewBag.AllRoles = allRoles.Select(r => new SelectListItem
                {
                    Value = r,
                    Text = r,
                    Selected = roles.Contains(r)
                }).ToList();

                return View(userRoles);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(UserRole model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.EUser.Id);
                if (user != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    // roles baru dipilih
                    var selectedRoles = model.Roles ?? new List<string>();

                    // buang roles yang dah tak dipilih
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
                    if (!removeResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Failed to remove roles");
                        return View(model);
                    }

                    // tambah roles baru yang belum ada
                    var addResult = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
                    if (!addResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Failed to add roles");
                        return View(model);
                    }
                }
            }

            return RedirectToAction("Index"); // balik ke senarai user
        }
    }
}
