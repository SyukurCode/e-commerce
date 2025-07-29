namespace E_Commers_Adelia.Models
{
    public class OrderViewcs
    {
        // Maklumat Pengguna
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // Alamat Penghantaran
        public string Address { get; set; }

        // Senarai Item Dalam Cart
        public List<CartItem> CartItems { get; set; } = new();

        // Ringkasan Harga
        public decimal Total {  get; set; }
        // Pilihan Penghantaran
        // 1 - Delivery by seller
        // 2 - Self pickup
        public int DeliveryOptionId { get; set; }

        // Pilihan Pembayaran
        // 1 - QR Code
        // 2 - Cash on delivery
        // 3 - Cash
        public int PaymentMethodId { get; set; }

        // Untuk display order number nanti
        public string OrderNumber { get; set; }
    }
}
