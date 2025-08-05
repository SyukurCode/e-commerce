using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace E_Commers_Adelia.Common
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Detail { get; set; }


        public static readonly PaymentMethod QR = new PaymentMethod
        {
            Id = 1,
            Name = "QR Code",
            Detail = "Bayar dengan scan QR, Anda perlu upload resit bayaran",
        };

        public static readonly PaymentMethod COD = new PaymentMethod
        {
            Id = 2,
            Name = "Cash on Delivery",
            Detail = "Bayar masa terima barang",
        };

        public static readonly PaymentMethod Cash = new PaymentMethod
        {
            Id = 3,
            Name = "Cash",
            Detail = "Bayaran secara tunai",
        };
        public static readonly PaymentMethod OnlineTransfer = new PaymentMethod
        {
            Id = 4,
            Name = "Online Transfer",
            Detail = "Bayaran secara transfer ke account bank"

        };

        public static IEnumerable<PaymentMethod> All =>
            new[] { QR, COD, Cash, OnlineTransfer };
    }
    public class DeliveryOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Detail { get; set; }


        public static readonly DeliveryOption StandardDelivery = new DeliveryOption
        {
            Id = 1,
            Name = "Standard Delivery",
            Detail = "Akan dihantar oleh penjual",

        };

        public static readonly DeliveryOption SelfPickup = new DeliveryOption
        {
            Id = 2,
            Name = "Self Pickup",
            Detail = "Ambil sendiri dari penjual",
        };

        public static IEnumerable<DeliveryOption> All =>
            new[] { StandardDelivery, SelfPickup };
    }

    public class OrderStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Detail { get; set; }

        public static readonly OrderStatus ToPay = new OrderStatus
        {
            Id = 1,
            Name = "To pay",
            Detail = "This order is pending for payment"
        };

        public static readonly OrderStatus OrderSend = new OrderStatus
        {
            Id = 2,
            Name = "Order sent",
            Detail = "Order was placed and sent to seller"
        };

        public static readonly OrderStatus PickupBySeller = new OrderStatus
        {
            Id = 3,
            Name = "Order pickup",
            Detail = "Order picked up by seller"
        };

        public static readonly OrderStatus Processing = new OrderStatus
        {
            Id = 4,
            Name = "Processing",
            Detail = "Seller is preparing the order"
        };

        public static readonly OrderStatus Shipped = new OrderStatus
        {
            Id = 5,
            Name = "Shipped",
            Detail = "Order has been shipped to customer"
        };

        public static readonly OrderStatus Delivered = new OrderStatus
        {
            Id = 6,
            Name = "Delivered",
            Detail = "Customer has received the order"
        };

        public static readonly OrderStatus Completed = new OrderStatus
        {
            Id = 7,
            Name = "Completed",
            Detail = "Order has been completed successfully"
        };

        public static readonly OrderStatus Cancelled = new OrderStatus
        {
            Id = 8,
            Name = "Cancelled",
            Detail = "Order has been cancelled by buyer or seller"
        };

        public static readonly OrderStatus Refunded = new OrderStatus
        {
            Id = 9,
            Name = "Refunded",
            Detail = "Payment was refunded to the customer"
        };
        public static readonly OrderStatus FailedPayment = new OrderStatus
        {
            Id = 10,
            Name = "Payment Failed",
            Detail = "Payment could not be processed"
        };

        public static readonly OrderStatus ReturnRequested = new OrderStatus
        {
            Id = 11,
            Name = "Return Requested",
            Detail = "Customer has requested a return"
        };

        public static readonly OrderStatus Returned = new OrderStatus
        {
            Id = 12,
            Name = "Returned",
            Detail = "Item has been returned by customer"
        };

        public static IEnumerable<OrderStatus> All =>
                 new[]
        {
            ToPay,
            OrderSend,
            PickupBySeller,
            Processing,
            Shipped,
            Delivered,
            Completed,
            Cancelled,
            Refunded,
            FailedPayment,
            ReturnRequested,
            Returned
        };
    }

}
