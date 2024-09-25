using ArtistSocialNetwork.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ArtistSocialNetwork.Controllers
{
    public class LoginController : BaseController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IDocumentInfoRepository _documentInfoRepository;

        public LoginController(IAccountRepository accountRepository, IRoleRepository roleRepository, IDocumentInfoRepository documentInfoRepository)
        {
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _documentInfoRepository = documentInfoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl = null)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginWeb userLogin)
        {
            if (ModelState.IsValid)
            {
                var email = userLogin.Email;
                var password = Commons.Library.EncryptMD5(userLogin.Password);
                var user = await _accountRepository.GetAccountEmailPassWord(email, password);

                if (user != null)
                {
                    // Lấy ảnh đại diện từ bảng DocumentInfo
                    var documentInfo = await _documentInfoRepository.GetByAccountId(user.IdAccount);
                    var profileImageUrl = documentInfo?.UrlDocument ?? "default-profile.png"; // Dùng ảnh mặc định nếu không có ảnh đại diện

                    // Lưu URL ảnh vào session
                    HttpContext.Session.SetString("ProfileImageUrl", profileImageUrl);

                    // Tạo các claims cho người dùng đã xác thực
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, email),
                        new Claim("Email", user.Email),
                        new Claim(ClaimTypes.Role, "User"),
                    };

                    // Tạo danh tính và thông tin người dùng
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // Đăng nhập người dùng
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                    {
                        IsPersistent = true
                    });

                    // Chuyển hướng về trang trước đó nếu có
                    if (TempData["ReturnUrl"] != null)
                    {
                        return Redirect(TempData["ReturnUrl"].ToString());
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Message"] = "Đăng nhập thất bại. Vui lòng kiểm tra email và mật khẩu của bạn, hoặc quyền truy cập.";
                    TempData["AlertType"] = "danger";
                }
            }

            return View(nameof(Index));
        }

        public async Task<IActionResult> Logout()
        {
            // Đăng xuất người dùng
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            SetAlert("Đăng xuất thành công!", "success");

            // Chuyển hướng về trang đăng nhập hoặc trang chủ
            return RedirectToAction("Index", "Login");
        }
    }
}
