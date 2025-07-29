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

namespace E_Commers_Adelia.Controllers
{
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
            return View(await _db.SellerDeliveryOptions.Where(d => d.userId == userId).ToListAsync());
        }

        // GET: SellerDeliveryOptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sellerDeliveryOption = await _db.SellerDeliveryOptions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sellerDeliveryOption == null)
            {
                return NotFound();
            }

            return View(sellerDeliveryOption);
        }

        // GET: SellerDeliveryOptions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SellerDeliveryOptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DeliveryId,userId,isDisable")] SellerDeliveryOption sellerDeliveryOption)
        {
            if (ModelState.IsValid)
            {
                _db.Add(sellerDeliveryOption);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sellerDeliveryOption);
        }

        // GET: SellerDeliveryOptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sellerDeliveryOption = await _db.SellerDeliveryOptions.FindAsync(id);
            if (sellerDeliveryOption == null)
            {
                return NotFound();
            }
            return View(sellerDeliveryOption);
        }

        // POST: SellerDeliveryOptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DeliveryId,userId,isDisable")] SellerDeliveryOption sellerDeliveryOption)
        {
            if (id != sellerDeliveryOption.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(sellerDeliveryOption);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SellerDeliveryOptionExists(sellerDeliveryOption.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sellerDeliveryOption);
        }
        private bool SellerDeliveryOptionExists(int id)
        {
            return _db.SellerDeliveryOptions.Any(e => e.Id == id);
        }
    }
}
