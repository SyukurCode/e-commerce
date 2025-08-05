using E_Commers_Adelia.Common;
using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace E_Commers_Adelia.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<EUser> _userManager;
        public CartController(ApplicationDbContext db, UserManager<EUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            decimal totalPrice = 0;
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            foreach (var item in cart)
            {
                totalPrice += item.UnitPrice * item.Quantity;
            }
            ViewData["TotalToPay"] = totalPrice;

            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveCartItem(Guid id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var itemToRemove = cart.FirstOrDefault(x => x.Id == id);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public decimal UpdateCartItem(Guid id, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var itemToUpdate = cart.FirstOrDefault(x => x.Id == id);
            if(itemToUpdate != null)
            {
                itemToUpdate.Quantity = quantity;
                itemToUpdate.TotalPrice = itemToUpdate.UnitPrice * quantity;
            }
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            decimal totalToPay = 0;

            cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            foreach (var item in cart)
            {
                totalToPay += item.TotalPrice;
            }

            return totalToPay;
        }
        public async Task<IActionResult> checkoutCartItem() 
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var orderNo = OrderNumber.Generate();
            var orders = new List<Order>();

            foreach (var item in cart)
            {
                string selectedOption = "";

                if (item.SelectedOptions != null && item.SelectedOptions.Any())
                {
                    selectedOption = string.Join(", ", item.SelectedOptions.Select(opt =>
                        $"+RM{opt.AdditionalPrice} {opt.OptionName}"
                    ));
                }

                var order = new Order
                {
                    OrderNo = orderNo,
                    CustomerId = _userManager.GetUserId(User) ?? "Guest",
                    UnitPrice = item.UnitPrice,
                    SubTotalPrice = item.TotalPrice,
                    PlaceDateTime = DateTime.UtcNow,
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity,
                    SelectedOption = selectedOption,
                    StatusId = OrderStatus.ToPay.Id,
                };

                orders.Add(order);
            }

            // Simpan semua order ke database
            _db.Orders.AddRange(orders);

            // Kosongkan cart selepas selesai
            HttpContext.Session.SetObjectAsJson("Cart", new List<CartItem>());

            await _db.SaveChangesAsync();
            return RedirectToAction("index", "Checkout", new { orderNo = orderNo });
        }
    }
}
