using ArtistSocialNetwork.Models;
using Business;
using Microsoft.AspNetCore.Mvc;
using Commons;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ArtistSocialNetwork.Controllers
{
    public class SignUpController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public SignUpController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Initialize gender dropdown options with SelectListItem
            ViewBag.GenderOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Nam", Text = "Nam" },
                new SelectListItem { Value = "Nữ", Text = "Nữ" },
                new SelectListItem { Value = "Khác", Text = "Khác" }
            };

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SignUpWeb model)
        {
            if (ModelState.IsValid)
            {
                // Check if the email is already taken
                var existingAccount = _context.Accounts
                    .FirstOrDefault(a => a.Email == model.Email);

                if (existingAccount != null)
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View(model);
                }

                // Create a new account
                var account = new Account
                {
                    Email = model.Email,
                    Password = Commons.Library.EncryptMD5(model.Password),  // Encrypt the password
                    IdRole = 2,  // Default to User role
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
                    CreatedWhen = DateTime.Now,
                    LastUpdateWhen = DateTime.Now,
                    CreatedBy = account.IdAccount,  // Assuming the user creates their own details
                    LastUpdateBy = account.IdAccount
                };

                _context.AccountDetails.Add(accountDetail);
                await _context.SaveChangesAsync();

                // Set success alert and redirect to the login page
                SetAlert(Contants.Update_success, Contants.success);
                return RedirectToAction("Index", "Login");
            }

            // Reload gender options if validation fails
            ViewBag.GenderOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Nam", Text = "Nam" },
                new SelectListItem { Value = "Nữ", Text = "Nữ" },
                new SelectListItem { Value = "Khác", Text = "Khác" }
            };

            // Set error alert if model validation fails
            SetAlert(Contants.PASSWORD_FAIL, Contants.FAIL);
            return View(model);
        }
    }
}
