using ArtistSocialNetwork.Models;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System.Linq;
using System.Threading.Tasks;
using Business;

namespace ArtistSocialNetwork.Controllers
{
    public class ArtistsController : BaseController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IDocumentInfoRepository _documentInfoRepository;
        private readonly IAccountDetailRepository _accountDetailRepository;

        // Constructor
        public ArtistsController(
            ILogger<BaseController> logger,
            ApplicationDbContext context,
            IAccountRepository accountRepository,
            IRoleRepository roleRepository,
            IDocumentInfoRepository documentInfoRepository,
            IAccountDetailRepository accountDetailRepository)
            : base(logger, context)
        {
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _documentInfoRepository = documentInfoRepository;
            _accountDetailRepository = accountDetailRepository;
        }

        public async Task<IActionResult> Index(string search = "")
        {
            // Lấy tất cả các vai trò có IdRole là 1 hoặc 3
            var roles = await _roleRepository.GetRoleAll();
            var relevantRoles = roles.Where(r => r.IdRole == 3 || r.IdRole == 1).ToList();

            // Nếu không tìm thấy vai trò nào, trả về lỗi
            if (!relevantRoles.Any())
            {
                return StatusCode(500, "Không tìm thấy vai trò 'nghệ sĩ' hoặc 'vai trò 1'.");
            }

            // Lấy danh sách tất cả tài khoản và lọc theo vai trò nghệ sĩ hoặc vai trò với IdRole = 1
            var allAccounts = await _accountRepository.GetAccountAll();
            var artistAccounts = allAccounts.Where(a => relevantRoles.Any(r => r.IdRole == a.IdRole));

            // Tìm kiếm nghệ sĩ theo tên nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                artistAccounts = artistAccounts.Where(a => a.AccountDetail.Fullname.ToLower().Contains(search));
            }

            // Lấy ID tài khoản đang đăng nhập từ session
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

            // Nếu tài khoản hiện tại là nghệ sĩ, thay đổi FullName thành "bạn"
            foreach (var account in artistAccounts)
            {
                if (account.IdAccount == currentUserId)
                {
                    account.AccountDetail.Fullname = "Bạn"; // Thay đổi FullName thành "bạn" nếu là nghệ sĩ
                }
            }

            var artistList = artistAccounts.ToList();
            ViewBag.Search = search;

            return View(artistList);
        }

    }
}
