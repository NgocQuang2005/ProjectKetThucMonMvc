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
        public IAccountRepository AccountReponsitory = null;
        private readonly IRoleRepository roleRepository = null;
        public LoginController()
        {
            AccountReponsitory = new AccountRepository();
            roleRepository = new RoleRepository();
        }
        public async Task<IActionResult> Index(string ReturnUrl = null)
        {
            TempData["ReturnUrl"] = ReturnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(AccountSignIn userlogin)
        {

            if (ModelState.IsValid)
            {
                var email = userlogin.Email;
                var passWord = Commons.Library.EncryptMD5(userlogin.Password);
                var user = await AccountReponsitory.GetAccountEmailPassWord(email, passWord);
                if (user != null && user.IdRole == 1)
                {
                    // A claim is a statement about a subject by an issuer and
                    //represent attributes of the subject that are useful in the context of authentication and authorization operations.
                    var claims = new List<Claim>() {
                        new Claim(ClaimTypes.Name, email),
                        new Claim("Email", user.Email),
                        new Claim(ClaimTypes.Role, "Admin"),
                    };
                    //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme    
                    var identity = new ClaimsIdentity(claims, "Admin");
                    //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity    
                    var principal = new ClaimsPrincipal(identity);
                    //SignInAsync is a Extension method for Sign in a principal for the specified scheme.    
                    HttpContext.SignInAsync("Admin", principal, new AuthenticationProperties()
                    {
                        IsPersistent = true
                    });
                    var routeValues = new RouteValueDictionary
                    {
                        {"area","Admin" },
                        {"returnURL",Request.Query["ReturnUrl"] },
                        {"claimValue","true" }
                    };
                    if (TempData["ReturnUrl"] != null)
                    {
                        return Redirect(TempData["ReturnUrl"].ToString());
                    }
                    return RedirectToAction("Index", "Home", routeValues);
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
            // Đăng xuất người dùng
            HttpContext.SignOutAsync("Admin");
            SetAlert("Đăng xuất thành công!", "success");
            // Chuyển hướng đến trang đăng nhập hoặc trang chính
            return RedirectToAction("Index", "Login", new { area = "Admin" });
            // Thay thế bằng tên trang đăng nhập hoặc trang chính của bạn
        }
    }
}
