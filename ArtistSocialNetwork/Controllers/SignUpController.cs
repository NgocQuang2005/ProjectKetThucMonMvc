using ArtistSocialNetwork.Models;
using Business;
using Microsoft.AspNetCore.Mvc;
using Commons;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace ArtistSocialNetwork.Controllers
{
    public class SignUpController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public SignUpController(ApplicationDbContext context, ILogger<SignUpController> logger) : base(logger, context) // Truyền logger và context đến BaseController
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Initialize gender dropdown options
            ViewBag.GenderOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Nam", Text = "Nam" },
                new SelectListItem { Value = "Nữ", Text = "Nữ" },
                new SelectListItem { Value = "Khác", Text = "Khác" }
            };

            // Initialize role dropdown options
            ViewBag.RoleOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "2", Text = "Người Dùng" },
                new SelectListItem { Value = "3", Text = "Nghệ Sỹ" }
            };

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SignUpWeb model)
        {
            // Set gender and role options before validating ModelState
            ViewBag.GenderOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Nam", Text = "Nam" },
                new SelectListItem { Value = "Nữ", Text = "Nữ" },
                new SelectListItem { Value = "Khác", Text = "Khác" }
            };

            ViewBag.RoleOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "2", Text = "Người Dùng" },
                new SelectListItem { Value = "3", Text = "Nghệ Sỹ" }
            };

            if (ModelState.IsValid)
            {
                // Check if the email is already taken
                var existingAccount = _context.Accounts.FirstOrDefault(a => a.Email == model.Email);

                if (existingAccount != null)
                {
                    ModelState.AddModelError("Email", "Email đã được đăng ký.");
                    return View(model);
                }

                // Create a new account
                var account = new Account
                {
                    Email = model.Email,
                    Password = Commons.Library.EncryptMD5(model.Password),
                    IdRole = model.IdRole,
                    CreatedWhen = DateTime.Now,
                    LastUpdateWhen = DateTime.Now
                };

                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                // Create the account detail with the associated account ID
                var accountDetail = new AccountDetail
                {
                    IdAccount = account.IdAccount,
                    Active = true,
                    Fullname = model.Fullname,
                    Gender = model.Gender,
                    Birthday = model.Birthday,
                    Nationality = model.Nationality,
                    CCCD = model.CCCD,
                    CreatedWhen = DateTime.Now,
                    LastUpdateWhen = DateTime.Now,
                    CreatedBy = account.IdAccount,
                    LastUpdateBy = account.IdAccount
                };

                _context.AccountDetails.Add(accountDetail);
                await _context.SaveChangesAsync();

                // Set success alert and redirect to the login page
                SetAlert(Contants.Update_success, Contants.success);
                return RedirectToAction("Index", "Login");
            }

            // Set error alert if model validation fails
            SetAlert(Contants.PASSWORD_FAIL, Contants.FAIL);
            return View(model);
        }
    }
}
