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

        public static readonly OrderStatus Completed = new OrderStatus
        {
            Id = 5,
            Name = "Completed",
            Detail = "Order has been completed successfully"
        };

        public static readonly OrderStatus Cancelled = new OrderStatus
        {
            Id = 6,
            Name = "Cancelled",
            Detail = "Order has been cancelled by seller"
        };

        public static readonly OrderStatus Shiped = new OrderStatus
        {
            Id = 7,
            Name = "Delivered",
            Detail = "Order has been deliverd to customer"
        };

        public static IEnumerable<OrderStatus> All =>
                 new[]
        {
            ToPay,
            OrderSend,
            PickupBySeller,
            Processing,
            Completed,
            Cancelled
        };
    }
    public class AccountType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public static readonly AccountType Real = new AccountType
        {
            Id = 1,
            Name = "Real"
        };
        public static readonly AccountType Dummy = new AccountType
        {
            Id = 2,
            Name = "Dummy"
        };
        public static readonly AccountType Test = new AccountType
        {
            Id = 3,
            Name = "Test",
        };
        public static IEnumerable<AccountType> All => new[] {
            Real,
            Dummy,
            Test
        };

    };
}
