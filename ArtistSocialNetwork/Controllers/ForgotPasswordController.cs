using Business;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace ArtistSocialNetwork.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public ForgotPasswordController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // Hiển thị form nhập email hoặc số điện thoại
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Xử lý khi người dùng nhập email hoặc số điện thoại
        [HttpPost]
        public async Task<IActionResult> CheckAccount(string emailOrPhone)
        {
            Account account = null;

            // Kiểm tra email hoặc số điện thoại
            if (emailOrPhone.Contains("@"))
            {
                account = (await _accountRepository.GetAccountAll())
                            .FirstOrDefault(a => a.Email == emailOrPhone);
            }
            else
            {
                account = (await _accountRepository.GetAccountAll())
                            .FirstOrDefault(a => a.Phone == emailOrPhone);
            }

            if (account == null)
            {
                ViewBag.Message = "Email hoặc số điện thoại không tồn tại!";
                return View("Index"); // Quay lại trang Index nếu không tìm thấy
            }

            // Nếu tìm thấy tài khoản, lưu ID người dùng vào TempData
            TempData["AccountId"] = account.IdAccount;
            TempData.Keep("AccountId"); // Giữ TempData sau redirect để dùng sau
            return RedirectToAction(nameof(EditPassword));
        }

        // Hiển thị form đổi mật khẩu
        [HttpGet]
        public IActionResult EditPassword()
        {
            if (TempData["AccountId"] == null)
            {
                return RedirectToAction(nameof(Index));
            }

            TempData.Keep("AccountId"); // Đảm bảo giữ TempData cho đến khi sử dụng
            return View();
        }

        // Xử lý thay đổi mật khẩu
        [HttpPost]
        public async Task<IActionResult> EditPassword(string newPassword)
        {
            if (TempData["AccountId"] == null)
            {
                return RedirectToAction(nameof(Index));
            }

            int accountId = (int)TempData["AccountId"];
            TempData.Keep("AccountId"); // Đảm bảo giữ lại TempData sau khi sử dụng

            var account = await _accountRepository.GetAccountById(accountId);

            if (account != null)
            {
                // Thay đổi mật khẩu, giả sử hàm mã hóa mật khẩu là EncryptMD5
                account.Password = Commons.Library.EncryptMD5(newPassword);
                await _accountRepository.Update(account);

                // Sau khi đổi mật khẩu thành công, chuyển về trang đăng nhập
                return RedirectToAction("Index", "Login");
            }

            // Nếu có lỗi, thông báo lại cho người dùng
            ViewBag.Message = "Có lỗi xảy ra, vui lòng thử lại!";
            return View();
        }
    }
}
