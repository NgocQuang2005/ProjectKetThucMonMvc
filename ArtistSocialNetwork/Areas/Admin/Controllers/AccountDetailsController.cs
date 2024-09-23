using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Repository;
using X.PagedList;
using Microsoft.AspNetCore.Http; // Để sử dụng Session

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

        // GET: Admin/AccountDetails
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

        // GET: Admin/AccountDetails/Create
        public async Task<IActionResult> Create()
        {
            // Lấy danh sách tài khoản để gán vào SelectList cho dropdown
            ViewData["IdAccount"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email");
            return View();
        }

        // POST: Admin/AccountDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAccountDt,Active,Fullname,IdAccount,CCCD,Description,Birthday,Nationality,Gender,Address")] AccountDetail accountDetail)
        {
            if (ModelState.IsValid)
            {
                // Lấy ID người dùng hiện tại từ Session
                var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

                if (currentUserId == null)
                {
                    ModelState.AddModelError("", "Không thể xác định người dùng hiện tại.");
                    return View(accountDetail);
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

            // Nếu ModelState không hợp lệ, vẫn cần lấy lại danh sách tài khoản
            ViewData["IdAccount"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", accountDetail.IdAccount);
            return View(accountDetail);
        }

        // GET: Admin/AccountDetails/Edit/5
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

        // POST: Admin/AccountDetails/Edit/5
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

                // Cập nhật LastUpdateBy là người dùng hiện tại
                accountDetail.LastUpdateBy = currentUserId.Value;
                accountDetail.LastUpdateWhen = DateTime.Now;

                await accountDetailRepository.Update(accountDetail);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, vẫn cần lấy lại danh sách tài khoản
            ViewData["IdAccount"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", accountDetail.IdAccount);
            return View(accountDetail);
        }

        // POST: Admin/AccountDetails/Delete/5
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
