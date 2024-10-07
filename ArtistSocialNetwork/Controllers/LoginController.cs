using ArtistSocialNetwork.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Business;

namespace ArtistSocialNetwork.Controllers
{
    public class LoginController : BaseController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IDocumentInfoRepository _documentInfoRepository;

        public LoginController(IAccountRepository accountRepository, IRoleRepository roleRepository, IDocumentInfoRepository documentInfoRepository, ILogger<LoginController> logger, ApplicationDbContext context)
            : base(logger, context) // Truyền logger và context đến BaseController
        {
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _documentInfoRepository = documentInfoRepository;
        }

        [HttpGet]
        public IActionResult Index(string returnUrl = null)
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
                    var documentInfo = await _documentInfoRepository.GetDocumentInfoByAccountId(user.IdAccount);
                    var profileImageUrl = documentInfo?.UrlDocument ?? "default-profile.png";

                    // Lưu URL ảnh và ID người dùng vào session
                    HttpContext.Session.SetString("ProfileImageUrl", profileImageUrl);
                    HttpContext.Session.SetInt32("CurrentUserId", user.IdAccount);

                    // Tạo các claims cho người dùng đã xác thực
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, email),
                        new Claim("Email", user.Email),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim("UserId", user.IdAccount.ToString())
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
                    TempData["Message"] = "Đăng nhập thất bại. Vui lòng kiểm tra email và mật khẩu của bạn.";
                    TempData["AlertType"] = "danger";
                }
            }
            else
            {
                TempData["Message"] = "Vui lòng nhập đầy đủ thông tin.";
                TempData["AlertType"] = "warning";
            }

            // Nếu đăng nhập thất bại, trả về lại view
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            // Đăng xuất người dùng
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear(); // Xóa session khi đăng xuất
            SetAlert("Đăng xuất thành công!", "success");

            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("Index", "Login");
        }
    }
}
