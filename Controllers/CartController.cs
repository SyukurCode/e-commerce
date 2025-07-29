using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace E_Commers_Adelia.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            decimal totalPrice = 0;
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            foreach (var item in cart)
            {
                totalPrice += item.UnitPrice;
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
        public IActionResult UpdateCartItem(Guid id, int quantity, string count)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var itemToUpdate = cart.FirstOrDefault(x => x.Id == id);
            if(itemToUpdate != null)
            {
                itemToUpdate.Quantity = quantity;
                itemToUpdate.TotalPrice = itemToUpdate.UnitPrice * quantity;
            }
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Index");
        }
        public IActionResult checkoutCartItem() 
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            return View();
        }
    }
}
