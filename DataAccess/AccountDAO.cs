using Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class AccountDAO : SingletonBase<AccountDAO>
    {
        public async Task<IEnumerable<Account>> GetAccountAll()
        {
            var account = await _context.Accounts.Include(a => a.AccountRole).ToListAsync();
            return account;
        }
        public async Task<Account> GetAccountById(int id)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.IdAccount == id);
            if (account == null) return null;

            return account;
        }
        public async Task Add(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Account account)
        {

            var existingItem = await GetAccountById(account.IdAccount);
            if (existingItem != null)
            {
                // Cập nhật các thuộc tính cần thiết
                _context.Entry(existingItem).CurrentValues.SetValues(account);
                await _context.SaveChangesAsync();
            }

        }
        public async Task Delete(int id)
        {
            var account = await GetAccountById(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Account> GetAccountEmailPassWord(string email, string password)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email.Equals(email) && a.Password.Equals(password));
            if (account == null) return null;
            return account;
        }
    }
}
