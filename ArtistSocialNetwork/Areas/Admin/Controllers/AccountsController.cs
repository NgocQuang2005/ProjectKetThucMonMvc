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
    public class AccountsController : BaseController
    {
        private readonly IAccountRepository accountRepository;
        private readonly IRoleRepository roleRepository;

        public AccountsController()
        {
            accountRepository = new AccountRepository();
            roleRepository = new RoleRepository();
        }

        // GET: Admin/Accounts
        public async Task<IActionResult> Index(string searchString, int? page, int IdRole)
        {
            var accounts = await accountRepository.GetAccountAll();
            if (!string.IsNullOrEmpty(searchString))
            {
                accounts = accounts.Where(c => Commons.Library.ConvertToUnSign(c.Email.ToLower()).Contains(Commons.Library.ConvertToUnSign(searchString.ToLower())));
            }
            if (IdRole != 0)
            {
                accounts = accounts.Where(u => u.IdRole == IdRole);
            }

            // Populate the SelectList for IdRole and IdAccount
            ViewData["IdRole"] = new SelectList(await roleRepository.GetRoleAll(), "IdRole", "RoleName");
            ViewData["IdAccount"] = new SelectList(accounts, "IdAccount", "Email");

            ViewBag.Page = 5;
            return View(accounts.ToPagedList(page ?? 1, (int)ViewBag.Page));
        }

        // GET: Admin/Accounts/Create
        public async Task<IActionResult> Create()
        {
            ViewData["IdRole"] = new SelectList(await roleRepository.GetRoleAll(), "IdRole", "RoleName");
            return View();
        }

        // POST: Admin/Accounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAccount,Email,Phone,Password,IdRole,CreatedBy,CreatedWhen,LastUpdateBy,LastUpdateWhen")] Account account)
        {
            ViewData["IdRole"] = new SelectList(await roleRepository.GetRoleAll(), "IdRole", "RoleName", account.IdRole);
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(account.Password))
                {
                    SetAlert(Commons.Contants.PASSWORD_FAIL, Commons.Contants.FAIL);
                    return View(account);
                }
                account.Password = Commons.Library.EncryptMD5(account.Password);
                await accountRepository.Add(account);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        // GET: Admin/Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var account = await accountRepository.GetAccountById(Convert.ToInt32(id));
            if (account == null)
            {
                return NotFound();
            }
            ViewData["IdRole"] = new SelectList(await roleRepository.GetRoleAll(), "IdRole", "RoleName", account.IdRole);
            return View(account);
        }

        // POST: Admin/Accounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAccount,Email,Phone,Password,IdRole,CreatedBy,CreatedWhen,LastUpdateBy,LastUpdateWhen")] Account account)
        {
            if (id != account.IdAccount)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(account.Password))
                {
                    var currentPassword = await accountRepository.GetAccountById(account.IdAccount);
                    account.Password = currentPassword.Password;
                }
                else
                {
                    account.Password = Commons.Library.EncryptMD5(account.Password);
                }
                await accountRepository.Update(account);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdRole"] = new SelectList(await roleRepository.GetRoleAll(), "IdRole", "RoleName", account.IdRole);
            return View(account);
        }

        // POST: Admin/Accounts/Delete/5
        public async Task<JsonResult> DeleteId(int id)
        {
            try
            {
                var account = await accountRepository.GetAccountById(id);
                if (account == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bản ghi" });
                }
                await accountRepository.Delete(id);
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
    }
}
