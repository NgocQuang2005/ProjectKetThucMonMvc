using Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class AccountDetailDAO : SingletonBase<AccountDetailDAO>
    {
        public async Task<IEnumerable<AccountDetail>> GetAccountDetailAll()
        {
            var accountdetail = await _context.AccountDetails.Include(ad => ad.account).ToListAsync();
            return accountdetail;
        }
        public async Task<AccountDetail> GetAccountDetailById(int id)
        {
            var accountdetail = await _context.AccountDetails.FirstOrDefaultAsync(ad => ad.IdAccountDt == id);
            if (accountdetail == null) return null;

            return accountdetail;
        }
        public async Task Add(AccountDetail accountdetail)
        {
            _context.AccountDetails.Add(accountdetail);
            await _context.SaveChangesAsync();
        }
        public async Task Update(AccountDetail accountdetail)
        {

            var existingItem = await GetAccountDetailById(accountdetail.IdAccountDt);
            if (existingItem != null)
            {
                // Cập nhật các thuộc tính cần thiết
                _context.Entry(existingItem).CurrentValues.SetValues(accountdetail);
                await _context.SaveChangesAsync();
            }

        }
        public async Task Delete(int id)
        {
            var accountdetail = await GetAccountDetailById(id);
            if (accountdetail != null)
            {
                _context.AccountDetails.Remove(accountdetail);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ChangeActive(int id)
        {
            var accountdetail = await GetAccountDetailById(id);
            accountdetail.Active = !accountdetail.Active;
            await _context.SaveChangesAsync();
            return accountdetail.Active;
        }
    }

}

