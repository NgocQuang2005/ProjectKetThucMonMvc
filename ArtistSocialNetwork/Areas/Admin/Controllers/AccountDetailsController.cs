using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Repository;
using X.PagedList;
using Microsoft.AspNetCore.Http; // Để sử dụng Session
using Microsoft.EntityFrameworkCore;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class AccountDetailsController : BaseController
    {
        private readonly IAccountDetailRepository accountDetailRepository;
        private readonly IAccountRepository accountRepository;

        public AccountDetailsController()
        {
            accountRepository = new AccountRepository();
            accountDetailRepository = new AccountDetailRepository();
        }

        public async Task<IActionResult> Index(string searchString, int? page)
        {
            var accountDetails = await accountDetailRepository.GetAccountDetailAll();
            if (!string.IsNullOrEmpty(searchString))
            {
                accountDetails = accountDetails
                    .Where(c => Commons.Library.ConvertToUnSign(c.Fullname.ToLower())
                    .Contains(Commons.Library.ConvertToUnSign(searchString.ToLower())))
                    .ToList();
            }

            ViewBag.Page = 5;
            return View(accountDetails.ToPagedList(page ?? 1, (int)ViewBag.Page));
        }

        public async Task<IActionResult> Create()
        {
            // Lấy danh sách tài khoản để gán vào SelectList cho dropdown
            ViewData["IdAccount"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAccountDt,Active,Fullname,IdAccount,CCCD,Description,Birthday,Nationality,Gender,Address")] AccountDetail accountDetail)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

                if (currentUserId == null)
                {
                    ModelState.AddModelError("", "Không thể xác định người dùng hiện tại.");
                    return View(accountDetail);
                }

                // Kiểm tra nếu ngày sinh có tồn tại
                if (accountDetail.Birthday != null)
                {
                    try
                    {
                        // Chuyển đổi định dạng ngày sinh từ chuỗi "dd/MM/yyyy" về kiểu DateTime
                        accountDetail.Birthday = DateTime.ParseExact(accountDetail.Birthday?.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);
                    }
                    catch (FormatException)
                    {
                        ModelState.AddModelError("Birthday", "Ngày sinh không hợp lệ. Vui lòng nhập đúng định dạng (dd/MM/yyyy).");
                        ViewData["IdAccount"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", accountDetail.IdAccount);
                        return View(accountDetail);
                    }
                }

                // Gán CreatedBy và LastUpdateBy là người dùng hiện tại
                accountDetail.CreatedBy = currentUserId.Value;
                accountDetail.LastUpdateBy = currentUserId.Value;
                accountDetail.CreatedWhen = DateTime.Now;
                accountDetail.LastUpdateWhen = DateTime.Now;

                await accountDetailRepository.Add(accountDetail);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdAccount"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", accountDetail.IdAccount);
            return View(accountDetail);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountDetail = await accountDetailRepository.GetAccountDetailById(Convert.ToInt32(id));
            if (accountDetail == null)
            {
                return NotFound();
            }

            // Lấy danh sách tài khoản để gán vào SelectList
            ViewData["IdAccount"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", accountDetail.IdAccount);
            return View(accountDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAccountDt,Active,Fullname,IdAccount,CCCD,Description,Birthday,Nationality,Gender,Address,CreatedBy,CreatedWhen")] AccountDetail accountDetail)
        {
            if (id != accountDetail.IdAccountDt)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Lấy ID người dùng hiện tại từ Session
                var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

                if (currentUserId == null)
                {
                    ModelState.AddModelError("", "Không thể xác định người dùng hiện tại.");
                    return View(accountDetail);
                }

                // Cập nhật LastUpdateBy và LastUpdateWhen
                accountDetail.LastUpdateBy = currentUserId.Value;
                accountDetail.LastUpdateWhen = DateTime.Now;

                try
                {
                    // Lấy thực thể từ cơ sở dữ liệu trước khi cập nhật
                    var existingAccountDetail = await accountDetailRepository.GetAccountDetailById(accountDetail.IdAccountDt);

                    if (existingAccountDetail == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các trường cần thay đổi
                    existingAccountDetail.Fullname = accountDetail.Fullname;
                    existingAccountDetail.Active = accountDetail.Active;
                    existingAccountDetail.IdAccount = accountDetail.IdAccount;
                    existingAccountDetail.CCCD = accountDetail.CCCD;
                    existingAccountDetail.Description = accountDetail.Description;
                    existingAccountDetail.Birthday = accountDetail.Birthday;
                    existingAccountDetail.Nationality = accountDetail.Nationality;
                    existingAccountDetail.Gender = accountDetail.Gender;
                    existingAccountDetail.Address = accountDetail.Address;
                    existingAccountDetail.LastUpdateBy = accountDetail.LastUpdateBy;
                    existingAccountDetail.LastUpdateWhen = accountDetail.LastUpdateWhen;

                    // Gọi hàm update trong repository
                    await accountDetailRepository.Update(existingAccountDetail);
                    SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountDetailExists(accountDetail.IdAccountDt))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Nếu ModelState không hợp lệ, vẫn cần lấy lại danh sách tài khoản
            ViewData["IdAccount"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", accountDetail.IdAccount);
            return View(accountDetail);
        }

        private bool AccountDetailExists(int id)
        {
            return accountDetailRepository.GetAccountDetailById(id) != null;
        }


        [HttpPost]
        public async Task<JsonResult> DeleteId(int id)
        {
            try
            {
                var accountDetail = await accountDetailRepository.GetAccountDetailById(id);
                if (accountDetail == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bản ghi" });
                }

                await accountDetailRepository.Delete(id);
                SetAlert(Commons.Contants.Delete_success, Commons.Contants.success);
                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ChangeActive(int id)
        {
            var result = await accountDetailRepository.ChangeActive(id);
            return Json(new { status = result });
        }
    }
}
