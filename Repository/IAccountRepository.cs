using Business;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAccountAll();
        Task<Account> GetAccountById(int id);
        Task<Account> GetAccountByIdAsNoTracking(int id); // Thêm phương thức mới này
        Task Add(Account account);
        Task Update(Account account);
        Task Delete(int id);
        Task<Account> GetAccountEmailPassWord(string email, string password);
        Task<Account> GetAccountByEmailOrPhone(string emailOrPhone);
    }
}
