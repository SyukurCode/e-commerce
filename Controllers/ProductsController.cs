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
using DotNetEnv;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.Options;

namespace E_Commers_Adelia.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<EUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context,
            UserManager<EUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _db = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User); 
            if(userId != null){
                var userProduct = await _db.Products.Where(p => p.userId == userId).Include(o => o.Options).ToListAsync();
                return View(userProduct);
            }
            return View();
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _db.Products.Include(o => o.Options)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,userId,Name,Description,ImageUrl,Price,Stock,CreateDate,UpdateDate")] Product product, IFormFile avatarFile)
        {

            if (ModelState.IsValid)
            {
                if (avatarFile != null && avatarFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath,"Upload","Products", product.userId);

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = Path.Combine("Upload","Products", product.userId ,uniqueFileName);
                }
                else
                {
                    TempData["FailedMessage"] = "Product image is required";
                    return View(product);
                }
                product.isEnable = true;
                product.CreateDate = DateTime.UtcNow;
                product.UpdateDate = DateTime.UtcNow;

                _db.Add(product);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "Product image is required");
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _db.Products.Include(o => o.Options)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,userId,Name,Description,ImageUrl,Price,Stock,CreateDate,UpdateDate,isEnable")] Product product)
        {
            if (id != product.Id)
            {
                TempData["DialogError"] = "Product not found";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                product.CreateDate = product.CreateDate.ToUniversalTime();
                product.UpdateDate = DateTime.UtcNow;
                try
                {
                    _db.Update(product);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        TempData["DialogError"] = "Product not found";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Product successfully update!.";
                return RedirectToAction("Edit", new { id = id });
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["DialogError"] = "Product not found";
                return RedirectToAction("Index");
            }

            var product = await _db.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                TempData["DialogError"] = "Product not found";
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product != null)
            {
                _db.Products.Remove(product);
            }

            // delete picture
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl);
            if (System.IO.File.Exists(uploadsFolder))
            {
                System.IO.File.Delete(uploadsFolder);
            }
            else
            {
                Log.Error("File not found, and fail to delete");
            }
            TempData["WarningMessage"] = "Product deleted!.";
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _db.Products.Any(e => e.Id == id);
        }

        public IActionResult CreateOption(int id)
        {
            ViewData["id"] = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOption([Bind("id,ProductId,OptionName,AdditionalPrice")] ProductOption productOption)
        {
            if (ModelState.IsValid)
            {
                _db.ProductOptions.Add(productOption);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Edit", new { id = productOption.ProductId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> DeleteOption(int id)
        {
            var option = await _db.ProductOptions.FindAsync(id);
            if(option != null)
            {
                _db.Remove(option);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Edit", new { id = option.ProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEnableProduct(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product != null)
            {
                product.isEnable = !product.isEnable;
                _db.Update(product);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
