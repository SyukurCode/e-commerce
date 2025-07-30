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
using Microsoft.AspNetCore.Authorization;

namespace E_Commers_Adelia.Controllers
{
    [Authorize]
    public class SellerPaymentMethodsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<EUser> _userManager;

        public SellerPaymentMethodsController(ApplicationDbContext context, UserManager<EUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        // GET: SellerPaymentMethods
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            // Add all option if not create yet
            var dbPaymentMethod = await _db.SellerPaymentMethods.Where(p => p.UserId == userId).ToListAsync();
            if (dbPaymentMethod == null || dbPaymentMethod.Count == 0)
            {
                foreach(var item in PaymentMethod.All)
                {
                    var sPaymentMethode = new SellerPaymentMethod {
                        PaymentMethodId = item.Id,
                        UserId = userId,
                        isEnable = true
                    };
                    await _db.SellerPaymentMethods.AddAsync(sPaymentMethode);
                    await _db.SaveChangesAsync();
                }
            }
            // Check new data is missing
            else {
                if (dbPaymentMethod.Count() != PaymentMethod.All.Count())
                {
                    foreach (var item in PaymentMethod.All)
                    {
                        var Exist = await _db.SellerPaymentMethods.FirstOrDefaultAsync(p => p.PaymentMethodId ==  item.Id);
                        if (Exist == null)
                        {
                            var sPaymentMethode = new SellerPaymentMethod
                            {
                                PaymentMethodId = item.Id,
                                UserId = userId,
                                isEnable = false
                            };
                            await _db.SellerPaymentMethods.AddAsync(sPaymentMethode);
                            await _db.SaveChangesAsync();
                        }
                    }
                }
            }
            return View(await _db.SellerPaymentMethods.Where(P => P.UserId == userId).OrderBy(p => p.Id).ToListAsync());
        }

        // GET: SellerPaymentMethods/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sellerPaymentMethod = await _db.SellerPaymentMethods
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sellerPaymentMethod == null)
            {
                return NotFound();
            }

            return View(sellerPaymentMethod);
        }

        // GET: SellerPaymentMethods/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError(string.Empty, "Payment option not found");
            }

            var sellerPaymentMethod = await _db.SellerPaymentMethods.FindAsync(id);
            if (sellerPaymentMethod == null)
            {
                ModelState.AddModelError(string.Empty, "Payment option not found");
            }
            return View(sellerPaymentMethod);
        }

        // POST: SellerPaymentMethods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PaymentMethodId,UserId,isEnable")] SellerPaymentMethod sellerPaymentMethod)
        {
            if (id != sellerPaymentMethod.Id)
            {
                ModelState.AddModelError(string.Empty, "Payment option not found");
            }
            if (ModelState.IsValid) {
                try
                {
                    _db.Update(sellerPaymentMethod);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SellerPaymentMethodExists(sellerPaymentMethod.Id))
                    {
                        ModelState.AddModelError(string.Empty, "Payment option not found");
                        return View(sellerPaymentMethod);
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sellerPaymentMethod);
        }
        private bool SellerPaymentMethodExists(int id)
        {
            return _db.SellerPaymentMethods.Any(e => e.Id == id);
        }
    }
}
