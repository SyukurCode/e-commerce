using E_Commers_Adelia.Common;
using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace E_Commers_Adelia.Controllers
{
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
            List<string> sellerId = [];
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            foreach (var item in cart)
            {
                if (!sellerId.Contains(item.Product.userId))
                {
                    sellerId.Add(item.Product.userId);
                }
               
            }
            // kire ikut seller
            foreach (var id in sellerId)
            {
                decimal totalPrice = 0;
                var sellerCart = cart.Where(c => c.Product.userId == id);
                foreach (var item in sellerCart)
                {
                    totalPrice += item.UnitPrice * item.Quantity;
                }
                ViewData[id] = totalPrice;
            }

           
            var cartview = new CartView
            {
                ItemCart = cart,
                Seller = sellerId
            };

            return View(cartview);
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
        public async Task<IActionResult> checkoutCartItem(string id) 
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var orderNo = OrderNumber.Generate();
            var orders = new List<Order>();

            foreach (var item in cart)
            {
                if (item.Product.userId == id)
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
                        SellerId = item.Product.userId,
                        CustomerId = _userManager.GetUserId(User) ?? "Guest",
                        UnitPrice = item.UnitPrice,
                        SubTotalPrice = item.TotalPrice,
                        PlaceDateTime = DateTime.UtcNow,
                        ProductImageUrl = item.Product.ImageUrl ?? "/img/blank.jpg",
                        ProductName = item.Product.Name,
                        Quantity = item.Quantity,
                        SelectedOption = selectedOption,
                        StatusId = OrderStatus.ToPay.Id,
                        DeliveryId = 0,
                        ExtraCharges = 0,
                    };

                    orders.Add(order);
                }
            }

            // Simpan semua order ke database
            _db.Orders.AddRange(orders);

            // Kosongkan cart selepas selesai
            var itemToRemove = cart.Where(x => x.Product.userId == id).ToList();
            if (itemToRemove != null)
            {
                foreach (var item in itemToRemove)
                {
                    cart.Remove(item);
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                }
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Checkout", "Order", new { orderNo = orderNo });
        }
    }
}
