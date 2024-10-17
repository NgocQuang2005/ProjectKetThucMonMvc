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
            var accountdetail = await _context.AccountDetails
                                              .AsNoTracking()  // Sử dụng AsNoTracking để ngăn chặn theo dõi thực thể
                                              .FirstOrDefaultAsync(ad => ad.IdAccountDt == id);
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
            var existingItem = await _context.AccountDetails.AsNoTracking().FirstOrDefaultAsync(ad => ad.IdAccountDt == accountdetail.IdAccountDt);
            if (existingItem != null)
            {
                // Attach entity and set state to Modified
                _context.AccountDetails.Attach(accountdetail);
                _context.Entry(accountdetail).State = EntityState.Modified;
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
        // Lấy tất cả tài khoản đang hoạt động
        public async Task<IEnumerable<AccountDetail>> GetActiveAccounts()
        {
            return await _context.AccountDetails
                                 .Include(ad => ad.account)
                                 .Where(ad => ad.Active) // Chỉ lấy những tài khoản đang hoạt động
                                 .ToListAsync();
        }

        // Lấy tất cả tài khoản không hoạt động
        public async Task<IEnumerable<AccountDetail>> GetInactiveAccounts()
        {
            return await _context.AccountDetails
                                 .Include(ad => ad.account)
                                 .Where(ad => !ad.Active) // Chỉ lấy những tài khoản không hoạt động
                                 .ToListAsync();
        }
    }

}

