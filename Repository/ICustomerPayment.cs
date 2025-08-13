using E_Commers_Adelia.Models;

namespace E_Commers_Adelia.Repository
{
    public interface ICustomerPayment
    {
        Task<CustomerPayment> GetByOrderNo(string orderNo);
        Task<CustomerPayment> GetById(int id);
        Task<bool> Create(string orderNo, int paymentTypeId, decimal amount, string resitUrl);
        Task<bool> Comfirm(string orderNo);
    }
}
