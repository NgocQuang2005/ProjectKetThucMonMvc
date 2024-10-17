using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System.Linq;
using System.Threading.Tasks;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class AccountReportController : BaseController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountDetailRepository _accountDetailRepository;
        private const int PageSize = 10; // Số lượng tài khoản trên mỗi trang

        public AccountReportController(IAccountRepository accountRepository, IAccountDetailRepository accountDetailRepository)
        {
            _accountRepository = accountRepository;
            _accountDetailRepository = accountDetailRepository;
        }

        // Kết hợp cả thống kê và báo cáo
        public async Task<IActionResult> Index(int page = 1)
        {
            var totalUsers = await _accountRepository.GetTotalAccounts();  // Tổng số tài khoản
            var activeUsers = (await _accountDetailRepository.GetActiveAccounts()).Count(); // Số tài khoản đang hoạt động
            var inactiveUsers = (await _accountDetailRepository.GetInactiveAccounts()).Count(); // Số tài khoản không hoạt động

            // Thống kê tài khoản theo thời gian
            var accountStats = await GetAccountStatisticsPerDay();

            // Phân trang cho danh sách tài khoản
            var accounts = (await _accountRepository.GetAccountAll())
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Tổng số trang dựa trên tổng số tài khoản
            var totalPages = (int)Math.Ceiling((double)totalUsers / PageSize);

            ViewBag.TotalUsers = totalUsers;
            ViewBag.ActiveUsers = activeUsers;
            ViewBag.InactiveUsers = inactiveUsers;
            ViewBag.AccountStats = accountStats;
            ViewBag.AccountDetails = accounts;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View();
        }

        // Hàm hỗ trợ tính thống kê tài khoản theo ngày
        private async Task<List<dynamic>> GetAccountStatisticsPerDay()
        {
            var accounts = await _accountRepository.GetAccountAll();
            var dailyStats = accounts
                .Where(a => a.CreatedWhen.HasValue)
                .GroupBy(a => a.CreatedWhen.Value.Date)
                .Select(group => new
                {
                    Date = group.Key,
                    TotalRegistrations = group.Count()
                })
                .OrderBy(x => x.Date)
                .ToList<dynamic>();

            return dailyStats;
        }
    }
}
