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
                        isEnable = true
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
            switch (id) 
            {
                
                case 1:   //qr
                    return View(sellerPaymentMethod);
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
        public async Task<IActionResult> EditQR(
        int id,
        [Bind("Id,PaymentMethodId,UserId,isEnable")] SellerPaymentMethod sellerPaymentMethod,
        IFormFile qrFile)
        {
            var qrExists = await _qr.isQRExist(sellerPaymentMethod.UserId);
            if (id != sellerPaymentMethod.Id)
            {
                ModelState.AddModelError(string.Empty, "Payment option ID mismatch.");
                return View(sellerPaymentMethod);
            }
            // --- Step 1: If user enables QR payment, QR image must be uploaded or exist ---
            if (sellerPaymentMethod.isEnable && sellerPaymentMethod.PaymentMethodId == PaymentMethod.QR.Id)
            {
                
                bool hasNewQrFile = qrFile != null && qrFile.Length > 0;

                if (!hasNewQrFile && !qrExists)
                {
                    ModelState.AddModelError(string.Empty, "Please upload a QR Code image to enable QR payment.");
                    return View(sellerPaymentMethod);
                }

                if (hasNewQrFile)
                {
                    var uploadSuccess = await _qr.UploadQrCodeAsync(qrFile, sellerPaymentMethod.UserId);
                    if (!uploadSuccess)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to upload QR code. Please try again.");
                        return View(sellerPaymentMethod);
                    }
                }
            }
            // Jika disable dan qr dipipilih
            if (!sellerPaymentMethod.isEnable && sellerPaymentMethod.Id == PaymentMethod.QR.Id)
            {
                ModelState.Remove("qrFile");
            }

            // jika enable dan qr dh ada
            if(sellerPaymentMethod.isEnable && sellerPaymentMethod.Id == PaymentMethod.QR.Id && qrExists)
            {
                ModelState.Remove("qrFile");
            }

            // --- Step 2: Save to database if everything is valid ---
            if (!ModelState.IsValid)
            {
                return View(sellerPaymentMethod);
            }

            try
            {
                _db.Update(sellerPaymentMethod);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Payment option successfully update!.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SellerPaymentMethodExists(sellerPaymentMethod.Id))
                {
                    ModelState.AddModelError(string.Empty, "Payment option not exist.");
                    return View(sellerPaymentMethod);
                }

                throw; // Let it bubble up if it's a real issue
            }
        }
        // GET: SellerPaymentMethods/EditCOD
        public async Task<IActionResult> EditCOD(int id)
        {
            var sellerPaymentMethod = await _db.SellerPaymentMethods.FindAsync(id);
            if (sellerPaymentMethod == null)
            {
                ModelState.AddModelError(string.Empty, "Payment option not found");
            }
            var dbCODNote = await _db.CodNotes.FirstOrDefaultAsync(c => c.UserId == sellerPaymentMethod.UserId);
            if (dbCODNote != null) {
                ViewData["CodNote"] = dbCODNote.Note;
            }
            return View(sellerPaymentMethod);
        }

        // POST: SellerPaymentMethods/EditCOD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCOD(int id, [Bind("Id,PaymentMethodId,UserId,isEnable")] SellerPaymentMethod sellerPaymentMethod, string codNote)
        {
            if (!sellerPaymentMethod.isEnable)
            {
                ModelState.Remove("codNote");
            }

            if (ModelState.IsValid)
            {
                if (sellerPaymentMethod.isEnable)
                {
                    var dbCODNote = await _db.CodNotes.FirstOrDefaultAsync(c => c.UserId == sellerPaymentMethod.UserId);
                    if (dbCODNote == null)
                    {
                        CodNote _codNote = new CodNote
                        {
                            UserId = sellerPaymentMethod.UserId,
                            Note = codNote
                        };
                        _db.CodNotes.Add(_codNote);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        if (dbCODNote.Note != codNote)
                        {
                            dbCODNote.Note = codNote;
                            _db.CodNotes.Update(dbCODNote);
                            await _db.SaveChangesAsync();
                        }
                    }
                    
                }
                try
                {
                    _db.Update(sellerPaymentMethod);
                    await _db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Payment option successfully update!.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SellerPaymentMethodExists(sellerPaymentMethod.Id))
                    {
                        ModelState.AddModelError(string.Empty, "Payment option not exist.");
                        return View(sellerPaymentMethod);
                    }

                    throw; // Let it bubble up if it's a real issue
                }
            }
            if((string.IsNullOrEmpty(codNote) || string.IsNullOrWhiteSpace(codNote)) && sellerPaymentMethod.isEnable)
            {
                ModelState.AddModelError(string.Empty, "Message to customer is required to enable COD payment.");
            }

            return View(sellerPaymentMethod);
        }
        // GET: SellerPaymentMethods/EditCash
        public async Task<IActionResult> EditCash(int id)
        {
            var sellerPaymentMethod = await _db.SellerPaymentMethods.FindAsync(id);
            if (sellerPaymentMethod == null)
            {
                ModelState.AddModelError(string.Empty, "Payment option not found");
            }
            var dbCashNote = await _db.CashNotes.FirstOrDefaultAsync(c => c.UserId == sellerPaymentMethod.UserId);
            if (dbCashNote != null)
            {
                ViewData["CashNote"] = dbCashNote.Note;
            }
            return View(sellerPaymentMethod);
        }
        // POST: SellerPaymentMethods/EditCash
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCash(int id, [Bind("Id,PaymentMethodId,UserId,isEnable")] SellerPaymentMethod sellerPaymentMethod, string cashNote)
        {
            if (!sellerPaymentMethod.isEnable)
            {
                ModelState.Remove("cashNote");
            }

            if (ModelState.IsValid)
            {
                if (sellerPaymentMethod.isEnable)
                {
                    var dbCashNote = await _db.CashNotes.FirstOrDefaultAsync(c => c.UserId == sellerPaymentMethod.UserId);
                    if (dbCashNote == null)
                    {
                        CashNote _cashNote = new CashNote
                        {
                            UserId = sellerPaymentMethod.UserId,
                            Note = cashNote
                        };
                        _db.CashNotes.Add(_cashNote);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        if (dbCashNote.Note != cashNote)
                        {
                            dbCashNote.Note = cashNote;
                            _db.CashNotes.Update(dbCashNote);
                            await _db.SaveChangesAsync();
                        }
                    }

                }
                try
                {
                    _db.Update(sellerPaymentMethod);
                    await _db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Payment option successfully update!.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SellerPaymentMethodExists(sellerPaymentMethod.Id))
                    {
                        ModelState.AddModelError(string.Empty, "Payment option not exist.");
                        return View(sellerPaymentMethod);
                    }

                    throw; // Let it bubble up if it's a real issue
                }
            }
            if ((string.IsNullOrEmpty(cashNote) || string.IsNullOrWhiteSpace(cashNote)) && sellerPaymentMethod.isEnable)
            {
                ModelState.AddModelError(string.Empty, "Message to customer is required to enable cash payment.");
            }

            return View(sellerPaymentMethod);
        }

        // GET: SellerPaymentMethods/EditCash
        public async Task<IActionResult> EditOnlineTransfer(int id)
        {
            var sellerPaymentMethod = await _db.SellerPaymentMethods.FindAsync(id);
            if (sellerPaymentMethod == null)
            {
                ModelState.AddModelError(string.Empty, "Payment option not found");
            }
            var dbOnlineTransferNote = await _db.onlineTransferNotes.FirstOrDefaultAsync(c => c.UserId == sellerPaymentMethod.UserId);
            if (dbOnlineTransferNote != null)
            {
                ViewBag.OnlineTransfer = dbOnlineTransferNote;
            }
            return View(sellerPaymentMethod);
        }
        // POST: SellerPaymentMethods/EditCash
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOnlineTransfer(int id, [Bind("Id,PaymentMethodId,UserId,isEnable")] SellerPaymentMethod sellerPaymentMethod, OnlineTransferNote onlineTransferNote)
        {
            if (!sellerPaymentMethod.isEnable)
            {
                ModelState.Remove("onlineTransferNote");
            }

            if (ModelState.IsValid)
            {
                if (sellerPaymentMethod.isEnable)
                {
                    var dbOnlineTransferNote = await _db.onlineTransferNotes.FirstOrDefaultAsync(c => c.UserId == sellerPaymentMethod.UserId);
                    if (dbOnlineTransferNote == null)
                    {
                        
                        _db.onlineTransferNotes.Add(onlineTransferNote);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        if (dbOnlineTransferNote.AccountNumber != onlineTransferNote.AccountNumber)
                        {
                            dbOnlineTransferNote.AccountNumber = onlineTransferNote.AccountNumber;
                        }
                        if (dbOnlineTransferNote.AccounOwnerName != onlineTransferNote.AccounOwnerName)
                        {
                            dbOnlineTransferNote.AccounOwnerName = onlineTransferNote.AccounOwnerName;
                        }
                        if (dbOnlineTransferNote.BankName != onlineTransferNote.BankName)
                        {
                            dbOnlineTransferNote.BankName = onlineTransferNote.BankName;
                        }

                        _db.onlineTransferNotes.Update(dbOnlineTransferNote);
                        await _db.SaveChangesAsync();
                    }

                }
                try
                {
                    _db.Update(sellerPaymentMethod);
                    await _db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Payment option successfully update!.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SellerPaymentMethodExists(sellerPaymentMethod.Id))
                    {
                        ModelState.AddModelError(string.Empty, "Payment option not exist.");
                        return View(sellerPaymentMethod);
                    }

                    throw; // Let it bubble up if it's a real issue
                }
            }


            return View(sellerPaymentMethod);
        }

        private bool SellerPaymentMethodExists(int id)
        {
            return _db.SellerPaymentMethods.Any(e => e.Id == id);
        }
    }
}
