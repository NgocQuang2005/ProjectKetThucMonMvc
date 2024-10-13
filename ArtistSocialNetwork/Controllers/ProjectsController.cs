using Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArtistSocialNetwork.Controllers
{
    public class ProjectsController : BaseController
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IDocumentInfoRepository _documentInfoRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IProjectParticipantRepository _projectParticipantRepository;

        public ProjectsController(
            ILogger<BaseController> logger,
            ApplicationDbContext context,
            IProjectRepository projectRepository,
            IAccountRepository accountRepository,
            IDocumentInfoRepository documentInfoRepository,
            IRoleRepository roleRepository,
            IProjectParticipantRepository projectParticipantRepository)
            : base(logger, context)
        {
            _projectRepository = projectRepository;
            _accountRepository = accountRepository;
            _documentInfoRepository = documentInfoRepository;
            _roleRepository = roleRepository;
            _projectParticipantRepository = projectParticipantRepository;
        }

        // Lấy danh sách dự án với bộ lọc, tìm kiếm và phân trang
        [HttpGet]
        public async Task<IActionResult> Index(string filter = "all", string searchTerm = "", int page = 1, int pageSize = 5)
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.Filter = filter;
            ViewBag.SearchTerm = searchTerm;

            IEnumerable<Project> projects;

            if (currentUserId != null)
            {
                var user = await _accountRepository.GetAccountById(currentUserId.Value);
                if (user != null && user.AccountRole != null)
                {
                    var userRole = await _roleRepository.GetRoleById(user.AccountRole.IdRole);
                    ViewBag.UserRoleId = (userRole.IdRole == 1 || userRole.IdRole == 3) ? userRole.IdRole : 0;
                    ViewBag.UserName = user?.AccountDetail?.Fullname ?? "Không xác định";
                    var userAvatar = await _documentInfoRepository.GetDocumentInfoByAccountId(currentUserId.Value);
                    ViewBag.UserAvatar = userAvatar?.UrlDocument ?? "default-profile.png";
                }
            }
            else
            {
                ViewBag.UserRoleId = 0;
                ViewBag.UserName = "Không xác định";
                ViewBag.UserAvatar = "default-profile.png";
            }

            // Lấy tất cả dự án từ repository
            var allProjects = await _projectRepository.GetProjectAll();

            // Lọc dự án của tôi
            projects = filter == "mine" && currentUserId != null
                ? allProjects.Where(p => p.IdAc == currentUserId.Value).ToList()
                : allProjects;

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                projects = projects.Where(p =>
                    (!string.IsNullOrEmpty(p.Title) && p.Title.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(p.Description) && p.Description.ToLower().Contains(searchTerm))
                ).ToList();
            }

            // Phân trang
            int totalProjects = projects.Count();
            int totalPages = (int)Math.Ceiling(totalProjects / (double)pageSize);
            projects = projects
                .OrderByDescending(p => p.LastUpdateWhen ?? p.CreatedWhen)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.ProjectList = await CreateProjectViewModelList(projects);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.UsersList = await GetAllUsers();

            await GetUsersByRoles();

            return View();
        }

        // Tạo ViewModel cho danh sách dự án
        private async Task<List<dynamic>> CreateProjectViewModelList(IEnumerable<Project> projects)
        {
            var projectViewModelList = new List<dynamic>();
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

            foreach (var projectItem in projects)
            {
                var images = await _documentInfoRepository.GetDocumentInfoByProjectId(projectItem.IdProject);
                var uploaderAvatarInfo = await _documentInfoRepository.GetDocumentInfoByAccountId(projectItem.IdAc);

                projectViewModelList.Add(new
                {
                    Project = projectItem,
                    IdAc = projectItem.IdAc,
                    Images = images.Where(img => !string.IsNullOrEmpty(img.UrlDocument))
                                   .Select(img => Url.Content("~/Upload/Images/" + img.UrlDocument))
                                   .ToList(),
                    UploaderName = projectItem.Account?.AccountDetail?.Fullname ?? "Không xác định",
                    UploaderAvatar = uploaderAvatarInfo?.UrlDocument ?? "default-profile.png",
                    TimeAgo = projectItem.LastUpdateWhen.HasValue ? CalculateTimeAgo(projectItem.LastUpdateWhen.Value) : "Không xác định"
                });
            }

            return projectViewModelList;
        }

        // Tính thời gian đã trôi qua từ lúc dự án được cập nhật
        private string CalculateTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;
            if (timeSpan.TotalMinutes < 1)
                return "Vừa xong";
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Title,Description,StartDate,EndDate")] Project projectItem, List<IFormFile> ImageFiles, List<int> Participants)
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            if (currentUserId == null)
            {
                ModelState.AddModelError("", "Không thể xác định người dùng hiện tại.");
                await GetProjectList();
                return View(projectItem);
            }

            if (ModelState.IsValid)
            {
                projectItem.IdAc = currentUserId.Value;
                projectItem.CreatedBy = currentUserId.Value;
                projectItem.LastUpdateBy = currentUserId.Value;
                projectItem.CreatedWhen = DateTime.Now;
                projectItem.LastUpdateWhen = DateTime.Now;
                projectItem.Active = true;

                await _projectRepository.Add(projectItem);

                // Xử lý thêm hình ảnh mới
                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    foreach (var file in ImageFiles)
                    {
                        if (file.Length > 0)
                        {
                            // Tạo đường dẫn lưu file
                            var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                            var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Images");

                            if (!Directory.Exists(uploadFolderPath))
                            {
                                Directory.CreateDirectory(uploadFolderPath);
                            }

                            var filePath = Path.Combine(uploadFolderPath, fileName);

                            // Lưu file lên server
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Lưu thông tin hình ảnh vào cơ sở dữ liệu
                            var documentInfo = new DocumentInfo
                            {
                                IdProject = projectItem.IdProject,
                                UrlDocument = fileName,
                                Created_by = currentUserId.Value,
                                Created_when = DateTime.Now,
                                Active = true
                            };

                            await _documentInfoRepository.Add(documentInfo);
                        }
                    }
                }

                // Xử lý người tham gia (lọc người tạo dự án ra khỏi danh sách)
                if (Participants != null && Participants.Any())
                {
                    var validParticipants = Participants.Where(p => p != currentUserId.Value).ToList(); // Lọc người tạo ra
                    foreach (var participantId in validParticipants)
                    {
                        var participant = new ProjectParticipant
                        {
                            IdProject = projectItem.IdProject,
                            IdAc = participantId,
                            Active = true,
                        };
                        await _projectParticipantRepository.Add(participant);
                    }
                }

                await GetProjectList();
                return RedirectToAction(nameof(Index));
            }

            await GetProjectList();
            return View(projectItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProject([Bind("IdProject,Title,Description,StartDate,EndDate")] Project projectItem, List<IFormFile> ImageFiles, string DeletedImages, List<int> Participants)
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            if (currentUserId == null)
            {
                return Json(new { success = false, message = "Không thể xác định người dùng hiện tại." });
            }

            // Lấy dự án từ cơ sở dữ liệu
            var existingProject = await _projectRepository.GetProjectById(projectItem.IdProject);
            if (existingProject == null)
            {
                return Json(new { success = false, message = "Dự án không tồn tại." });
            }

            // Cập nhật các thông tin cơ bản
            existingProject.Title = projectItem.Title;
            existingProject.Description = projectItem.Description;
            existingProject.StartDate = projectItem.StartDate;
            existingProject.EndDate = projectItem.EndDate;
            existingProject.LastUpdateBy = currentUserId.Value;
            existingProject.LastUpdateWhen = DateTime.Now;

            // Cập nhật dự án trong cơ sở dữ liệu
            try
            {
                _context.Entry(existingProject).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi cập nhật dự án: " + ex.Message });
            }

            // Xử lý xóa hình ảnh nếu có
            if (!string.IsNullOrEmpty(DeletedImages))
            {
                var deletedImageIds = DeletedImages.Split(',').Select(int.Parse).ToList();
                foreach (var imageId in deletedImageIds)
                {
                    var documentInfo = await _documentInfoRepository.GetDocumentInfoById(imageId);
                    if (documentInfo != null)
                    {
                        _context.Entry(documentInfo).State = EntityState.Deleted;
                        await _documentInfoRepository.Delete(imageId);
                    }
                }
            }

            // Xử lý thêm hình ảnh mới
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
                            IdProject = existingProject.IdProject,
                            UrlDocument = fileName,
                            Created_by = currentUserId.Value,
                            Created_when = DateTime.Now,
                            Active = true
                        };

                        await _documentInfoRepository.Add(documentInfo);
                    }
                }
            }

            // Lấy danh sách người tham gia hiện tại của dự án
            var currentParticipants = await _projectParticipantRepository.GetProjectParticipantsByProjectId(existingProject.IdProject);
            var currentParticipantIds = currentParticipants.Select(p => p.IdAc).ToList();

            // Xử lý những người cần xóa (người tham gia cũ nhưng không có trong danh sách mới)
            var participantsToRemove = currentParticipantIds.Except(Participants).ToList();
            foreach (var participantId in participantsToRemove)
            {
                await _projectParticipantRepository.DeleteByProjectAndAccount(existingProject.IdProject, participantId);
            }

            // Xử lý những người cần thêm mới (người tham gia mới có trong danh sách mới nhưng chưa có trong dự án)
            var participantsToAdd = Participants.Except(currentParticipantIds).ToList();
            foreach (var participantId in participantsToAdd)
            {
                var newParticipant = new ProjectParticipant
                {
                    IdProject = existingProject.IdProject,
                    IdAc = participantId,
                    Active = true,
                };
                await _projectParticipantRepository.Add(newParticipant);
            }

            await GetProjectList();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<JsonResult> DeleteProject(int id)
        {
            try
            {
                // Lấy thông tin dự án từ cơ sở dữ liệu và không theo dõi để tránh xung đột
                var project = await _context.Projects
                    .Include(p => p.DocumentInfos)
                    .Include(p => p.Account)
                    .ThenInclude(a => a.AccountDetail)
                    .AsNoTracking() // Không theo dõi thực thể để tránh xung đột trong DbContext
                    .FirstOrDefaultAsync(p => p.IdProject == id);

                if (project == null)
                {
                    return Json(new { success = false, message = "Dự án không tồn tại." });
                }

                // Tách thực thể 'Project' nếu đã được theo dõi
                var trackedProject = _context.Projects.Local.FirstOrDefault(p => p.IdProject == id);
                if (trackedProject != null)
                {
                    _context.Entry(trackedProject).State = EntityState.Detached;
                }

                // Tách thực thể 'AccountDetail' nếu đã được theo dõi
                if (project.Account?.AccountDetail != null)
                {
                    var trackedAccountDetail = _context.AccountDetails.Local
                        .FirstOrDefault(ad => ad.IdAccountDt == project.Account.AccountDetail.IdAccountDt);

                    if (trackedAccountDetail != null)
                    {
                        _context.Entry(trackedAccountDetail).State = EntityState.Detached;
                    }
                }

                // Xóa tất cả những người tham gia dự án trước
                var participants = await _context.ProjectParticipants
                    .Where(pp => pp.IdProject == id)
                    .ToListAsync();

                if (participants.Any())
                {
                    _context.ProjectParticipants.RemoveRange(participants);
                }

                // Xóa các DocumentInfos liên quan trước
                if (project.DocumentInfos != null && project.DocumentInfos.Any())
                {
                    _context.DocumentInfos.RemoveRange(project.DocumentInfos);
                }

                // Xóa dự án
                _context.Projects.Remove(project);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                // Ghi log chi tiết lỗi cơ sở dữ liệu
                Console.WriteLine($"Lỗi cơ sở dữ liệu khi xóa dự án: {dbEx.Message}");
                return Json(new { success = false, message = "Lỗi cơ sở dữ liệu khi xóa dự án." });
            }
            catch (Exception ex)
            {
                // Ghi log chi tiết lỗi khác
                Console.WriteLine($"Lỗi không xác định: {ex.Message}");
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa dự án." });
            }
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
                    Name = user.AccountDetail?.Fullname ?? "Không xác định",
                    DateOfBirth = user.AccountDetail?.Birthday,
                    Country = user.AccountDetail?.Nationality ?? "Không xác định",
                    ProfileImage = user.DocumentInfos.FirstOrDefault()?.UrlDocument ?? "default-profile.png"
                })
                .ToList();

            ViewBag.UsersWithRoles = usersWithRoles;
        }

        // Lấy danh sách dự án
        private async Task GetProjectList()
        {
            var projects = await _projectRepository.GetProjectAll();
            projects = projects.OrderByDescending(p => p.LastUpdateWhen ?? p.CreatedWhen).ToList();
            ViewBag.ProjectList = await CreateProjectViewModelList(projects);
        }
        [HttpGet]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var projectItem = await _projectRepository.GetProjectById(id);
            if (projectItem == null)
            {
                return NotFound();  // Trả về 404 nếu không tìm thấy dự án
            }

            // Lấy danh sách người tham gia hiện tại của dự án
            var participants = await _projectParticipantRepository.GetProjectParticipantsByProjectId(id);
            var participantIds = participants.Select(p => p.IdAc).ToList(); // Lấy danh sách ID người tham gia

            // Lấy danh sách hình ảnh của dự án
            var images = await _documentInfoRepository.GetDocumentInfoByProjectId(projectItem.IdProject);
            var imageUrls = images.Select(img => new
            {
                IdDcIf = img.IdDcIf,  // ID của hình ảnh
                UrlDocument = Url.Content("~/Upload/Images/" + img.UrlDocument)  // Đường dẫn URL của hình ảnh
            }).ToList();

            // Truyền dữ liệu về View
            return Json(new
            {
                idProject = projectItem.IdProject,
                title = projectItem.Title,
                description = projectItem.Description,
                startDate = projectItem.StartDate,
                endDate = projectItem.EndDate,
                images = imageUrls,  // Danh sách hình ảnh trả về
                participants = participantIds // Danh sách người tham gia hiện tại
            });
        }

        private async Task<List<dynamic>> GetAllUsers()
        {
            var users = await _accountRepository.GetAccountAll();
            var usersList = users
                .Select(user => new
                {
                    IdAccount = user.IdAccount,
                    FullName = user.AccountDetail?.Fullname ?? "Không xác định",
                })
                .ToList();

            // Cast the list of anonymous types to dynamic
            return usersList.Cast<dynamic>().ToList();
        }


    }
}
