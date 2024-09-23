using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Business;
using Microsoft.AspNetCore.Authorization;
using Repository;
using X.PagedList;

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
            // Lấy danh sách tài khoản từ repository
            var accounts = await accountRepository.GetAccountAll();

            // Lọc theo từ khoá tìm kiếm nếu có
            if (!string.IsNullOrEmpty(searchString))
            {
                accounts = accounts.Where(c => Commons.Library.ConvertToUnSign(c.Email.ToLower())
                                    .Contains(Commons.Library.ConvertToUnSign(searchString.ToLower()))).ToList();
            }

            // Lọc theo vai trò nếu được chọn
            if (IdRole != 0)
            {
                accounts = accounts.Where(u => u.IdRole == IdRole).ToList();
            }

            // Trả dữ liệu cho view
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
        // POST: Admin/Accounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAccount,Email,Phone,Password,IdRole,CreatedBy,CreatedWhen,LastUpdateBy,LastUpdateWhen")] Account account, IFormFile ProfileImage)
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

                // Lưu tài khoản vào database
                await accountRepository.Add(account);

                // Xử lý việc lưu ảnh nếu có
                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    var fileName = Path.GetFileName(ProfileImage.FileName);
                    var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Images");

                    // Kiểm tra và tạo thư mục nếu nó chưa tồn tại
                    if (!Directory.Exists(uploadFolderPath))
                    {
                        Directory.CreateDirectory(uploadFolderPath);
                    }

                    var filePath = Path.Combine(uploadFolderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfileImage.CopyToAsync(stream);
                    }

                    // Chỉ lưu tên file vào cơ sở dữ liệu (không bao gồm thư mục)
                    var documentInfo = new DocumentInfo
                    {
                        IdAc = account.IdAccount, // Gán IdAccount mới tạo vào DocumentInfo
                        UrlDocument = fileName,   // Chỉ lưu tên file, không lưu cả đường dẫn
                        Created_by = account.IdAccount,
                        Created_when = DateTime.Now,
                        Active = true
                    };

                    await documentInfoRepository.Add(documentInfo);
                }

                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }
            return View(account);
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

            ViewData["IdRole"] = new SelectList(await roleRepository.GetRoleAll(), "IdRole", "RoleName", account.IdRole);

            // Lấy thông tin ảnh đại diện nếu có
            var documentInfo = await documentInfoRepository.GetByAccountId(account.IdAccount);
            ViewData["ProfileImageUrl"] = documentInfo?.UrlDocument; // Lưu đường dẫn ảnh vào ViewData

            return View(account);
        }


        // POST: Admin/Accounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAccount,Email,Phone,Password,IdRole,CreatedBy,CreatedWhen,LastUpdateBy,LastUpdateWhen")] Account account, IFormFile ProfileImage)
        {
            if (id != account.IdAccount)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(account.Password))
                {
                    var currentAccount = await accountRepository.GetAccountById(account.IdAccount);
                    account.Password = currentAccount.Password;
                }
                else
                {
                    account.Password = Commons.Library.EncryptMD5(account.Password);
                }

                // Cập nhật tài khoản
                await accountRepository.Update(account);

                // Xử lý cập nhật ảnh nếu có
                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    var fileName = Path.GetFileName(ProfileImage.FileName);
                    var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Images");

                    // Kiểm tra và tạo thư mục nếu nó chưa tồn tại
                    if (!Directory.Exists(uploadFolderPath))
                    {
                        Directory.CreateDirectory(uploadFolderPath);
                    }

                    var filePath = Path.Combine(uploadFolderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfileImage.CopyToAsync(stream);
                    }

                    var documentInfo = await documentInfoRepository.GetByAccountId(account.IdAccount);
                    if (documentInfo == null)
                    {
                        // Tạo mới DocumentInfo nếu chưa có
                        documentInfo = new DocumentInfo
                        {
                            IdAc = account.IdAccount,
                            UrlDocument = fileName,   // Chỉ lưu tên file
                            Created_by = account.IdAccount,
                            Created_when = DateTime.Now,
                            Active = true
                        };
                        await documentInfoRepository.Add(documentInfo);
                    }
                    else
                    {
                        // Cập nhật DocumentInfo nếu đã có
                        documentInfo.UrlDocument = fileName;   // Chỉ lưu tên file
                        documentInfo.Last_update_by = account.IdAccount;
                        documentInfo.Last_update_when = DateTime.Now;
                        await documentInfoRepository.Update(documentInfo);
                    }
                }

                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdRole"] = new SelectList(await roleRepository.GetRoleAll(), "IdRole", "RoleName", account.IdRole);
            return View(account);
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

                // Xóa DocumentInfo nếu có
                var documentInfo = await documentInfoRepository.GetByAccountId(id);
                if (documentInfo != null)
                {
                    await documentInfoRepository.Delete(documentInfo.IdDcIf);
                }

                // Xóa Account
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
