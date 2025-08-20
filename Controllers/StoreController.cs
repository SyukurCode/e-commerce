using AspNetCoreGeneratedDocument;
using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.CodeDom;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace E_Commers_Adelia.Controllers
{
    [Authorize(Roles = "Admin,Seller")]
    public class StoreController : Controller
    {
        private readonly UserManager<EUser> _userManager;
        private readonly ApplicationDbContext _db;


        public StoreController(UserManager<EUser> userManager, ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                StoreConfig storeConfig = new StoreConfig
                {
                    StoreName = currentUser.StoreName,
                    OwnerName = currentUser.DisplayName,
                    IsOpen = currentUser.IsOpen,

                };
                return View(storeConfig);
            }
            return View();
        }
        public async Task<IActionResult> Edit()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                StoreConfig storeConfig = new StoreConfig
                {
                    StoreName = currentUser.StoreName,
                    OwnerName = currentUser.DisplayName,
                    IsOpen = currentUser.IsOpen,

                };
                return View(storeConfig);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StoreConfig model)
        {
            var dbUser = await _userManager.GetUserAsync(User);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (dbUser != null)
            {
                // validate payment and delivery was setup
                if (model.IsOpen)
                {
                    // validate payment option
                    var paymetOption = await _db.SellerPaymentMethods.Where(x => x.UserId == dbUser.Id).ToListAsync();
                    if (paymetOption == null)
                    {
                        TempData["DialogWarning"] = "Please setup payment method";
                        return RedirectToAction("Index", "SellerPaymentMethods");
                    }
                    var totalPaymentEnable = paymetOption.Where(x => x.isEnable == true).Count();
                    if (totalPaymentEnable == 0)
                    {
                        TempData["DialogWarning"] = "Please turn on atleast one payment option";
                        return RedirectToAction("Index", "SellerPaymentMethods");
                    }

                    // Validate delivery option
                    var deliveryOption = await _db.SellerDeliveryOptions.Where(x => x.userId == dbUser.Id).ToListAsync();
                    if (deliveryOption == null)
                    {
                        TempData["DialogWarning"] = "Please setup delivery method";
                        return RedirectToAction("Index", "SellerDeliveryOptions");
                    }
                    var totalDeliveryEnable = deliveryOption.Where(x => x.isEnable == true).Count();
                    if (totalDeliveryEnable == 0)
                    {
                        TempData["DialogWarning"] = "Please turn on atleast one delivery option";
                        return RedirectToAction("Index", "SellerDeliveryOptions");
                    }

                    // validate product
                    var product = await _db.Products.Where(x => x.userId == dbUser.Id).ToArrayAsync();
                    var totalProduct = product.Count();
                    if (totalProduct == 0)
                    {
                        TempData["DialogWarning"] = "Your have nothing to sell, please add product";
                        return RedirectToAction("Index", "Products");
                    }
                }

                dbUser.StoreName = model.StoreName;
                dbUser.DisplayName = model.OwnerName;
                dbUser.IsOpen = model.IsOpen;

                var updateStoreData = await _userManager.UpdateAsync(dbUser);
                if (!updateStoreData.Succeeded)
                {
                    TempData["DialogError"] = "Unexpected error when trying to set store config.";
                    return View(model);
                }
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> toggleOpen(string userId)
        {
            var dbUser = await _userManager.FindByIdAsync(userId);
            if (dbUser != null)
            {
                dbUser.IsOpen = !dbUser.IsOpen;
                var updateOpen = await _userManager.UpdateAsync(dbUser);
                if (!updateOpen.Succeeded)
                {
                    {
                        TempData["DialogError"] = "Unexpected error when trying to set store open.";
                        return Json( new { success = false, message = "Unexpected error when trying to set store open" });
                    }

                }
                return Json(new { success = true, message = "" });
            }
            return Json(new { success = false, message = "User not found" });
        }
    }
}
