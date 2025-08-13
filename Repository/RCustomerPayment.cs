using E_Commers_Adelia.Data;
using E_Commers_Adelia.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commers_Adelia.Repository
{
    public class RCustomerPayment : ICustomerPayment
    {
        private readonly ApplicationDbContext _db;
        public RCustomerPayment(ApplicationDbContext db) 
        {
            _db = db;
        }
        public async Task<bool> Comfirm(string orderNo)
        {
            var payment = await _db.CustomerPayments.FirstOrDefaultAsync(x => x.OrderNo == orderNo);
            if (payment != null)
            {
                payment.PaymentComfirmation = true;
                payment.PaymentComfrmDate = DateTime.UtcNow;
                _db.CustomerPayments.Update(payment);
                await _db.SaveChangesAsync();   
                return true;
            }
            return false;
        }

        public async Task<bool> Create(string orderNo, int paymentTypeId, decimal amount, string resitUrl)
        {
            var payment = new CustomerPayment
            {
                OrderNo = orderNo,
                PaymentTypeId = paymentTypeId,
                Amount = amount,
                PaymentDate = DateTime.UtcNow,
                ResitUrl = resitUrl
            };
            _db.CustomerPayments.Add(payment);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<CustomerPayment> GetById(int id)
        {
            return(await _db.CustomerPayments.FindAsync(id));
        }

        public async Task<CustomerPayment> GetByOrderNo(string orderNo)
        {
            return(await _db.CustomerPayments.FirstOrDefaultAsync(x => x.OrderNo == orderNo));
        }
    }
}
