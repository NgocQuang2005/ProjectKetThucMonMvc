using Business;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess
{
    public class AccountDAO : SingletonBase<AccountDAO>
    {
        // Lấy tất cả các tài khoản, bao gồm thông tin vai trò và ảnh liên quan
        public IQueryable<Account> GetAccountAll()
        {
            return _context.Accounts
                           .Include(a => a.AccountRole) // Bao gồm dữ liệu vai trò
                           .Include(a => a.DocumentInfos) // Bao gồm dữ liệu ảnh
                           .AsNoTracking(); // Sử dụng AsNoTracking() để tối ưu hóa truy vấn
        }

        public async Task<Account> GetAccountById(int id)
        {
            return await _context.Accounts
                                 .Include(a => a.DocumentInfos) // Bao gồm dữ liệu ảnh
                                 .AsNoTracking() // Sử dụng AsNoTracking() để giảm tải
                                 .FirstOrDefaultAsync(a => a.IdAccount == id);
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
            return await _context.Accounts
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(a => a.Email == email && a.Password == password);
        }
    }
}
