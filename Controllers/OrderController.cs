using AspNetCoreGeneratedDocument;
using E_Commers_Adelia.Common;
using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using E_Commers_Adelia.Repository;
using E_Commers_Adelia.Service;
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
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<EUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INotification _noti;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IReceiptVerificationService _receiptVerificationService;
        private readonly IOrderHistory _orderHistory;
        public OrderController(ApplicationDbContext db,
            UserManager<EUser> userManager, 
            IWebHostEnvironment webHostEnvironment, 
            INotification noti, 
            IHubContext<NotificationHub> hub,
            IReceiptVerificationService receiptVerificationService,
            IOrderHistory orderHistory)
        {
            _db = db;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _noti = noti;
            _hub = hub;
            _receiptVerificationService = receiptVerificationService;
            _orderHistory = orderHistory;
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
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index), new { id = orderDetails.Product?.Id });
            }

            var orderNo = OrderNumber.Generate();

            if (orderDetails.Quantity > orderDetails.Product?.Stock)
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

                //check seller still open
                var isOpen = _userManager.FindByIdAsync(orderDetails.Product?.userId ?? "").Result?.IsOpen ?? false;
                if (!isOpen) 
                {
                    TempData["DialogError"] = "Store was close";
                    return RedirectToAction("Index","Home");
                }

                var order = new Models.Order
                {
                    OrderNo = orderNo,
                    SellerId = orderDetails.Product?.userId ?? "",
                    CustomerId = (User?.Identity != null && User.Identity.IsAuthenticated) ? _userManager.GetUserId(User) : null,
                    UnitPrice = productPrice + additionalPrice,
                    SubTotalPrice = (productPrice + additionalPrice) * orderDetails.Quantity,
                    PlaceDateTime = DateTime.UtcNow,
                    ProductId = orderDetails.Product?.Id ?? 0,
                    ProductImageUrl = orderDetails.Product?.ImageUrl ?? "/img/blank.jpg",
                    ProductName = orderDetails.Product?.Name ?? "",
                    ProductDescription = orderDetails.Product?.Description ?? "",
                    Quantity = orderDetails.Quantity,
                    SelectedOption = selectedOption,
                    StatusId = OrderStatus.ToPay.Id,
                    DeliveryId = 0,
                    ExtraCharges = 0
                };

                // validate product 
                var product = await _db.Products.FindAsync(order.ProductId);
                if (product != null || product.isEnable || !product.isHide)
                {
                    if(product.Stock >= order.Quantity)
                    {
                        product.Stock = product.Stock - order.Quantity;
                        _db.Products.Update(product);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        if(product.Stock > 0)
                        {
                            TempData["DialogWarning"] = "Product in low stock, please reduce quantity";
                            return RedirectToAction("Index");
                        }
                        TempData["DialogWarning"] = "Sorry, Product is soldout";
                        return RedirectToAction("Index");
                    }
                }
                else {
                    TempData["DialogWarning"] = $"This product currently not available";
                    return RedirectToAction(nameof(Index), new { id = orderDetails.Product.Id });
                }
                
                await _db.Orders.AddAsync(order);
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
            var orders = await _db.Orders.Where(x => x.OrderNo == orderNo && x.StatusId != OrderStatus.UserCanceled.Id).ToListAsync();
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
            var orders = await _db.Orders.Where(x => x.OrderNo == model.OrderNo && x.StatusId != OrderStatus.UserCanceled.Id).ToListAsync();
            var Extrcharge = _db.SellerDeliveryOptions.FirstOrDefault(x => x.DeliveryId == model.OrderPlace.DeliveryTypeId)?.AdditionalPrice;
            foreach (var order in orders)
            {
                order.DeliveryId = model.OrderPlace.DeliveryTypeId;
                order.ExtraCharges = Extrcharge ?? 0;
                _db.Orders.UpdateRange(order);
            }
            await _db.SaveChangesAsync();

            HttpContext.Session.SetString("OrderView", JsonConvert.SerializeObject(model));
            return RedirectToAction("Placed", new { OrderNo = model.OrderNo });
        }

        public async Task<IActionResult> Placed(string OrderNo)
        {
            // for selfpickup get self pickup address 
            var customer = User.Identity.IsAuthenticated ? await _userManager.GetUserAsync(User): null;
            var customerDeliveryInfo = customer != null ? await _db.CustomerDeliveryInfo.FirstOrDefaultAsync(c => c.CustomerId == customer.Id): null
    ;
            if (customerDeliveryInfo == null || customer == null)
            {
                customerDeliveryInfo = new CustomerDeliveryInfo();
            }

            if (customer != null)
            {
                // Sentiasa update jika `cutomer.Address` ada
                if (!string.IsNullOrWhiteSpace(customer.Address))
                {
                    customerDeliveryInfo.Address = customer.Address;
                }

                // Hanya update jika tiada phone dan `cutomer.PhoneNumber` ada
                if (string.IsNullOrWhiteSpace(customerDeliveryInfo.CustomerPhone) &&
                    !string.IsNullOrWhiteSpace(customer.PhoneNumber))
                {
                    customerDeliveryInfo.CustomerPhone = customer.PhoneNumber;
                }

                // Nama
                if (string.IsNullOrWhiteSpace(customerDeliveryInfo.CustomerName) &&
                    !string.IsNullOrWhiteSpace(customer.DisplayName))
                {
                    customerDeliveryInfo.CustomerName = customer.DisplayName;
                }

                // Email
                if (string.IsNullOrWhiteSpace(customerDeliveryInfo.CustomerEmail) &&
                    !string.IsNullOrWhiteSpace(customer.Email))
                {
                    customerDeliveryInfo.CustomerEmail = customer.Email;
                }
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
                    await _db.CustomerPayments.AddAsync(customerPayment);
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
            //if (model.DeliveryTypeId == DeliveryOption.SelfPickup.Id)
            //{
            //    ModelState.Remove("CustomerDeliveryInfo.Address");
            //    ModelState.Remove("CustomerDeliveryInfo.CustomerName");
            //    ModelState.Remove("CustomerDeliveryInfo.CustomerEmail");
            //    ModelState.Remove("CustomerDeliveryInfo.CustomerPhone");
            //    ModelState.Remove("CustomerDeliveryInfo.OrderNo");
            //}

            // Validate Model
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Placed", new { OrderNo =  model.OrderNo});
            }

            // Jika Delivery adalah Standard Delivery tambah maklumat customer
            if (model.DeliveryTypeId == DeliveryOption.StandardDelivery.Id || model.DeliveryTypeId == DeliveryOption.SelfPickup.Id)
            {
                // Jika maklumat dilivery tiada trus tambah baru
                var customerDeliveryInfo = await _db.CustomerDeliveryInfo.FirstOrDefaultAsync(c => c.CustomerId == model.CustomerDeliveryInfo.CustomerId);
                if (customerDeliveryInfo == null || string.IsNullOrEmpty(model.CustomerDeliveryInfo.CustomerId))
                {
                    customerDeliveryInfo = new CustomerDeliveryInfo();
                    customerDeliveryInfo = model.CustomerDeliveryInfo;
                    await _db.CustomerDeliveryInfo.AddAsync(customerDeliveryInfo);
                    await _db.SaveChangesAsync();
                }
            }

            // Get Payment Detail
            var payment = await _db.CustomerPayments.FirstOrDefaultAsync(p => p.OrderNo == model.OrderNo);
            if (payment.PaymentTypeId == PaymentMethod.QR.Id || payment.PaymentTypeId == PaymentMethod.OnlineTransfer.Id)
            {
                return RedirectToAction("UploadReceipt", new { id = payment.Id });
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
            var order = await _db.Orders.Where(x => x.OrderNo == orderNo && x.StatusId != OrderStatus.UserCanceled.Id).ToListAsync();
            var SellerId = "";
            var totalPrice = order.Select(x=>x.SubTotalPrice).Sum();
            if (order != null)
            {
                foreach (var item in order)
                {
                    SellerId = item.SellerId;
                    item.StatusId = OrderStatus.OrderSend.Id;
                    item.UpdateDateTime = DateTime.UtcNow;
                    _db.Orders.Update(item);
                    await _orderHistory.CreateAsync(orderNo: item.OrderNo, text: "Your order was send to seller",item.StatusId);
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
                
                // Tolak quantity Product
                //var product = await _db.Products.FindAsync()

                await _hub.Clients.Users(SellerId).SendAsync("Order-Receive", orderNo, totalPrice);
            }
            
            return View(payment);
        }


        public async Task<IActionResult> UploadReceipt(int id)
        {
            var payment = await _db.CustomerPayments.FindAsync(id);
            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadReceipt(CustomerPayment model, IFormFile image) 
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model.ResitUrl = await UploadFileHelper.Upload(image, "PaymentReceipt",_webHostEnvironment, "800KB");
                var result = await _receiptVerificationService.VerifyReceiptAsync(model.ResitUrl,model.Amount, DateTime.UtcNow);
                if(!result.Status)
                {
                    TempData["DialogWarning"] = $"{result.Message}";
                    return View(model);
                }

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
                var order = await _db.Orders.Where(x => x.OrderNo == orderNo && x.StatusId != OrderStatus.UserCanceled.Id).ToListAsync();
                return View(order);
            }
            
            var userOrder = await _db.Orders.Where(x => x.CustomerId == _userManager.GetUserId(User)).ToListAsync();
            return View(userOrder);
        }

        public async Task<IActionResult> Cancel(string orderNo)
        {
            var orders = await _db.Orders.Where(x => x.OrderNo == orderNo).ToListAsync();
            foreach(var order in orders)
            {
                var product = await _db.Products.FindAsync(order.ProductId);
                if (product != null)
                {
                    product.Stock = product.Stock + order.Quantity;
                    order.StatusId = OrderStatus.Cancelled.Id;
                    _db.Orders.Update(order);
                    _db.Products.Update(product);
                    await _db.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> CancelSingle(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order != null)
            {
                var product = await _db.Products.FindAsync(order.ProductId);
                if (product != null)
                {
                    product.Stock = product.Stock + order.Quantity;
                    order.StatusId = OrderStatus.UserCanceled.Id;
                    _db.Orders.Update(order);
                    _db.Products.Update(product);
                    await _db.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index", "CustomerOrder");
        }
    }
}
