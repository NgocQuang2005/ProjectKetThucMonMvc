using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Business;
using Microsoft.AspNetCore.Authorization;
using Repository;
using X.PagedList;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class EventsController : BaseController
    {
        private readonly IEventRepository eventRepository;
        private readonly IAccountRepository accountRepository;
        private readonly IDocumentInfoRepository documentInfoRepository;

        public EventsController()
        {
            eventRepository = new EventRepository();
            accountRepository = new AccountRepository();
            documentInfoRepository = new DocumentInfoRepository();
        }

        // GET: Admin/Events
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            // Truy vấn sự kiện và liên kết với tài khoản để lấy email
            var events = await eventRepository.GetEventAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                events = events
                    .Where(e => Commons.Library.ConvertToUnSign(e.Title.ToLower())
                    .Contains(Commons.Library.ConvertToUnSign(searchString.ToLower())))
                    .ToList();
            }

            // Lấy danh sách tài khoản (Account) và tạo từ điển để lấy Email dựa trên IdAc
            var accountList = await accountRepository.GetAccountAll();
            var accountEmails = accountList.ToDictionary(a => a.IdAccount, a => a.Email);

            ViewBag.AccountEmails = accountEmails;  // Truyền danh sách email qua ViewBag
            ViewData["IdAc"] = new SelectList(accountList, "IdAccount", "Email");
            ViewBag.Page = 5;

            return View(events.ToPagedList(page ?? 1, (int)ViewBag.Page));
        }

        // GET: Admin/Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await eventRepository.GetEventById(id.Value);
            if (@event == null)
            {
                return NotFound();
            }

            ViewData["IdAc"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", @event.IdAc);
            ViewBag.EventImages = @event.DocumentInfos.ToList(); // Đảm bảo lấy đúng danh sách DocumentInfos

            return View(@event);
        }

        // POST: Admin/Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEvent,Active,Title,IdAc,Description,StartDate,EndDate,NumberOfPeople,CreatedBy,CreatedWhen")] Event @event, List<IFormFile> ImageFiles, string DeletedImages = "")
        {
            if (id != @event.IdEvent)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

                if (currentUserId == null)
                {
                    ModelState.AddModelError("", "Không thể xác định người dùng hiện tại.");
                    ViewData["IdAc"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", @event.IdAc);
                    return View(@event);
                }

                @event.LastUpdateBy = currentUserId.Value;
                @event.LastUpdateWhen = DateTime.Now;

                // Cập nhật sự kiện
                await eventRepository.Update(@event);

                // Xử lý ảnh bị xóa
                if (!string.IsNullOrEmpty(DeletedImages))
                {
                    var deletedImageIds = DeletedImages.Split(',');
                    foreach (var imageId in deletedImageIds)
                    {
                        var documentInfo = await documentInfoRepository.GetDocumentInfoById(int.Parse(imageId));
                        if (documentInfo != null)
                        {
                            // Xóa file khỏi thư mục
                            var filePath = Path.Combine("wwwroot/Upload/Images", documentInfo.UrlDocument);
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }

                            // Xóa thông tin ảnh khỏi cơ sở dữ liệu
                            await documentInfoRepository.Delete(documentInfo.IdDcIf);
                        }
                    }
                }

                // Xử lý thêm ảnh mới
                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    foreach (var file in ImageFiles)
                    {
                        var fileExtension = Path.GetExtension(file.FileName);
                        var newFileName = $"{Guid.NewGuid()}{fileExtension}";
                        var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Images");

                        if (!Directory.Exists(uploadFolderPath))
                        {
                            Directory.CreateDirectory(uploadFolderPath);
                        }

                        var filePath = Path.Combine(uploadFolderPath, newFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var documentInfo = new DocumentInfo
                        {
                            Active = true,
                            UrlDocument = newFileName,
                            IdEvent = @event.IdEvent,
                            Created_by = currentUserId.Value,
                            Created_when = DateTime.Now,
                            Last_update_by = currentUserId.Value,
                            Last_update_when = DateTime.Now
                        };

                        await documentInfoRepository.Add(documentInfo);
                    }
                }

                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdAc"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", @event.IdAc);
            return View(@event);
        }

        // POST: Admin/Events/Delete/5
        [HttpPost]
        public async Task<JsonResult> DeleteId(int id)
        {
            try
            {
                var @event = await eventRepository.GetEventById(id);
                if (@event == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sự kiện" });
                }

                var documentInfos = await documentInfoRepository.GetDocumentInfoByEventId(id);
                foreach (var documentInfo in documentInfos)
                {
                    var filePath = Path.Combine("wwwroot/Upload/Images", documentInfo.UrlDocument);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    await documentInfoRepository.Delete(documentInfo.IdDcIf);
                }

                await eventRepository.Delete(id);
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
            var result = await eventRepository.ChangeActive(id);
            return Json(new { status = result });
        }
    }
}
