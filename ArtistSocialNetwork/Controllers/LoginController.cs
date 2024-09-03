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

        public LoginController(IAccountRepository accountRepository, IRoleRepository roleRepository)
        {
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
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
                    // Create claims for the authenticated user
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, email),
                        new Claim("Email", user.Email),
                        new Claim(ClaimTypes.Role, "User"),
                    };

                    // Create an identity and principal
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    // Sign in the user
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                    {
                        IsPersistent = true
                    });

                    // Redirect to the return URL if available
                    if (TempData["ReturnUrl"] != null)
                    {
                        return Redirect(TempData["ReturnUrl"].ToString());
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Message"] = "Login failed. Please check your email and password, or your access rights.";
                    TempData["AlertType"] = "danger";
                }
            }

            return View(nameof(Index));
        }

        public IActionResult Logout()
        {
            // Sign out the user
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            SetAlert("Successfully logged out!", "success");

            // Redirect to the login page or the home page
            return RedirectToAction("Index", "Login");
        }
    }
}
