using AspNetCoreGeneratedDocument;
using E_Commers_Adelia.Common;
using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using E_Commers_Adelia.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Transactions;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;

namespace E_Commers_Adelia.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<EUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INotification _noti;
        private readonly IHubContext<NotificationHub> _hub;
        public OrderController(ApplicationDbContext db, UserManager<EUser> userManager, IWebHostEnvironment webHostEnvironment, INotification noti, IHubContext<NotificationHub> hub)
        {
            _db = db;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _noti = noti;
            _hub = hub;
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

            if (action == "Buy Now")
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
                    for (int i = 0; i < orderDetails.SelectedOptionIds.Count(); i++)
                    {

                        var id = orderDetails.SelectedOptionIds[i];
                        var option = await _db.ProductOptions.FindAsync(id);
                        //var line = string.Format("+RM{0} {1}", option.AdditionalPrice, option.OptionName);
                        var line = string.Format("- {0}", option.OptionName);

                        if (i < orderDetails.SelectedOptionIds.Count() - 1)
                            selectedOption += line + "\r\n"; // tambah newline kalau bukan last
                        else
                            selectedOption += line; // last item takde newline
                    }
                }

                var order = new Models.Order
                {
                    OrderNo = orderNo,
                    SellerId = orderDetails.Product.userId,
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

                return RedirectToAction("Checkout", new { orderNo = orderNo });
            }
            else if (action == "Add Cart")
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
            foreach (var id in ids)
            {
                var option = await _db.ProductOptions.FindAsync(id);
                price = price + option.AdditionalPrice;
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

        public async Task<IActionResult> UserOrder(string orderNo)
        {
            var model = await _db.Orders.Where(o => o.OrderNo == orderNo).ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> Checkout(string orderNo)
        {
            decimal totalPrice = 0;
            var orders = await _db.Orders.Where(o => o.OrderNo == orderNo).ToListAsync();
            var sellerId = string.Empty;
            foreach (var order in orders)
            {
                totalPrice += order.SubTotalPrice;
                sellerId = order.SellerId;
            }
            var oderView = new OrderView
            {
                OrderNo = orderNo,
                OrderItem = orders,
                SellerId = sellerId,
                TotalToPay = totalPrice,
            };

            return View(oderView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout([Bind("OrderNo,SellerId,OrderItem,OrderPlace,OrderPlace.PaymentTypeId,OrderPlace.DeliveryTypeId,TotalToPay")] OrderView model)
        {
            ModelState.Remove("OrderItem");

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            HttpContext.Session.SetString("OrderView", JsonConvert.SerializeObject(model));
            return RedirectToAction("Placed", new { OrderNo = model.OrderNo });
        }

        public async Task<IActionResult> Placed(string OrderNo)
        {
            // for selfpickup get self pickup address
            var cutomer = await _userManager.GetUserAsync(User);
            var customerDeliveryInfo = await _db.CustomerDeliveryInfo.FirstOrDefaultAsync(c => c.CustomerId == cutomer.Id);
            if (customerDeliveryInfo == null)
            {
                customerDeliveryInfo = new CustomerDeliveryInfo();
            }

            // Sentiasa update jika `cutomer.Address` ada
            if (!string.IsNullOrWhiteSpace(cutomer.Address))
            {
                customerDeliveryInfo.Address = cutomer.Address;
            }

            // Hanya update jika tiada phone dan `cutomer.PhoneNumber` ada
            if (string.IsNullOrWhiteSpace(customerDeliveryInfo.CustomerPhone) &&
                !string.IsNullOrWhiteSpace(cutomer.PhoneNumber))
            {
                customerDeliveryInfo.CustomerPhone = cutomer.PhoneNumber;
            }

            // Nama
            if (string.IsNullOrWhiteSpace(customerDeliveryInfo.CustomerName) &&
                !string.IsNullOrWhiteSpace(cutomer.DisplayName))
            {
                customerDeliveryInfo.CustomerName = cutomer.DisplayName;
            }

            // Email
            if (string.IsNullOrWhiteSpace(customerDeliveryInfo.CustomerEmail) &&
                !string.IsNullOrWhiteSpace(cutomer.Email))
            {
                customerDeliveryInfo.CustomerEmail = cutomer.Email;
            }

            var json = HttpContext.Session.GetString("OrderView");
            if (json != null)
            {
                var model = JsonConvert.DeserializeObject<OrderView>(json);

                var customerPayment = await _db.CustomerPayments.FirstOrDefaultAsync(x => x.OrderNo == model.OrderNo);
                if (customerPayment == null)
                {
                    customerPayment = new CustomerPayment();
                }
                customerPayment.OrderNo = model.OrderNo;
                customerPayment.Amount = model.TotalToPay;
                customerPayment.PaymentTypeId = model.OrderPlace.PaymentTypeId;
                if (customerPayment == null)
                {
                    _db.CustomerPayments.Add(customerPayment);
                }
                else
                {
                    _db.CustomerPayments.Update(customerPayment);
                }
                await _db.SaveChangesAsync();

                var viewModel = new OrderPlaceView
                {
                    Amount = model.TotalToPay,
                    SellerId = model.SellerId,
                    OrderNo = customerPayment.OrderNo,
                    PaymentTypeId = model.OrderPlace.PaymentTypeId,
                    DeliveryTypeId = model.OrderPlace.DeliveryTypeId,
                    CustomerDeliveryInfo = customerDeliveryInfo,
                };
                // Remove Session
                HttpContext.Session.Remove("OrderView");
                return View(viewModel);
            }
            return RedirectToAction("CheckOut", new { OrderNo = OrderNo });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Placed(OrderPlaceView model)
        {
            // remove model state jika selfpickup
            if (model.DeliveryTypeId == DeliveryOption.SelfPickup.Id)
            {
                ModelState.Remove("CustomerDeliveryInfo.Address");
                ModelState.Remove("CustomerDeliveryInfo.CustomerName");
                ModelState.Remove("CustomerDeliveryInfo.CustomerEmail");
                ModelState.Remove("CustomerDeliveryInfo.CustomerPhone");

            }

            // Validate Model
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Jika Delivery adalah Standard Delivery tambah maklumat customer
            if (model.DeliveryTypeId == DeliveryOption.StandardDelivery.Id)
            {
                // Jika maklumat dilivery tiada trus tambah baru
                var customerDeliveryInfo = await _db.CustomerDeliveryInfo.FirstOrDefaultAsync(c => c.CustomerId == model.CustomerDeliveryInfo.CustomerId);
                if (customerDeliveryInfo == null)
                {
                    customerDeliveryInfo = new CustomerDeliveryInfo();
                    customerDeliveryInfo = model.CustomerDeliveryInfo;
                    _db.CustomerDeliveryInfo.Add(customerDeliveryInfo);
                    await _db.SaveChangesAsync();
                }
            }

            // Get Payment Detail
            var payment = await _db.CustomerPayments.FirstOrDefaultAsync(p => p.OrderNo == model.OrderNo);
            if (payment.PaymentTypeId == PaymentMethod.QR.Id || payment.PaymentTypeId == PaymentMethod.OnlineTransfer.Id)
            {
                return RedirectToAction("UploadRecept", new { id = payment.Id });
            }
            return RedirectToAction("PaidComplete", new { orderNo = model.OrderNo });

        }

        public async Task<IActionResult> PaidComplete(string orderNo)
        {
            var payment = await _db.CustomerPayments.FirstOrDefaultAsync(p => p.OrderNo == orderNo);
            // Update Paymet Detail
            payment.PaymentDate = DateTime.UtcNow;
            _db.CustomerPayments.Update(payment);
            await _db.SaveChangesAsync();

            // Update Order Status
            var order = await _db.Orders.Where(x => x.OrderNo == orderNo).ToListAsync();
            var SellerId = "";
            var totalPrice = order.Select(x=>x.SubTotalPrice).Sum();
            if (order != null)
            {
                foreach (var item in order)
                {
                    SellerId = item.SellerId;
                    item.StatusId = OrderStatus.OrderSend.Id;
                    item.UpdateDateTime = DateTime.UtcNow;
                    _db.Orders.UpdateRange(item);
                }
                await _db.SaveChangesAsync();

                // Send notification to seller
                await _noti.Create(new Notification { 
                    ActionURL = $"/ProcessOrder/View/{orderNo}",
                    DateCreated = DateTime.UtcNow,
                    FaIcon = "fa-shopping-bag",
                    Text = "New order receive",
                    UserId = SellerId 
                });

                await _hub.Clients.Users(SellerId).SendAsync("Order-Receive", orderNo, totalPrice);
            }
            
            return View(payment);
        }

        public async Task<IActionResult> UploadRecept(int id)
        {
            var payment = await _db.CustomerPayments.FindAsync(id);
            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadRecept(CustomerPayment model, IFormFile image) 
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                model.ResitUrl = await UploadFileHelper.Upload(image, "PaymentReceipt",_webHostEnvironment, "800KB");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            _db.CustomerPayments.Update(model);
            await _db.SaveChangesAsync();

            return RedirectToAction("PaidComplete", new { orderNo = model.OrderNo });
        }

        public async Task<IActionResult> ViewOrder(string orderNo)
        {

            if (orderNo != "")
            {
                ViewData["OrderNo"] = orderNo;
                var order = await _db.Orders.Where(x => x.OrderNo == orderNo).ToListAsync();
                return View(order);
            }
            
            var userOrder = await _db.Orders.Where(x => x.CustomerId == _userManager.GetUserId(User)).ToListAsync();
            return View(userOrder);
        }
    }
}
