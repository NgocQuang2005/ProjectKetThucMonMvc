using ArtistSocialNetwork.Models;
using Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repository;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ArtistSocialNetwork.Controllers
{
    public class PersonalController : BaseController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IDocumentInfoRepository _documentInfoRepository;
        private readonly IArtworkRepository _artworkRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IReactionRepository _reactionRepository;
        private readonly IFollowRepository _followRepository;

        public PersonalController(ILogger<BaseController> logger, ApplicationDbContext context,
            IAccountRepository accountRepository, IDocumentInfoRepository documentInfoRepository,
            IArtworkRepository artworkRepository, IEventRepository eventRepository, IProjectRepository projectRepository,
            IReactionRepository reactionRepository, IFollowRepository followRepository)
            : base(logger, context)
        {
            _accountRepository = accountRepository;
            _documentInfoRepository = documentInfoRepository;
            _artworkRepository = artworkRepository;
            _eventRepository = eventRepository;
            _projectRepository = projectRepository;
            _reactionRepository = reactionRepository;
            _followRepository = followRepository;
        }

        public async Task<IActionResult> Index()
        {
            await GetUserDetails();
            await GetUserPosts();
            return View();
        }

        private async Task GetUserDetails()
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

            if (currentUserId != null)
            {
                var user = await _accountRepository.GetAccountById(currentUserId.Value);
                if (user != null)
                {
                    // Truy xuất hình ảnh đại diện và thông tin cơ bản của người dùng
                    var profileImage = (await _documentInfoRepository.GetDocumentInfoByAccountId(currentUserId.Value))?.UrlDocument ?? "default-profile.png";
                    var fullName = user.AccountDetail?.Fullname ?? "Unknown";

                    // Đếm số lượng người theo dõi và đang theo dõi bằng cách sử dụng FollowRepository
                    var followers = await _followRepository.GetFollowAll();
                    var followersCount = followers.Count(f => f.IdFollowing == currentUserId.Value && f.Active);  // Đếm số người theo dõi (Followers)
                    var followingCount = followers.Count(f => f.IdFollower == currentUserId.Value && f.Active);  // Đếm số người mà người dùng đang theo dõi (Following)

                    ViewBag.ProfileImage = profileImage;
                    ViewBag.FullName = fullName;
                    ViewBag.FollowersCount = followersCount;
                    ViewBag.FollowingCount = followingCount;
                }
            }
            else
            {
                ViewBag.ProfileImage = "default-profile.png";
                ViewBag.FullName = "Người dùng không xác định";
                ViewBag.FollowersCount = 0;
                ViewBag.FollowingCount = 0;
            }
        }


        private async Task GetUserPosts()
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            if (currentUserId == null) return;

            var artworks = (await _artworkRepository.GetArtworkAll())
                .Where(a => a.IdAc == currentUserId.Value)
                .ToList();

            var events = (await _eventRepository.GetEventAll())
                .Where(e => e.IdAc == currentUserId.Value)
                .ToList();

            var projects = (await _projectRepository.GetProjectAll())
                .Where(p => p.IdAc == currentUserId.Value)
                .ToList();

            var posts = new List<dynamic>();

            // Add artworks to posts
            foreach (var artwork in artworks)
            {
                var images = (await _documentInfoRepository.GetDocumentInfosByArtworkId(artwork.IdArtwork))
                             .Select(img => img.UrlDocument)
                             .ToList();

                // Get reaction (like) information
                var reactions = await _reactionRepository.GetReactionAll();
                int likeCount = reactions.Count(r => r.IdArtwork == artwork.IdArtwork && r.Action);
                bool isLikedByCurrentUser = currentUserId.HasValue && reactions.Any(r => r.IdArtwork == artwork.IdArtwork && r.IdAc == currentUserId.Value && r.Action);

                posts.Add(new
                {
                    ArtworkId = artwork.IdArtwork,
                    Title = artwork.Title,
                    Content = artwork.Description,
                    Images = images,
                    Timestamp = artwork.LastUpdateWhen,
                    TimeAgo = CalculateTimeAgo((DateTime)artwork.LastUpdateWhen), // Add time ago
                    IsArtwork = true,
                    LikeCount = likeCount,
                    IsLikedByCurrentUser = isLikedByCurrentUser,
                    GridClass = GetImageGridClass(images.Count)
                });
            }

            // Add events to posts (No reaction)
            foreach (var evnt in events)
            {
                var images = (await _documentInfoRepository.GetDocumentInfoByEventId(evnt.IdEvent))
                             .Select(img => img.UrlDocument)
                             .ToList();

                posts.Add(new
                {
                    ArtworkId = 0,
                    Title = evnt.Title,
                    Content = evnt.Description,
                    Images = images,
                    Timestamp = evnt.LastUpdateWhen,
                    TimeAgo = CalculateTimeAgo((DateTime)evnt.LastUpdateWhen), // Add time ago
                    IsArtwork = false,
                    LikeCount = 0,
                    IsLikedByCurrentUser = false,
                    GridClass = GetImageGridClass(images.Count)
                });
            }

            // Add projects to posts (No reaction)
            foreach (var project in projects)
            {
                var images = (await _documentInfoRepository.GetDocumentInfoByProjectId(project.IdProject))
                             .Select(img => img.UrlDocument)
                             .ToList();

                posts.Add(new
                {
                    ArtworkId = 0,
                    Title = project.Title,
                    Content = project.Description,
                    Images = images,
                    Timestamp = project.LastUpdateWhen,
                    TimeAgo = CalculateTimeAgo((DateTime)project.LastUpdateWhen), // Add time ago
                    IsArtwork = false,
                    LikeCount = 0,
                    IsLikedByCurrentUser = false,
                    GridClass = GetImageGridClass(images.Count)
                });
            }

            ViewBag.UserPosts = posts.OrderByDescending(p => p.Timestamp).ToList();
        }

        // Helper method to determine grid class
        private string GetImageGridClass(int count)
        {
            switch (count)
            {
                case 1:
                    return "single";
                case 2:
                    return "double";
                case 3:
                    return "triple";
                default:
                    return "quad";
            }
        }

        // New helper method to calculate time ago
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
