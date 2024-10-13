using ArtistSocialNetwork.Models;
using Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ArtistSocialNetwork.Controllers
{
    public class EventsController : BaseController
    {
        private readonly IEventRepository _eventRepository;
        private readonly IDocumentInfoRepository _documentInfoRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IEventParticipantRepository _eventParticipantRepository;

        public EventsController(
            ILogger<BaseController> logger,
            ApplicationDbContext context,
            IEventRepository eventRepository,
            IDocumentInfoRepository documentInfoRepository,
            IAccountRepository accountRepository,
            IRoleRepository roleRepository,
            IEventParticipantRepository eventParticipantRepository)
            : base(logger, context)
        {
            _eventRepository = eventRepository;
            _documentInfoRepository = documentInfoRepository;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _eventParticipantRepository = eventParticipantRepository;
        }

        // Lấy danh sách sự kiện với bộ lọc
        [HttpGet]
        public async Task<IActionResult> Index(string filter = "all", string searchTerm = "", int page = 1, int pageSize = 5)
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.Filter = filter;
            ViewBag.SearchTerm = searchTerm;

            IEnumerable<Event> events;

            // Nếu người dùng hiện tại tồn tại trong session
            if (currentUserId != null)
            {
                var user = await _accountRepository.GetAccountById(currentUserId.Value);
                if (user != null && user.AccountRole != null)
                {
                    var userRole = await _roleRepository.GetRoleById(user.AccountRole.IdRole);
                    ViewBag.UserRoleId = (userRole.IdRole == 1 || userRole.IdRole == 3) ? userRole.IdRole : 0;
                    ViewBag.UserName = user?.AccountDetail?.Fullname ?? "Unknown";
                    var userAvatar = await _documentInfoRepository.GetDocumentInfoByAccountId(currentUserId.Value);
                    ViewBag.UserAvatar = userAvatar?.UrlDocument ?? "default-profile.png";
                }
            }
            else
            {
                ViewBag.UserRoleId = 0;
                ViewBag.UserName = "Unknown";
                ViewBag.UserAvatar = "default-profile.png";
            }

            // Chờ kết quả từ GetEventAll() trước khi áp dụng Where
            var allEvents = await _eventRepository.GetEventAll();

            // Lọc sự kiện theo người dùng hiện tại nếu bộ lọc là "mine"
            events = filter == "mine" && currentUserId != null
                ? allEvents.Where(e => e.IdAc == currentUserId.Value).ToList()
                : allEvents;

            // Lọc theo từ khóa tìm kiếm (tìm kiếm trong tiêu đề và mô tả)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower(); // Chuyển từ khóa tìm kiếm về chữ thường để tìm kiếm không phân biệt hoa/thường
                events = events.Where(e =>
                    (!string.IsNullOrEmpty(e.Title) && e.Title.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(e.Description) && e.Description.ToLower().Contains(searchTerm))
                ).ToList();
            }

            // Sắp xếp các sự kiện theo thứ tự cập nhật mới nhất hoặc tạo mới
            events = events.OrderByDescending(e => e.LastUpdateWhen ?? e.CreatedWhen).ToList();

            // Phân trang
            int totalEvents = events.Count();
            int totalPages = (int)Math.Ceiling(totalEvents / (double)pageSize);

            // Lấy dữ liệu trang hiện tại
            events = events.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Tạo danh sách EventViewModel và lưu vào ViewBag
            ViewBag.EventList = await CreateEventViewModelList(events);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            // Lấy danh sách người dùng có vai trò nghệ sĩ hoặc admin
            await GetUsersByRoles();

            return View();
        }

        // Lấy thông tin sự kiện và trả về JSON
        [HttpGet]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventItem = await _eventRepository.GetEventById(id);
            if (eventItem == null)
            {
                return NotFound();
            }

            var images = await _documentInfoRepository.GetDocumentInfoByEventId(eventItem.IdEvent);
            var imageUrls = images.Select(img => new
            {
                IdDcIf = img.IdDcIf,
                UrlDocument = Url.Content("~/Upload/Images/" + img.UrlDocument)
            }).ToList();

            return Json(new
            {
                idEvent = eventItem.IdEvent,
                title = eventItem.Title,
                description = eventItem.Description,
                startDate = eventItem.StartDate,
                endDate = eventItem.EndDate,
                numberOfPeople = eventItem.NumberOfPeople,
                images = imageUrls
            });
        }

        // Tạo danh sách view model cho sự kiện
        private async Task<List<dynamic>> CreateEventViewModelList(IEnumerable<Event> events)
        {
            var eventViewModelList = new List<dynamic>();
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

            foreach (var eventItem in events)
            {
                var images = await _documentInfoRepository.GetDocumentInfoByEventId(eventItem.IdEvent);
                var uploaderAvatarInfo = await _documentInfoRepository.GetDocumentInfoByAccountId(eventItem.IdAc);

                eventViewModelList.Add(new
                {
                    Event = eventItem,
                    IdAc = eventItem.IdAc,
                    Images = images.Where(img => !string.IsNullOrEmpty(img.UrlDocument))
                                   .Select(img => Url.Content("~/Upload/Images/" + img.UrlDocument))
                                   .ToList(),
                    UploaderName = eventItem.Account?.AccountDetail?.Fullname ?? "Unknown",
                    UploaderAvatar = uploaderAvatarInfo?.UrlDocument ?? "default-profile.png",
                    TimeAgo = eventItem.LastUpdateWhen.HasValue ? CalculateTimeAgo(eventItem.LastUpdateWhen.Value) : "Unknown time"
                });
            }

            return eventViewModelList;
        }

        // Tính thời gian đã trôi qua kể từ thời điểm sự kiện được cập nhật
        private string CalculateTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;
            if (timeSpan.TotalMinutes < 1)
                return "Bây giờ";
            if (timeSpan.TotalMinutes < 60)
                return $"{timeSpan.Minutes} phút trước";
            if (timeSpan.TotalHours < 24)
                return $"{timeSpan.Hours} giờ trước";
            if (timeSpan.TotalDays < 7)
                return $"{timeSpan.Days} ngày trước";
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)} tuần trước";
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} tháng trước";
            return $"{(int)(timeSpan.TotalDays / 365)} năm trước";
        }

        // Xử lý thêm sự kiện mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Title,Description,StartDate,EndDate,NumberOfPeople")] Event eventItem, List<IFormFile> ImageFiles)
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            if (currentUserId == null)
            {
                ModelState.AddModelError("", "Cannot identify the current user.");
                await GetEventList();
                return View(eventItem);
            }

            if (ModelState.IsValid)
            {
                eventItem.IdAc = currentUserId.Value;
                eventItem.CreatedBy = currentUserId.Value;
                eventItem.LastUpdateBy = currentUserId.Value;
                eventItem.CreatedWhen = DateTime.Now;
                eventItem.LastUpdateWhen = DateTime.Now;
                eventItem.Active = true;

                await _eventRepository.Add(eventItem);

                // Xử lý tải ảnh lên
                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    foreach (var file in ImageFiles)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                            var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Images");

                            if (!Directory.Exists(uploadFolderPath))
                            {
                                Directory.CreateDirectory(uploadFolderPath);
                            }

                            var filePath = Path.Combine(uploadFolderPath, fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var documentInfo = new DocumentInfo
                            {
                                IdEvent = eventItem.IdEvent,
                                UrlDocument = fileName,
                                Created_by = currentUserId.Value,
                                Created_when = DateTime.Now,
                                Active = true
                            };

                            await _documentInfoRepository.Add(documentInfo);
                        }
                    }
                }

                await GetEventList();
                return RedirectToAction(nameof(Index));
            }

            await GetEventList();
            return View(eventItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent([Bind("IdEvent,Title,Description,StartDate,EndDate,NumberOfPeople")] Event eventItem, List<IFormFile> ImageFiles)
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            if (currentUserId == null)
            {
                return Json(new { success = false, message = "Cannot identify the current user." });
            }

            // Lấy sự kiện hiện tại từ cơ sở dữ liệu, sử dụng AsNoTracking để tránh theo dõi trùng lặp
            var existingEvent = await _eventRepository.GetEventById(eventItem.IdEvent);
            if (existingEvent == null)
            {
                return Json(new { success = false, message = "Event not found." });
            }

            // Cập nhật các trường dữ liệu của sự kiện
            existingEvent.Title = eventItem.Title;
            existingEvent.Description = eventItem.Description;
            existingEvent.StartDate = eventItem.StartDate;
            existingEvent.EndDate = eventItem.EndDate;
            existingEvent.NumberOfPeople = eventItem.NumberOfPeople;

            // Kiểm tra và gán giá trị hợp lệ cho LastUpdateBy
            existingEvent.LastUpdateBy = currentUserId.Value;
            existingEvent.LastUpdateWhen = DateTime.Now;

            // Cập nhật sự kiện vào cơ sở dữ liệu
            try
            {
                // Đặt trạng thái của thực thể là Modified để báo EF theo dõi cập nhật
                _context.Entry(existingEvent).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating event: " + ex.Message });
            }

            // Xử lý ảnh bị xóa
            var deletedImages = Request.Form["DeletedImages"].ToString().Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
            foreach (var imageId in deletedImages)
            {
                var documentInfo = await _documentInfoRepository.GetDocumentInfoById(imageId);

                if (documentInfo != null)
                {
                    _context.Entry(documentInfo).State = EntityState.Deleted; // Đánh dấu thực thể để xóa
                    await _documentInfoRepository.Delete(imageId);
                }
            }

            // Xử lý ảnh được tải lên (đảm bảo mỗi ảnh chỉ được lưu một lần)
            if (ImageFiles != null && ImageFiles.Count > 0)
            {
                foreach (var file in ImageFiles)
                {
                    if (file.Length > 0)
                    {
                        // Tạo tên file duy nhất và đường dẫn để lưu ảnh
                        var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                        var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Images");

                        if (!Directory.Exists(uploadFolderPath))
                        {
                            Directory.CreateDirectory(uploadFolderPath);
                        }

                        var filePath = Path.Combine(uploadFolderPath, fileName);

                        // Lưu tệp ảnh vào đường dẫn
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Tạo đối tượng DocumentInfo mới để lưu vào cơ sở dữ liệu
                        var documentInfo = new DocumentInfo
                        {
                            IdEvent = existingEvent.IdEvent, // Lưu IdEvent từ existingEvent để đảm bảo đúng sự kiện
                            UrlDocument = fileName,
                            Created_by = currentUserId.Value,
                            Created_when = DateTime.Now,
                            Active = true
                        };

                        // Lưu ảnh vào cơ sở dữ liệu (chỉ thêm một lần)
                        await _documentInfoRepository.Add(documentInfo);
                    }
                }
            }

            // Lấy danh sách sự kiện để hiển thị lại
            await GetEventList();
            return Json(new { success = true });
        }

        // Xử lý đăng ký sự kiện
        [HttpPost]
        public async Task<IActionResult> RegisterEvent(int IdEvent)
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            if (currentUserId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để đăng ký sự kiện." });
            }

            var eventItem = await _eventRepository.GetEventById(IdEvent);
            if (eventItem == null)
            {
                return Json(new { success = false, message = "Sự kiện không tồn tại." });
            }

            var existingRegistration = await _eventParticipantRepository.GetEventParticipantsByEventAndUser(IdEvent, currentUserId.Value);
            if (existingRegistration != null)
            {
                return Json(new { success = false, message = "Bạn đã đăng ký sự kiện này rồi." });
            }

            var eventParticipants = new EventParticipants
            {
                IdAc = currentUserId.Value,
                IdEvent = IdEvent,
                RegistrationTime = DateTime.Now,
                Active = true
            };

            try
            {
                await _eventParticipantRepository.Add(eventParticipants);
                return Json(new { success = true, message = "Đăng ký thành công!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi trong quá trình đăng ký. Vui lòng thử lại sau." });
            }
        }

        // Xóa sự kiện
        [HttpPost]
        public async Task<JsonResult> DeleteEvent(int id)
        {
            try
            {
                var eventItem = await _eventRepository.GetEventById(id);
                if (eventItem == null)
                {
                    return Json(new { success = false, message = "Event not found." });
                }

                var images = await _documentInfoRepository.GetDocumentInfoByEventId(id);
                foreach (var image in images)
                {
                    await _documentInfoRepository.Delete(image.IdDcIf);
                }

                await _eventRepository.Delete(id);
                await GetEventList();
                return Json(new { success = true });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "Database error occurred while deleting the event." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error occurred while deleting the event." });
            }
        }

        // Lấy danh sách sự kiện
        private async Task GetEventList()
        {
            var events = await _eventRepository.GetEventAll();
            events = events.OrderByDescending(e => e.LastUpdateWhen ?? e.CreatedWhen).ToList();
            ViewBag.EventList = await CreateEventViewModelList(events);
        }

        // Lấy danh sách người dùng có vai trò nghệ sĩ hoặc admin
        private async Task GetUsersByRoles()
        {
            var accounts = await _accountRepository.GetAccountAll();
            var usersWithRoles = accounts
                .Where(a => a.AccountRole != null &&
                            (a.AccountRole.RoleName.Equals("Nghệ Sỹ", StringComparison.OrdinalIgnoreCase) ||
                             a.AccountRole.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
                .Select(user => new
                {
                    IdAccount = user.IdAccount,
                    Name = user.AccountDetail?.Fullname ?? "Unknown",
                    DateOfBirth = user.AccountDetail?.Birthday,
                    Country = user.AccountDetail?.Nationality ?? "Unknown",
                    ProfileImage = user.DocumentInfos.FirstOrDefault()?.UrlDocument ?? "default-profile.png"
                })
                .ToList();

            ViewBag.UsersWithRoles = usersWithRoles;
        }
    }
}
