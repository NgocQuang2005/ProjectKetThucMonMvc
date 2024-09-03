using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Business;
using Microsoft.AspNetCore.Authorization;
using Repository;
using X.PagedList;
using System.Data;

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
                accountDetails = accountDetails.Where(c => Commons.Library.ConvertToUnSign(c.Fullname.ToLower()).Contains(Commons.Library.ConvertToUnSign(searchString.ToLower())));
            }

            ViewBag.Page = 5;
            return View(accountDetails.ToPagedList(page ?? 1, (int)ViewBag.Page));
        }

        // GET: Admin/AccountDetails/Details/5
        

        // GET: Admin/AccountDetails/Create
        public IActionResult Create()
        {
           return View();
        }

        // POST: Admin/AccountDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAccountDt,Active,Fullname,IdAccount,CCCD,Description,Birthday,Nationality,Gender,Address,CreatedBy,CreatedWhen,LastUpdateBy,LastUpdateWhen")] AccountDetail accountDetail)
        {
            if (ModelState.IsValid)
            {
                await accountDetailRepository.Add(accountDetail);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }
            return View(accountDetail);
        }

        // GET: Admin/AccountDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var accountDetail = await accountDetailRepository.GetAccountDetailById(Convert.ToInt32(id));
            if (accountDetail == null)
            {
                return NotFound();
            }
            return View(accountDetail);
        }

        // POST: Admin/AccountDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAccountDt,Active,Fullname,IdAccount,CCCD,Description,Birthday,Nationality,Gender,Address,CreatedBy,CreatedWhen,LastUpdateBy,LastUpdateWhen")] AccountDetail accountDetail)
        {
            if (id != accountDetail.IdAccountDt)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await accountDetailRepository.Update(accountDetail);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }
            return View(accountDetail);
        }

        // GET: Admin/AccountDetails/Delete/5
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
                return Json(new
                {
                    status = true
                });
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
            return Json(new
            {
                status = result
            });
        }
    }
}
