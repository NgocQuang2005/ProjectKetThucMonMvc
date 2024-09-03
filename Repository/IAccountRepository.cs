using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAccountAll();
        Task<Account> GetAccountById(int id);
        Task Add(Account account);
        Task Update(Account account);
        Task Delete(int id);
        Task<Account> GetAccountEmailPassWord(string email, string password);
    }
}
