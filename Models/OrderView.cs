namespace E_Commers_Adelia.Models
{
    public class OrderView
    {
        public string OrderNo { get; set; }
        public List<Order> OrderItem { get; set; }
        public string SellerId {  get; set; }
        public decimal TotalToPay { get; set; }
        public OrderPlace OrderPlace { get; set; }
    }
}
