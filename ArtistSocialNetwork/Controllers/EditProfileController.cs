using ArtistSocialNetwork.Models;
using Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Repository;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ArtistSocialNetwork.Controllers
{
    public class EditProfileController : BaseController
    {
        private readonly IAccountDetailRepository _accountDetailRepository;

        public EditProfileController(IAccountDetailRepository accountDetailRepository, ILogger<EditProfileController> logger, ApplicationDbContext context)
            : base(logger, context)
        {
            _accountDetailRepository = accountDetailRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

            if (currentUserId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var accountDetail = await _accountDetailRepository.GetAccountDetailById(currentUserId.Value);

            // Nếu chưa có thông tin chi tiết, trả về một đối tượng mới để thêm
            if (accountDetail == null)
            {
                accountDetail = new AccountDetail
                {
                    IdAccount = currentUserId.Value
                };
            }

            return View(accountDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AccountDetail model)
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
                var accountDetail = await _accountDetailRepository.GetAccountDetailById(currentUserId.Value);

                if (accountDetail == null)
                {
                    // Thêm mới thông tin chi tiết nếu chưa tồn tại
                    model.IdAccount = currentUserId.Value;
                    model.CreatedBy = currentUserId.Value;
                    model.CreatedWhen = DateTime.Now;
                    await _accountDetailRepository.Add(model);
                    SetAlert("Thêm mới thông tin cá nhân thành công!", "success");
                }
                else
                {
                    // Cập nhật thông tin chi tiết nếu đã tồn tại
                    accountDetail.Fullname = model.Fullname;
                    accountDetail.Birthday = model.Birthday;
                    accountDetail.Nationality = model.Nationality;
                    accountDetail.Gender = model.Gender;
                    accountDetail.Address = model.Address;
                    accountDetail.CCCD = model.CCCD;
                    accountDetail.Description = model.Description;
                    accountDetail.LastUpdateBy = currentUserId.Value;
                    accountDetail.LastUpdateWhen = DateTime.Now;

                    await _accountDetailRepository.Update(accountDetail);
                    SetAlert("Cập nhật thông tin cá nhân thành công!", "success");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating profile.");
                SetAlert("Đã xảy ra lỗi khi cập nhật hồ sơ cá nhân. Vui lòng thử lại.", "error");
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi cập nhật hồ sơ cá nhân.");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
