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
                var email = userLogin.Email;
                var password = Commons.Library.EncryptMD5(userLogin.Password);
                var user = await AccountRepository.GetAccountEmailPassWord(email, password);

                if (user != null && user.IdRole == 1) // Check quyền Admin
                {
                    // Lưu thông tin người dùng vào session
                    HttpContext.Session.SetInt32("CurrentUserId", user.IdAccount);

                    // Lấy ảnh profile từ DocumentInfo theo Id của người dùng
                    var documentInfo = await documentInfoRepository.GetDocumentInfoById(user.IdAccount);
                    var profileImageUrl = documentInfo?.UrlDocument; // Trích xuất đường dẫn ảnh từ UrlDocument nếu documentInfo không null

                    // Tạo claims để xác thực người dùng
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, email),
                        new Claim("Email", user.Email),
                        new Claim(ClaimTypes.Role, "Admin"),
                        new Claim("ProfileImageUrl", !string.IsNullOrEmpty(profileImageUrl) ? profileImageUrl : "~/Admin/images/avatars/avtar_1.png"), // Nếu profileImageUrl null, dùng ảnh mặc định
                    };

                    var identity = new ClaimsIdentity(claims, "Admin");
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync("Admin", principal, new AuthenticationProperties()
                    {
                        IsPersistent = true
                    });

                    if (TempData["ReturnUrl"] != null)
                    {
                        return Redirect(TempData["ReturnUrl"].ToString());
                    }
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else
                {
                    TempData["Message"] = "Đăng nhập không thành công. Vui lòng kiểm tra lại email và mật khẩu hoặc quyền truy cập của bạn.";
                    TempData["AlertType"] = "danger";
                }
            }
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
