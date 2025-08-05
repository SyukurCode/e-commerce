using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Identity;
using E_Commers_Adelia.Common;
using Serilog;
using Microsoft.AspNetCore.Authorization;
namespace E_Commers_Adelia.Controllers
{
    [Authorize]
    public class SellerDeliveryOptionsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<EUser> _userManager;

        public SellerDeliveryOptionsController(ApplicationDbContext context, UserManager<EUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        // GET: SellerDeliveryOptions
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            // Add all option if not create yet
            var dbDeliveryOption = await _db.SellerDeliveryOptions.Where(p => p.userId == userId).ToListAsync();
            if (dbDeliveryOption == null || dbDeliveryOption.Count == 0)
            {
                Log.Information("Add delivery option");
                foreach (var item in DeliveryOption.All)
                {
                    var sDeliveryOption = new SellerDeliveryOption
                    {
                        DeliveryId = item.Id,
                        userId = userId,
                        isEnable = false
                    };
                    await _db.SellerDeliveryOptions.AddAsync(sDeliveryOption);
                    await _db.SaveChangesAsync();
                }
            }
            // Check new data is missing
            else
            {
                Log.Information("Delivery option, find missing and add if found");
                if (dbDeliveryOption.Count() != DeliveryOption.All.Count())
                {
                    foreach (var item in PaymentMethod.All)
                    {
                        var Exist = await _db.SellerDeliveryOptions.FirstOrDefaultAsync(p => p.DeliveryId == item.Id);
                        if (Exist == null)
                        {
                            var sDeliveryOption = new SellerDeliveryOption
                            {
                                DeliveryId = item.Id,
                                userId = userId,
                                isEnable = false,
                                
                            };
                            await _db.SellerDeliveryOptions.AddAsync(sDeliveryOption);
                            await _db.SaveChangesAsync();
                        }
                    }
                }
            }
            return View(await _db.SellerDeliveryOptions.Where(P => P.userId == userId).OrderBy(p => p.Id).ToListAsync());
        }

        // GET: SellerDeliveryOptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                string errMsg = "Delivery option id not found";
                Log.Error(errMsg);
                TempData["DialogError"] = errMsg;
                return RedirectToAction("Index");
            }

            var sellerDeliveryOption = await _db.SellerDeliveryOptions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sellerDeliveryOption == null)
            {
                string errMsg = "Delivery option id not found";
                Log.Error(errMsg);
                TempData["DialogError"] = errMsg;
                return RedirectToAction("Index");
            }

            return View(sellerDeliveryOption);
        }

        // GET: SellerDeliveryOptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["DialogError"] = "Delivery option not found";
                return RedirectToAction("Index");
            }

            var model = await _db.SellerDeliveryOptions.FindAsync(id);
            if (model == null)
            {
                TempData["DialogError"] = "Delivery option not found";
                return RedirectToAction("Index");
            }
            if(model.DeliveryId == DeliveryOption.SelfPickup.Id)
            {
                return RedirectToAction("EditSelftPickup", new { id = id});
            }
            if (model.DeliveryId == DeliveryOption.StandardDelivery.Id)
            {
                return View(model);
            }
            TempData["DialogError"] = "Delivery option not found";
            return RedirectToAction("Index");
        }

        // POST: SellerDeliveryOptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DeliveryId,AdditionalPrice,userId,isEnable")] SellerDeliveryOption model)
        {

            if (id != model.Id)
            {
                TempData["DialogError"] = "Delivery option not found";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(model);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SellerDeliveryOptionExists(model.Id))
                    {
                        TempData["DialogError"] = "Delivery option not found";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        // GET: SellerDeliveryOptions/Edit/5
        public async Task<IActionResult> EditSelftPickup(int? id)
        {
            if (id == null)
            {
                TempData["DialogError"] = "Delivery option not found";
                return RedirectToAction("Index");
            }

            var sellerDeliveryOption = await _db.SellerDeliveryOptions.FindAsync(id);
            if (sellerDeliveryOption == null)
            {
                TempData["DialogError"] = "Delivery option not found";
                return RedirectToAction("Index");
            }
            var user = await _userManager.GetUserAsync(User);
            var selfPickupModel = await _db.selfPickupAddresses.FirstOrDefaultAsync(a => a.UserId == sellerDeliveryOption.userId);

            if (selfPickupModel == null)
            {
                selfPickupModel = new SelfPickupAddress
                {

                    UserId = sellerDeliveryOption.userId,
                    PhoneNo = user.PhoneNumber,
                    Address = user.Address
                };
            }

            var model = new EditSelfPickupView
            {
                DeliveryOpt = sellerDeliveryOption,
                Selfpickup = selfPickupModel
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSelftPickup(EditSelfPickupView model)
        {
            if (ModelState.IsValid)
            {
                var Selftpickup = await _db.selfPickupAddresses.FirstOrDefaultAsync(s => s.UserId == model.DeliveryOpt.userId);    
                if(Selftpickup == null)
                {
                    _db.selfPickupAddresses.Add(model.Selfpickup);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    if (Selftpickup.PhoneNo != model.Selfpickup.PhoneNo)
                    {
                        Selftpickup.PhoneNo = model.Selfpickup.PhoneNo;
                    }
                    if (Selftpickup.Address != model.Selfpickup.Address)
                    {
                        Selftpickup.Address = model.Selfpickup.Address;
                    }
                    _db.selfPickupAddresses.Update(Selftpickup);
                    await _db.SaveChangesAsync();
                }

                    try
                    {
                        _db.Update(model.DeliveryOpt);
                        await _db.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Payment option successfully update!.";
                        return RedirectToAction(nameof(Index));
                }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!SellerDeliveryOptionExists(model.DeliveryOpt.Id))
                        {
                            TempData["DialogError"] = "Delivery option not found";
                            return View(model);
                    }
                        else
                        {
                            throw;
                        }
                    }
            }
            return View(model);
        }
        private bool SellerDeliveryOptionExists(int id)
        {
            return _db.SellerDeliveryOptions.Any(e => e.Id == id);
        }
    }
}
