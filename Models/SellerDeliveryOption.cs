namespace E_Commers_Adelia.Models
{
    public class SellerDeliveryOption
    {
        public int Id { get; set; }
        public required int DeliveryId { get; set; }
        public required string userId { get; set; }
        public bool isDisable { get; set; }
    }
}
