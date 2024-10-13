using ArtistSocialNetwork.Areas.Admin.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System.Security.Claims;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : BaseController
    {
        public IAccountRepository AccountRepository = null;
        private readonly IRoleRepository roleRepository = null;
        private readonly IDocumentInfoRepository documentInfoRepository = null;

        public LoginController()
        {
            AccountRepository = new AccountRepository();
            roleRepository = new RoleRepository();
            documentInfoRepository = new DocumentInfoRepository();
        }

        public async Task<IActionResult> Index(string ReturnUrl = null)
        {
            TempData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(AccountSignIn userLogin)
        {
            if (ModelState.IsValid)
            {
                // Mã hóa mật khẩu người dùng nhập vào
                var email = userLogin.Email;
                var password = Commons.Library.EncryptMD5(userLogin.Password);

                // Lấy thông tin người dùng dựa vào email và mật khẩu đã mã hóa
                var user = await AccountRepository.GetAccountEmailPassWord(email, password);

                // Nếu tìm thấy người dùng và người dùng có quyền Admin
                if (user != null && user.IdRole == 1) // 1 là quyền Admin
                {
                    // Lưu thông tin ID người dùng vào session
                    HttpContext.Session.SetInt32("CurrentUserId", user.IdAccount);

                    // Lấy thông tin ảnh đại diện từ DocumentInfo theo Id của người dùng
                    var documentInfo = await documentInfoRepository.GetDocumentInfoByAccountId(user.IdAccount);
                    var profileImageUrl = documentInfo?.UrlDocument ?? "noanh.png"; // Nếu documentInfo null, dùng ảnh mặc định

                    // Lưu đường dẫn ảnh vào session
                    HttpContext.Session.SetString("ProfileImageUrl", profileImageUrl);

                    // Tạo danh sách claims để xác thực người dùng
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, email), // Tên của người dùng
                        new Claim("Email", user.Email),    // Email người dùng
                        new Claim(ClaimTypes.Role, "Admin"), // Gán quyền Admin
                        new Claim("ProfileImageUrl", profileImageUrl) // Lưu đường dẫn ảnh đại diện trong claim
                    };

                    // Tạo danh tính người dùng từ danh sách claims
                    var identity = new ClaimsIdentity(claims, "Admin");
                    var principal = new ClaimsPrincipal(identity);

                    // Đăng nhập vào hệ thống với scheme "Admin"
                    await HttpContext.SignInAsync("Admin", principal, new AuthenticationProperties()
                    {
                        IsPersistent = true // Đăng nhập vĩnh viễn (giữ phiên đăng nhập khi đóng trình duyệt)
                    });

                    // Chuyển hướng về trang trước đó nếu có
                    if (TempData["ReturnUrl"] != null)
                    {
                        return Redirect(TempData["ReturnUrl"].ToString());
                    }

                    // Nếu không có trang trước đó, chuyển hướng về trang chủ Admin
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else
                {
                    // Đăng nhập không thành công
                    TempData["Message"] = "Đăng nhập không thành công. Vui lòng kiểm tra lại email và mật khẩu hoặc quyền truy cập của bạn.";
                    TempData["AlertType"] = "danger";
                }
            }

            // Trả về trang đăng nhập nếu thông tin không hợp lệ
            return View(nameof(Index));
        }

        public IActionResult Logout()
        {
            // Đăng xuất và xóa session
            HttpContext.SignOutAsync("Admin");
            HttpContext.Session.Clear();
            SetAlert("Đăng xuất thành công!", "success");
            return RedirectToAction("Index", "Login", new { area = "Admin" });
        }
    }
}
