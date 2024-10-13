using Business;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess
{
    public class AccountDAO : SingletonBase<AccountDAO>
    {
        private readonly ApplicationDbContext _context;

        public AccountDAO()
        {
            _context = new ApplicationDbContext();
        }

        // Lấy tất cả các tài khoản, bao gồm thông tin vai trò và ảnh liên quan
        public IQueryable<Account> GetAccountAll()
        {
            return _context.Accounts
                           .Include(a => a.AccountRole)
                           .Include(a => a.AccountDetail)
                           .Include(a => a.DocumentInfos)
                           .AsNoTracking(); // để tối ưu hóa truy vấn
        }

        public async Task<Account> GetAccountById(int id)
        {
            return await _context.Accounts
                                 .Include(a => a.AccountRole)
                                 .Include(a => a.DocumentInfos)
                                 .Include(a => a.AccountDetail)
                                 .FirstOrDefaultAsync(a => a.IdAccount == id);
        }

        public async Task<Account> GetAccountByIdAsNoTracking(int id)
        {
            return await _context.Accounts
                                 .AsNoTracking()
                                 .Include(a => a.AccountRole)
                                 .Include(a => a.DocumentInfos)
                                 .Include(a => a.AccountDetail)
                                 .FirstOrDefaultAsync(a => a.IdAccount == id);
        }

        public async Task Add(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Account account)
        {
            var existingItem = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.IdAccount == account.IdAccount);
            if (existingItem != null)
            {
                // Attach entity and set state to Modified
                _context.Accounts.Attach(account);
                _context.Entry(account).State = EntityState.Modified;
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

        public async Task<Account> GetAccountByEmailOrPhone(string emailOrPhone)
        {
            return await _context.Accounts
                .Include(a => a.AccountRole)        // Nạp dữ liệu vai trò của tài khoản
                .Include(a => a.AccountDetail)      // Nạp dữ liệu chi tiết của tài khoản
                .Include(a => a.DocumentInfos)      // Nạp dữ liệu liên quan đến tài liệu
                .AsNoTracking()                    // Tối ưu hóa hiệu suất
                .FirstOrDefaultAsync(a => a.Email == emailOrPhone || a.Phone == emailOrPhone);
        }
    }
}
