using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Business;
using Repository;
using X.PagedList;
using ArtistSocialNetwork.Models;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class AccountsController : BaseController
    {
        private readonly IAccountRepository accountRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IDocumentInfoRepository documentInfoRepository;

        public AccountsController()
        {
            accountRepository = new AccountRepository();
            roleRepository = new RoleRepository();
            documentInfoRepository = new DocumentInfoRepository();
        }

        // GET: Admin/Accounts
        public async Task<IActionResult> Index(string searchString, int? page, int IdRole)
        {
            var accounts = await accountRepository.GetAccountAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                accounts = accounts.Where(c => Commons.Library.ConvertToUnSign(c.Email.ToLower())
                                    .Contains(Commons.Library.ConvertToUnSign(searchString.ToLower()))).ToList();
            }

            if (IdRole != 0)
            {
                accounts = accounts.Where(u => u.IdRole == IdRole).ToList();
            }

            ViewData["IdRole"] = new SelectList(await roleRepository.GetRoleAll(), "IdRole", "RoleName");
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
        public async Task<IActionResult> Create([Bind("IdAccount,Email,Phone,Password,IdRole,CreatedBy,CreatedWhen,LastUpdateBy,LastUpdateWhen,ProfileImageUrl")] AccountDTO accountDTO, IFormFile ProfileImage)
        {
            ViewData["IdRole"] = new SelectList(await roleRepository.GetRoleAll(), "IdRole", "RoleName", accountDTO.IdRole);

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(accountDTO.Password))
                {
                    SetAlert(Commons.Contants.PASSWORD_FAIL, Commons.Contants.FAIL);
                    return View(accountDTO);
                }

                var account = new Account
                {
                    Email = accountDTO.Email,
                    Phone = accountDTO.Phone,
                    Password = Commons.Library.EncryptMD5(accountDTO.Password),
                    IdRole = accountDTO.IdRole,
                    CreatedBy = accountDTO.CreatedBy,
                    CreatedWhen = accountDTO.CreatedWhen,
                    LastUpdateBy = accountDTO.LastUpdateBy,
                    LastUpdateWhen = accountDTO.LastUpdateWhen
                };

                await accountRepository.Add(account);

                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    var fileName = Path.GetFileName(ProfileImage.FileName);
                    var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Images");

                    if (!Directory.Exists(uploadFolderPath))
                    {
                        Directory.CreateDirectory(uploadFolderPath);
                    }

                    var filePath = Path.Combine(uploadFolderPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfileImage.CopyToAsync(stream);
                    }

                    var documentInfo = new DocumentInfo
                    {
                        IdAc = account.IdAccount,
                        UrlDocument = fileName,
                        Created_by = account.IdAccount,
                        Created_when = DateTime.Now,
                        Active = true
                    };

                    await documentInfoRepository.Add(documentInfo);
                    accountDTO.ProfileImageUrl = fileName;
                }

                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }
            return View(accountDTO);
        }

        // GET: Admin/Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var account = await accountRepository.GetAccountById(id.Value);
            if (account == null)
            {
                return NotFound();
            }

            var accountDTO = new AccountDTO
            {
                IdAccount = account.IdAccount,
                Email = account.Email,
                Phone = account.Phone,
                IdRole = account.IdRole,
                CreatedBy = account.CreatedBy,
                CreatedWhen = account.CreatedWhen,
                LastUpdateBy = account.LastUpdateBy,
                LastUpdateWhen = account.LastUpdateWhen,
                ProfileImageUrl = await GetProfileImageUrl(account.IdAccount)
            };

            ViewData["IdRole"] = new SelectList(await roleRepository.GetRoleAll(), "IdRole", "RoleName", account.IdRole);
            return View(accountDTO);
        }

        // POST: Admin/Accounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAccount,Email,Phone,Password,IdRole,CreatedBy,CreatedWhen,LastUpdateBy,LastUpdateWhen,ProfileImageUrl")] AccountDTO accountDTO, IFormFile? ProfileImage)
        {
            if (id != accountDTO.IdAccount)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Retrieve the existing account entity from the database
                var existingAccount = await accountRepository.GetAccountById(accountDTO.IdAccount);
                if (existingAccount == null)
                {
                    return NotFound();
                }

                // Update the account properties with the values from accountDTO
                existingAccount.Email = accountDTO.Email;
                existingAccount.Phone = accountDTO.Phone;
                existingAccount.IdRole = accountDTO.IdRole;
                existingAccount.LastUpdateBy = accountDTO.LastUpdateBy;
                existingAccount.LastUpdateWhen = DateTime.Now;

                // Update password only if a new password is provided
                if (!string.IsNullOrEmpty(accountDTO.Password))
                {
                    existingAccount.Password = Commons.Library.EncryptMD5(accountDTO.Password);
                }

                // Handle profile image update
                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    var fileName = Path.GetFileName(ProfileImage.FileName);
                    var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Images");

                    if (!Directory.Exists(uploadFolderPath))
                    {
                        Directory.CreateDirectory(uploadFolderPath);
                    }

                    var filePath = Path.Combine(uploadFolderPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfileImage.CopyToAsync(stream);
                    }

                    var documentInfo = await documentInfoRepository.GetDocumentInfoByAccountId(accountDTO.IdAccount);
                    if (documentInfo == null)
                    {
                        documentInfo = new DocumentInfo
                        {
                            IdAc = accountDTO.IdAccount,
                            UrlDocument = fileName,
                            Created_by = accountDTO.IdAccount,
                            Created_when = DateTime.Now,
                            Active = true
                        };
                        await documentInfoRepository.Add(documentInfo);
                    }
                    else
                    {
                        documentInfo.UrlDocument = fileName;
                        documentInfo.Last_update_by = accountDTO.IdAccount;
                        documentInfo.Last_update_when = DateTime.Now;
                        await documentInfoRepository.Update(documentInfo);
                    }

                    // Update the ProfileImageUrl in the accountDTO
                    accountDTO.ProfileImageUrl = fileName;
                }

                // Update the account in the repository
                await accountRepository.Update(existingAccount);

                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }

            // If model state is not valid, reload roles and return to view with errors
            ViewData["IdRole"] = new SelectList(await roleRepository.GetRoleAll(), "IdRole", "RoleName", accountDTO.IdRole);
            SetAlert("Có lỗi xảy ra. Vui lòng kiểm tra lại thông tin.", Commons.Contants.FAIL);
            return View(accountDTO);
        }

        private async Task<string?> GetProfileImageUrl(int accountId)
        {
            var documentInfo = await documentInfoRepository.GetDocumentInfoByAccountId(accountId);
            return documentInfo?.UrlDocument;
        }

        public async Task<JsonResult> DeleteId(int id)
        {
            try
            {
                var account = await accountRepository.GetAccountById(id);
                if (account == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bản ghi" });
                }

                var documentInfo = await documentInfoRepository.GetDocumentInfoByAccountId(id);
                if (documentInfo != null)
                {
                    await documentInfoRepository.Delete(documentInfo.IdDcIf);
                }

                await accountRepository.Delete(id);
                SetAlert(Commons.Contants.Delete_success, Commons.Contants.success);
                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
