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
}
