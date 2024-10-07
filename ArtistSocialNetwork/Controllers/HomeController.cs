using ArtistSocialNetwork.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Business;
using Repository;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace ArtistSocialNetwork.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IArtworkRepository _artworkRepository;
        private readonly IDocumentInfoRepository _documentInfoRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ITypeOfArtworkRepository _typeOfArtworkRepository;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context,
            IArtworkRepository artworkRepository,
            IDocumentInfoRepository documentInfoRepository,
            IAccountRepository accountRepository,
            IEventRepository eventRepository,
            IProjectRepository projectRepository,
            ITypeOfArtworkRepository typeOfArtworkRepository)
            : base(logger, context)
        {
            _artworkRepository = artworkRepository;
            _documentInfoRepository = documentInfoRepository;
            _accountRepository = accountRepository;
            _eventRepository = eventRepository;
            _projectRepository = projectRepository;
            _typeOfArtworkRepository = typeOfArtworkRepository;
        }

        public async Task<IActionResult> Index(int? typeId)
        {
            await GetUsersByRoles();
            await GetArtworkTypes();

            // Chỉ gọi GetArtworksByType nếu có typeId, nếu không thì gọi GetArtworkList
            if (typeId.HasValue)
            {
                await GetArtworksByType(typeId);
            }
            else
            {
                await GetArtworkList();
            }

            await GetLatestArtwork();
            await GetLatestEvent();
            await GetEventList();
            await GetLatestProject();
            await GetProjectList();

            ViewBag.SelectedTypeId = typeId;
            return View();
        }


        private async Task GetUsersByRoles()
        {
            var accounts = await _accountRepository.GetAccountAll();

            // Lọc tài khoản với vai trò là "nghệ sĩ" hoặc "admin"
            var usersWithRoles = accounts
                .Where(a => a.AccountRole != null &&
                            (a.AccountRole.RoleName.Equals("Nghệ Sỹ", StringComparison.OrdinalIgnoreCase) ||
                             a.AccountRole.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
                .Select(user => new 
                {
                    Name = user.AccountDetail?.Fullname ?? "Unknown",
                    DateOfBirth = user.AccountDetail?.Birthday,
                    Country = user.AccountDetail?.Nationality ?? "Unknown",
                    ProfileImage = user.DocumentInfos.FirstOrDefault()?.UrlDocument ?? "default-profile.png"
                })
                .ToList();
            foreach (var user in accounts)
            {
                _logger.LogInformation($"Account ID: {user.IdAccount}, AccountDetail Exists: {user.AccountDetail != null}, Fullname: {user.AccountDetail?.Fullname} , Birthday: {user.AccountDetail?.Birthday} , Nationality: {user.AccountDetail?.Nationality}");
            }
            ViewBag.UsersWithRoles = usersWithRoles;

            // Debug: Ghi log số lượng người dùng tìm thấy
            _logger.LogInformation($"Number of users with roles 'nghệ sĩ' or 'admin': {usersWithRoles.Count}");
            foreach (var user in usersWithRoles)
            {
                _logger.LogInformation($"User: {user.Name}, DOB: {user.DateOfBirth}, Country: {user.Country}");
            }
        }


        private async Task GetArtworkTypes()
        {
            var artworkTypes = await _typeOfArtworkRepository.GetTypeOfArtworkAll();
            ViewBag.ArtworkTypes = artworkTypes;
        }

        private async Task GetArtworksByType(int? typeId)
        {
            var artworks = await _artworkRepository.GetArtworkAll(); // Lấy tất cả các tác phẩm
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

            _logger.LogInformation($"Received typeId: {typeId}");

            if (typeId.HasValue)
            {
                // Thực hiện lọc theo typeId
                artworks = artworks.Where(a => a.IdTypeOfArtwork == typeId.Value).ToList();
                _logger.LogInformation($"Number of artworks after filtering: {artworks.Count()}");
            }
            else
            {
                _logger.LogInformation("No typeId provided, skipping filtering.");
            }

            // Chuyển đổi danh sách tác phẩm sau khi lọc
            var artworkList = artworks.OrderByDescending(a => a.LastUpdateWhen)
                                      .Take(5)
                                      .Select(artwork => new
                                      {
                                          Artwork = artwork,
                                          ImageUrl = _documentInfoRepository.GetDocumentInfosByArtworkId(artwork.IdArtwork).Result.FirstOrDefault()?.UrlDocument ?? "default_image.jpg",
                                          UploaderName = artwork.Account?.IdAccount == currentUserId ? "bạn" : artwork.Account?.AccountDetail?.Fullname ?? "Unknown",
                                          ArtworkType = artwork.TypeOfArtwork?.NameTypeOfArtwork ?? "Unknown",
                                          TimeAgo = artwork.LastUpdateWhen.HasValue ? CalculateTimeAgo(artwork.LastUpdateWhen.Value) : "Unknown time"
                                      }).ToList();

            // Ghi log số lượng tác phẩm sau khi lọc
            _logger.LogInformation($"Artwork list count to display: {artworkList.Count}");

            // Cập nhật ViewBag.ArtworkList để hiển thị trong View
            ViewBag.ArtworkList = artworkList;
        }



        private async Task GetLatestArtwork()
        {
            var latestArtwork = (await _artworkRepository.GetArtworkAll())
                                .OrderByDescending(a => a.IdArtwork)
                                .FirstOrDefault();

            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

            if (latestArtwork != null)
            {
                var artworkImage = (await _documentInfoRepository.GetDocumentInfosByArtworkId(latestArtwork.IdArtwork))
                                   .FirstOrDefault()?.UrlDocument ?? "default_image.jpg";
                var uploaderName = latestArtwork.Account?.IdAccount == currentUserId ? "bạn" : latestArtwork.Account?.AccountDetail?.Fullname ?? "Unknown";
                var artworkType = latestArtwork.TypeOfArtwork?.NameTypeOfArtwork ?? "Unknown";
                var timeAgo = latestArtwork.LastUpdateWhen.HasValue ? CalculateTimeAgo(latestArtwork.LastUpdateWhen.Value) : "Unknown time";

                ViewBag.Artwork = latestArtwork;
                ViewBag.ArtworkImageUrl = artworkImage;
                ViewBag.UploaderName = uploaderName;
                ViewBag.ArtworkType = artworkType;
                ViewBag.TimeAgo = timeAgo;
            }
            else
            {
                ViewBag.ArtworkImageUrl = "default_image.jpg";
                ViewBag.UploaderName = "Unknown";
                ViewBag.ArtworkType = "Unknown";
                ViewBag.TimeAgo = "Unknown time";
                ViewBag.Artwork = new { Title = "Untitled Artwork", Description = "No description available." };
            }
        }

        private async Task GetArtworkList()
        {
            var artworks = (await _artworkRepository.GetArtworkAll())
                           .OrderByDescending(a => a.LastUpdateWhen)
                           .Take(5)
                           .ToList();

            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

            var artworkList = artworks.Select(artwork => new
            {
                Artwork = artwork,
                ImageUrl = _documentInfoRepository.GetDocumentInfosByArtworkId(artwork.IdArtwork).Result.FirstOrDefault()?.UrlDocument ?? "default_image.jpg",
                UploaderName = artwork.Account?.IdAccount == currentUserId ? "bạn" : artwork.Account?.AccountDetail?.Fullname ?? "Unknown",
                ArtworkType = artwork.TypeOfArtwork?.NameTypeOfArtwork ?? "Unknown",
                TimeAgo = artwork.LastUpdateWhen.HasValue ? CalculateTimeAgo(artwork.LastUpdateWhen.Value) : "Unknown time"
            }).ToList();

            ViewBag.ArtworkList = artworkList;
        }

        private async Task GetLatestEvent()
        {
            var latestEvent = (await _eventRepository.GetEventAll())
                              .OrderByDescending(e => e.LastUpdateWhen)
                              .FirstOrDefault();

            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

            if (latestEvent != null)
            {
                var eventImage = (await _documentInfoRepository.GetDocumentInfoByEventId(latestEvent.IdEvent))
                                 .FirstOrDefault()?.UrlDocument ?? "default_image.jpg";
                var uploaderName = latestEvent.Account?.IdAccount == currentUserId ? "bạn" : latestEvent.Account?.AccountDetail?.Fullname ?? "Unknown";
                var timeAgo = latestEvent.LastUpdateWhen.HasValue ? CalculateTimeAgo(latestEvent.LastUpdateWhen.Value) : "Unknown time";

                ViewBag.LatestEvent = latestEvent;
                ViewBag.EventImageUrl = eventImage;
                ViewBag.EventUploaderName = uploaderName;
                ViewBag.EventTimeAgo = timeAgo;
            }
            else
            {
                ViewBag.EventImageUrl = "default_image.jpg";
                ViewBag.EventUploaderName = "Unknown";
                ViewBag.EventTimeAgo = "Unknown time";
                ViewBag.LatestEvent = new { Title = "Untitled Event", Description = "No description available." };
            }
        }

        private async Task GetEventList()
        {
            var events = (await _eventRepository.GetEventAll())
                         .OrderByDescending(e => e.LastUpdateWhen)
                         .Take(5)
                         .ToList();

            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

            var eventList = events.Select(e => new
            {
                Event = e,
                ImageUrl = _documentInfoRepository.GetDocumentInfoByEventId(e.IdEvent).Result.FirstOrDefault()?.UrlDocument ?? "default_image.jpg",
                UploaderName = e.Account?.IdAccount == currentUserId ? "bạn" : e.Account?.AccountDetail?.Fullname ?? "Unknown",
                TimeAgo = e.LastUpdateWhen.HasValue ? CalculateTimeAgo(e.LastUpdateWhen.Value) : "Unknown time"
            }).ToList();

            ViewBag.EventList = eventList;
        }

        private async Task GetLatestProject()
        {
            var latestProject = (await _projectRepository.GetProjectAll())
                                .OrderByDescending(p => p.LastUpdateWhen)
                                .FirstOrDefault();

            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

            if (latestProject != null)
            {
                var projectImage = (await _documentInfoRepository.GetDocumentInfoByProjectId(latestProject.IdProject))
                                   .FirstOrDefault()?.UrlDocument ?? "default_image.jpg";
                var uploaderName = latestProject.Account?.IdAccount == currentUserId ? "bạn" : latestProject.Account?.AccountDetail?.Fullname ?? "Unknown";
                var timeAgo = latestProject.LastUpdateWhen.HasValue ? CalculateTimeAgo(latestProject.LastUpdateWhen.Value) : "Unknown time";

                ViewBag.LatestProject = latestProject;
                ViewBag.ProjectImageUrl = projectImage;
                ViewBag.ProjectUploaderName = uploaderName;
                ViewBag.ProjectTimeAgo = timeAgo;
            }
            else
            {
                ViewBag.ProjectImageUrl = "default_image.jpg";
                ViewBag.ProjectUploaderName = "Unknown";
                ViewBag.ProjectTimeAgo = "Unknown time";
                ViewBag.LatestProject = new { Title = "Untitled Project", Description = "No description available." };
            }
        }

        private async Task GetProjectList()
        {
            var projects = (await _projectRepository.GetProjectAll())
                           .OrderByDescending(p => p.LastUpdateWhen)
                           .Take(5)
                           .ToList();

            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

            var projectList = projects.Select(p => new
            {
                Project = p,
                ImageUrl = _documentInfoRepository.GetDocumentInfoByProjectId(p.IdProject).Result.FirstOrDefault()?.UrlDocument ?? "default_image.jpg",
                UploaderName = p.Account?.IdAccount == currentUserId ? "bạn" : p.Account?.AccountDetail?.Fullname ?? "Unknown",
                TimeAgo = p.LastUpdateWhen.HasValue ? CalculateTimeAgo(p.LastUpdateWhen.Value) : "Unknown time"
            }).ToList();

            ViewBag.ProjectList = projectList;
        }

        private string CalculateTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "Bây giờ";
            else if (timeSpan.TotalMinutes < 60)
                return $"{timeSpan.Minutes} phút trước";
            else if (timeSpan.TotalHours < 24)
                return $"{timeSpan.Hours} giờ trước";
            else if (timeSpan.TotalDays < 7)
                return $"{timeSpan.Days} ngày trước";
            else if (timeSpan.TotalDays < 30)
            {
                int weeks = (int)(timeSpan.TotalDays / 7);
                return $"{weeks} tuần trước";
            }
            else if (timeSpan.TotalDays < 365)
            {
                int months = (int)(timeSpan.TotalDays / 30);
                return $"{months} tháng trước";
            }
            else
            {
                int years = (int)(timeSpan.TotalDays / 365);
                return $"{years} năm trước";
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
