using Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repository;
using System.Collections.Generic;
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

        public PersonalController(ILogger<BaseController> logger, ApplicationDbContext context,
            IAccountRepository accountRepository, IDocumentInfoRepository documentInfoRepository,
            IArtworkRepository artworkRepository, IEventRepository eventRepository, IProjectRepository projectRepository,
            IReactionRepository reactionRepository)
            : base(logger, context)
        {
            _accountRepository = accountRepository;
            _documentInfoRepository = documentInfoRepository;
            _artworkRepository = artworkRepository;
            _eventRepository = eventRepository;
            _projectRepository = projectRepository;
            _reactionRepository = reactionRepository;
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
                    var profileImage = (await _documentInfoRepository.GetDocumentInfoByAccountId(currentUserId.Value))?.UrlDocument ?? "default-profile.png";
                    var fullName = user.AccountDetail?.Fullname ?? "Unknown";
                    var followersCount = user.Followers?.Count() ?? 0;
                    var followingCount = user.Following?.Count() ?? 0;

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
                    ArtworkId = artwork.IdArtwork, // Thêm ArtworkId để dùng trong view
                    Title = artwork.Title,
                    Content = artwork.Description,
                    Images = images,
                    Timestamp = artwork.LastUpdateWhen,
                    IsArtwork = true, // Mark as artwork for reaction section
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
                    ArtworkId = 0, // Thêm giá trị mặc định cho ArtworkId
                    Title = evnt.Title,
                    Content = evnt.Description,
                    Images = images,
                    Timestamp = evnt.LastUpdateWhen,
                    IsArtwork = false, // Mark as not artwork to exclude reaction section
                    LikeCount = 0, // Add default value for LikeCount
                    IsLikedByCurrentUser = false, // Add default value for IsLikedByCurrentUser
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
                    ArtworkId = 0, // Thêm giá trị mặc định cho ArtworkId
                    Title = project.Title,
                    Content = project.Description,
                    Images = images,
                    Timestamp = project.LastUpdateWhen,
                    IsArtwork = false, // Mark as not artwork to exclude reaction section
                    LikeCount = 0, // Add default value for LikeCount
                    IsLikedByCurrentUser = false, // Add default value for IsLikedByCurrentUser
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
    }
}
