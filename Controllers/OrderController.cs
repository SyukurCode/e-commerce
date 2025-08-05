using E_Commers_Adelia.Common;
using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;

namespace E_Commers_Adelia.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<EUser> _userManager;
        public OrderController(ApplicationDbContext db, UserManager<EUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int id)
        {
            var product = await _db.Products.Include(o => o.Options).FirstOrDefaultAsync(p => p.Id == id);
            var orderDeatails = new OrderDetails
            {
                Product = product,
            };
            return View(orderDeatails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(OrderDetails orderDetails, string action)
        {
            var orderNo = OrderNumber.Generate();

            if (orderDetails.Quantity > orderDetails.Product.Stock)
            {
                TempData["DialogWarning"] = "Product in low stock, please reduce quantity";
                return RedirectToAction("Index");
            }

            if (action == "BuyNow")
            {

                // kira additional price
                decimal additionalPrice = 0;
                decimal productPrice = 0;

                if (orderDetails.SelectedOptionIds != null && orderDetails.SelectedOptionIds.Count > 0)
                {
                    foreach (var item in orderDetails.SelectedOptionIds)
                    {
                        var option = await _db.ProductOptions.FirstOrDefaultAsync(po => po.Id == item);
                        if (option != null)
                        {
                            additionalPrice += option.AdditionalPrice;
                        }
                    }
                }

                if (orderDetails.Product != null)
                {
                    productPrice = orderDetails.Product.Price;
                }

                // product option
                string selectedOption = String.Empty;
                bool isFirst = false;

                if (orderDetails.SelectedOptionIds != null)
                {
                    foreach (var id in orderDetails.SelectedOptionIds)
                    {
                        var option = await _db.ProductOptions.FindAsync(id);
                        if (orderDetails.SelectedOptionIds.Count() > 1)
                        {
                            if (isFirst) { selectedOption = string.Format("+RM{0} {1},", option.AdditionalPrice, option.OptionName); }
                            else { selectedOption += string.Format("+RM{0} {1}", option.AdditionalPrice, option.OptionName); }
                        }
                        else
                        {
                            selectedOption = string.Format("+RM{0} {1}", option.AdditionalPrice, option.OptionName);
                        }
                    }
                }

                var order = new Models.Order 
                {
                    OrderNo = orderNo,
                    CustomerId = _userManager.GetUserId(User) ?? "Guest",
                    UnitPrice = productPrice + additionalPrice,
                    SubTotalPrice = (productPrice + additionalPrice) * orderDetails.Quantity,
                    PlaceDateTime = DateTime.UtcNow,
                    ProductId = orderDetails.Product.Id,
                    Quantity = orderDetails.Quantity,
                    SelectedOption = selectedOption,
                    StatusId = OrderStatus.ToPay.Id,
                };

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                return RedirectToAction("Index","Checkout", new { orderNo = orderNo});
            } 
            else if(action == "AddToCart")
            {
                    // 🛒 Logik tambah ke cart
                    await AddProductToCart(orderDetails);
                    TempData["SuccessMessage"] = "Item added to cart!";
                    return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<decimal> ChooseChecked(List<int> ids)
        {
            decimal price = 0;
            foreach(var id in ids)
            {
                var option = await _db.ProductOptions.FindAsync(id);
                price = price +  option.AdditionalPrice;
            }
            return price;
        }

        private async Task AddProductToCart(OrderDetails orderDetails)
        {
            List<ProductOption> productOption = new List<ProductOption>();

            if (orderDetails.SelectedOptionIds != null && orderDetails.SelectedOptionIds.Count > 0)
            {
                foreach (var option in orderDetails.SelectedOptionIds)
                {
                    var o = await _db.ProductOptions.FirstAsync(o => o.Id == option);
                    if (o != null)
                    {
                        productOption.Add(o);
                    }
                }
            }

            // Contoh implementasi (gunakan session, DB, etc.)
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // kira additional price
            decimal additionalPrice = 0;
            decimal productPrice = 0;

            if (orderDetails.SelectedOptionIds != null && orderDetails.SelectedOptionIds.Count > 0)
            {
                foreach (var item in orderDetails.SelectedOptionIds)
                {
                    var option = await _db.ProductOptions.FirstOrDefaultAsync(po => po.Id == item);
                    if (option != null)
                    {
                        additionalPrice += option.AdditionalPrice;
                    }
                }
            }

            if (orderDetails.Product != null)
            {
                productPrice = orderDetails.Product.Price;
            }

            cart.Add(new CartItem
            {
                Id = Guid.NewGuid(),
                Product = orderDetails.Product,
                Quantity = orderDetails.Quantity,
                SelectedOptions = productOption,
                UnitPrice = productPrice + additionalPrice,
                TotalPrice = (productPrice + additionalPrice) * orderDetails.Quantity

            });

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            
        }

    }
}
