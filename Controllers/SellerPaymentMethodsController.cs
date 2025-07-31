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
using Microsoft.AspNetCore.Hosting;
using E_Commers_Adelia.Service;

namespace E_Commers_Adelia.Controllers
{
    [Authorize]
    public class SellerPaymentMethodsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<EUser> _userManager;
        private readonly IUploadQRImage _qr;

        public SellerPaymentMethodsController(ApplicationDbContext context, UserManager<EUser> userManager, IUploadQRImage qr)
        {
            _db = context;
            _userManager = userManager;
            _qr = qr;
        }

        // GET: SellerPaymentMethods
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            // Add all option if not create yet
            var dbPaymentMethod = await _db.SellerPaymentMethods.Where(p => p.UserId == userId).ToListAsync();
            if (dbPaymentMethod == null || dbPaymentMethod.Count == 0)
            {
                foreach (var item in PaymentMethod.All)
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
            // Check new data is missing
            else
            {
                if (dbPaymentMethod.Count() != PaymentMethod.All.Count())
                {
                    foreach (var item in PaymentMethod.All)
                    {
                        var Exist = await _db.SellerPaymentMethods.FirstOrDefaultAsync(p => p.PaymentMethodId == item.Id);
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
                TempData["DialogError"] = "Please upload QR image";
                return RedirectToAction("Index");
            }

            var sellerPaymentMethod = await _db.SellerPaymentMethods
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sellerPaymentMethod == null)
            {
                TempData["DialogError"] = "Please upload QR image";
                return RedirectToAction("Index");
            }

            return View(sellerPaymentMethod);
        }

        // GET: SellerPaymentMethods/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["DialogError"] = "Payment option not found";
                return RedirectToAction("Index");
            }
            var userId = _userManager.GetUserId(User);
            var model = await _db.SellerPaymentMethods.FindAsync(id);
            if (model == null)
            {
                TempData["DialogError"] = "Payment option not found";
                return RedirectToAction("Index");
            }

            switch (model.PaymentMethodId)
            {

                case 1:   //qr
                    return View(model);
                case 2:   //COD
                    return RedirectToAction("EditCOD", new { id = id });
                case 3:  //cash
                    return RedirectToAction("EditCash", new { id = id });
                case 4:  //Online transfer
                    return RedirectToAction("EditOnlineTransfer", new { id = id });
            }
            return RedirectToAction("Index");
        }

        // POST: SellerPaymentMethods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SellerPaymentMethod model, IFormFile ImageFile)
        {
            var qrExists = await _qr.isQRExist(model.UserId);
            if (qrExists)
            {
                ModelState.Remove("ImageFile");
            }

            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadSuccess = await _qr.UploadQrCodeAsync(ImageFile, model.UserId);
                    if (!uploadSuccess)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to upload QR code. Please try again.");
                        return View(model);
                    }
                }

                try
                {
                    _db.Update(model);
                    await _db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Payment option successfully update!.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SellerPaymentMethodExists(model.Id))
                    {
                        ModelState.AddModelError(string.Empty, "Payment option not exist.");
                        return View(model);
                    }

                    throw; // Let it bubble up if it's a real issue
                }
            }

            if (ImageFile == null || ImageFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please upload QR image");
                TempData["DialogError"] = "Please upload QR image";
            }

            return View(model);
        }
        // GET: SellerPaymentMethods/EditCOD
        public async Task<IActionResult> EditCOD(int id)
        {
            var sellerPaymentMethod = await _db.SellerPaymentMethods.FindAsync(id);
            if (sellerPaymentMethod == null)
            {
                TempData["DialogError"] = "Payment option not found";
                return RedirectToAction("Index");
            }
            var dbCODNote = await _db.CodNotes.FirstOrDefaultAsync(c => c.UserId == sellerPaymentMethod.UserId);

            var model = new EditCODView
            {
                PaymentOption = sellerPaymentMethod,
                codNote = dbCODNote
            };

            return View(model);
        }

        // POST: SellerPaymentMethods/EditCOD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCOD(int id, EditCODView model)
        {
            if (ModelState.IsValid)
            {

                var dbCODNote = await _db.CodNotes.FirstOrDefaultAsync(c => c.UserId == model.PaymentOption.UserId);
                if (dbCODNote == null)
                {
                    _db.CodNotes.Add(model.codNote);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    if (dbCODNote.Note != model.codNote.Note)
                    {
                        dbCODNote.Note = model.codNote.Note;
                        _db.CodNotes.Update(dbCODNote);
                        await _db.SaveChangesAsync();
                    }
                }


                try
                {
                    _db.Update(model.PaymentOption);
                    await _db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Payment option successfully update!.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SellerPaymentMethodExists(model.PaymentOption.Id))
                    {
                        ModelState.AddModelError(string.Empty, "Payment option not exist.");
                        return View(model);
                    }

                    throw; // Let it bubble up if it's a real issue
                }
            }
            if ((string.IsNullOrEmpty(model.codNote.Note) || string.IsNullOrWhiteSpace(model.codNote.Note)) && model.PaymentOption.isEnable)
            {
                ModelState.AddModelError(string.Empty, "Message to customer is required to enable COD payment.");
            }

            return View(model);
        }
        // GET: SellerPaymentMethods/EditCash
        public async Task<IActionResult> EditCash(int id)
        {

            var sellerPaymentMethod = await _db.SellerPaymentMethods.FindAsync(id);
            if (sellerPaymentMethod == null)
            {
                TempData["DialogError"] = "Payment option not found";
                return RedirectToAction("Index");
            }
            var dbCashNote = await _db.CashNotes.FirstOrDefaultAsync(c => c.UserId == sellerPaymentMethod.UserId);

            var model = new EditCashView
            {
                PaymentOption = sellerPaymentMethod,
                cashNote = dbCashNote
            };

            return View(model);
        }
        // POST: SellerPaymentMethods/EditCash
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCash(int id, EditCashView model)
        {
            if (ModelState.IsValid)
            {
                var dbCashNote = await _db.CashNotes.FirstOrDefaultAsync(c => c.UserId == model.PaymentOption.UserId);
                if (dbCashNote == null)
                {
                    _db.CashNotes.Add(model.cashNote);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    if (dbCashNote.Note != model.cashNote.Note)
                    {
                        dbCashNote.Note = model.cashNote.Note;
                        _db.CashNotes.Update(dbCashNote);
                        await _db.SaveChangesAsync();
                    }
                }
                try
                {
                    _db.Update(model.PaymentOption);
                    await _db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Payment option successfully update!.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SellerPaymentMethodExists(model.PaymentOption.Id))
                    {
                        ModelState.AddModelError(string.Empty, "Payment option not exist.");
                        return View(model);
                    }

                    throw; // Let it bubble up if it's a real issue
                }
            }
            if ((string.IsNullOrEmpty(model.cashNote.Note) || string.IsNullOrWhiteSpace(model.cashNote.Note)) && model.PaymentOption.isEnable)
            {
                ModelState.AddModelError(string.Empty, "Message to customer is required to enable cash payment.");
            }

            return View(model);
        }

        // GET: SellerPaymentMethods/EditCash
        public async Task<IActionResult> EditOnlineTransfer(int id)
        {
            var sellerPaymentMethod = await _db.SellerPaymentMethods.FindAsync(id);
            if (sellerPaymentMethod == null)
            {
                TempData["DialogError"] = "Payment option not found";
                return RedirectToAction("Index");
            }

            var dbOnlineTransferNote = await _db.onlineTransferNotes.FirstOrDefaultAsync(c => c.UserId == sellerPaymentMethod.UserId);
            var model = new EditOnlineTransferView
            {
                PaymentOption = sellerPaymentMethod,
                OnlineTransferDetail = dbOnlineTransferNote
            };

            return View(model);
        }
        // POST: SellerPaymentMethods/EditCash
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOnlineTransfer(int id, EditOnlineTransferView model)
        {
            if (ModelState.IsValid)
            {
                var dbOnlineTransferNote = await _db.onlineTransferNotes.FirstOrDefaultAsync(c => c.UserId == model.PaymentOption.UserId);
                if (dbOnlineTransferNote == null)
                {

                    _db.onlineTransferNotes.Add(model.OnlineTransferDetail);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    if (dbOnlineTransferNote.AccountNumber != model.OnlineTransferDetail.AccountNumber)
                    {
                        dbOnlineTransferNote.AccountNumber = model.OnlineTransferDetail.AccountNumber;
                    }
                    if (dbOnlineTransferNote.AccounOwnerName != model.OnlineTransferDetail.AccounOwnerName)
                    {
                        dbOnlineTransferNote.AccounOwnerName = model.OnlineTransferDetail.AccounOwnerName;
                    }
                    if (dbOnlineTransferNote.BankName != model.OnlineTransferDetail.BankName)
                    {
                        dbOnlineTransferNote.BankName = model.OnlineTransferDetail.BankName;
                    }

                    _db.onlineTransferNotes.Update(dbOnlineTransferNote);
                    await _db.SaveChangesAsync();
                }

                try
                {
                    _db.Update(model.PaymentOption);
                    await _db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Payment option successfully update!.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SellerPaymentMethodExists(model.PaymentOption.Id))
                    {
                        ModelState.AddModelError(string.Empty, "Payment option not exist.");
                        return View(model);
                    }

                    throw; // Let it bubble up if it's a real issue
                }
            }
            return View(model);
        }

        private bool SellerPaymentMethodExists(int id)
        {
            return _db.SellerPaymentMethods.Any(e => e.Id == id);
        }
    }
}
