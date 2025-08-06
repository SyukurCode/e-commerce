namespace E_Commers_Adelia.Models
{
    public class OrderView
    {
        public List<Order> Orders { get; set; }
        public string SellerId {  get; set; }
        public decimal TotalToPay { get; set; }


    }
}
