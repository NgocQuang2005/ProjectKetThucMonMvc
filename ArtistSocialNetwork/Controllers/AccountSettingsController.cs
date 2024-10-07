using ArtistSocialNetwork.Models;
using Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Repository;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ArtistSocialNetwork.Controllers
{
    public class AccountSettingsController : BaseController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IDocumentInfoRepository _documentInfoRepository;

        public AccountSettingsController(IAccountRepository accountRepository, IDocumentInfoRepository documentInfoRepository, ILogger<AccountSettingsController> logger, ApplicationDbContext context)
            : base(logger, context)
        {
            _accountRepository = accountRepository;
            _documentInfoRepository = documentInfoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

            if (currentUserId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var account = await _accountRepository.GetAccountById(currentUserId.Value);
            if (account == null)
            {
                return NotFound();
            }

            // Map to AccountDTO
            var model = new AccountDTO
            {
                IdAccount = account.IdAccount,
                Email = account.Email,
                Phone = account.Phone,
                ProfileImageUrl = account.DocumentInfos?.FirstOrDefault()?.UrlDocument // Assuming DocumentInfos contains the profile image
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AccountDTO model, IFormFile profileImage)
        {
            if (!ModelState.IsValid)
            {
                SetAlert("Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.", "error");
                return View(model);
            }

            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            if (currentUserId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                var account = await _accountRepository.GetAccountById(currentUserId.Value);
                if (account == null)
                {
                    SetAlert("Không tìm thấy tài khoản.", "error");
                    return NotFound();
                }

                // Update account details
                account.Email = model.Email;
                account.Phone = model.Phone;
                if (!string.IsNullOrEmpty(model.Password))
                {
                    account.Password = Commons.Library.EncryptMD5(model.Password);
                }
                account.LastUpdateWhen = DateTime.Now;
                account.LastUpdateBy = currentUserId;

                // Handle image upload
                if (profileImage != null && profileImage.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(profileImage.FileName)}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "images", fileName);

                    // Save the image to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profileImage.CopyToAsync(stream);
                    }

                    // Update or create the DocumentInfo entry
                    var documentInfo = await _documentInfoRepository.GetDocumentInfoByAccountId(account.IdAccount);
                    if (documentInfo != null)
                    {
                        documentInfo.UrlDocument = $"{fileName}";
                        await _documentInfoRepository.Update(documentInfo);
                    }
                    else
                    {
                        documentInfo = new DocumentInfo
                        {
                            IdAc = account.IdAccount,
                            UrlDocument = $"{fileName}",
                            Active = true,
                            Created_when = DateTime.Now,
                            Last_update_when = DateTime.Now
                        };
                        await _documentInfoRepository.Add(documentInfo);
                    }

                    model.ProfileImageUrl = documentInfo.UrlDocument; // Update the model to display the new image
                }

                await _accountRepository.Update(account);

                // Set success alert and redirect to home page
                SetAlert("Cập nhật thông tin tài khoản thành công!", "success");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error occurred while updating account settings.");

                // Set alert with error message
                SetAlert("Đã xảy ra lỗi khi cập nhật tài khoản. Vui lòng thử lại.", "error");

                // Optionally, you can add the error message to the model to show on the view
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi cập nhật tài khoản.");
                return View(model);
            }
        }
    }
}
