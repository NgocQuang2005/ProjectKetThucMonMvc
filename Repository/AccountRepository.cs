using Business;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public class AccountRepository : IAccountRepository
    {
        public async Task Add(Account account)
        {
            await AccountDAO.Instance.Add(account);
        }

        public async Task Delete(int id)
        {
            await AccountDAO.Instance.Delete(id);
        }

        public async Task<IEnumerable<Account>> GetAccountAll()
        {
            var accounts = await AccountDAO.Instance.GetAccountAll().ToListAsync();
            return accounts ?? new List<Account>(); // Đảm bảo luôn trả về danh sách, ngay cả khi null
        }

        public async Task<Account> GetAccountById(int id)
        {
            return await AccountDAO.Instance.GetAccountById(id) ?? new Account(); // Trả về một đối tượng Account mới nếu null
        }

        public async Task<Account> GetAccountByIdAsNoTracking(int id)
        {
            return await AccountDAO.Instance.GetAccountByIdAsNoTracking(id);
        }

        public async Task Update(Account account)
        {
            await AccountDAO.Instance.Update(account);
        }

        public async Task<Account> GetAccountEmailPassWord(string email, string password)
        {
            return await AccountDAO.Instance.GetAccountEmailPassWord(email, password);
        }

        public async Task<Account> GetAccountByEmailOrPhone(string emailOrPhone)
        {
            return await AccountDAO.Instance.GetAccountByEmailOrPhone(emailOrPhone);
        }
        public async Task<int> GetTotalAccounts()
        {
            return await AccountDAO.Instance.GetTotalAccounts();
        }
    }
}
