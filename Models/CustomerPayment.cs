using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Internal;
using System.ComponentModel.DataAnnotations;

namespace E_Commers_Adelia.Models
{
    public class CustomerPayment
    {
        [Key]
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public int PaymentTypeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? ResitUrl { get; set; }
        public bool PaymentComfirmation { get; set; }
        public DateTime PaymentComfrmDate { get; set; }

    }
}
