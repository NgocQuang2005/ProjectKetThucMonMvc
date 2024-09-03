using Business;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return await AccountDAO.Instance.GetAccountAll();
        }

        public async Task<Account> GetAccountById(int id)
        {
            return await AccountDAO.Instance.GetAccountById(id);
        }
        public async Task Update(Account account)
        {
            await AccountDAO.Instance.Update(account);
        }
        public async Task<Account> GetAccountEmailPassWord(string email, string password) => await AccountDAO.Instance.GetAccountEmailPassWord(email, password);

    }
}
