using ArtistSocialNetwork.Models;
using Business;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ArtistSocialNetwork.Controllers
{
    public class ArtistDetailsController : BaseController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IArtworkRepository _artworkRepository;
        private readonly IAccountDetailRepository _accountDetailRepository;
        private readonly IDocumentInfoRepository _documentInfoRepository;
        private readonly IFollowRepository _followRepository;

        public ArtistDetailsController(
            ILogger<BaseController> logger,
            ApplicationDbContext context,
            IAccountRepository accountRepository,
            IArtworkRepository artworkRepository,
            IAccountDetailRepository accountDetailRepository,
            IDocumentInfoRepository documentInfoRepository,
            IFollowRepository followRepository)
            : base(logger, context)
        {
            _accountRepository = accountRepository;
            _artworkRepository = artworkRepository;
            _accountDetailRepository = accountDetailRepository;
            _documentInfoRepository = documentInfoRepository;
            _followRepository = followRepository;
        }

        // Method to calculate time ago from the artwork's creation date
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

        // Lấy thông tin chi tiết nghệ sĩ và các tác phẩm của nghệ sĩ theo IdAccount
        public async Task<IActionResult> Index(int id)
        {
            var artist = await _accountRepository.GetAccountById(id);

            if (artist == null)
            {
                return NotFound("Không tìm thấy nghệ sĩ.");
            }

            // Lấy danh sách các tác phẩm của nghệ sĩ
            var artworks = await _artworkRepository.GetArtworkAll();
            var artistArtworks = artworks.Where(a => a.IdAc == artist.IdAccount).ToList();

            // Truy xuất hình ảnh của nghệ sĩ từ DocumentInfo
            var artistImages = await _documentInfoRepository.GetDocumentInfoByAccountId(id);
            ViewBag.ArtistImage = artistImages?.UrlDocument ?? "default-avatar.png";  // Nếu không có ảnh thì dùng ảnh mặc định

            // Truy xuất hình ảnh và thời gian đăng tác phẩm của nghệ sĩ
            var artworkImageUrls = artistArtworks
                .Select(a => new
                {
                    ArtworkId = a.IdArtwork,
                    Title = a.Title,
                    ImageUrl = a.DocumentInfos.FirstOrDefault()?.UrlDocument ?? "default-artwork.png",
                    CreatedWhen = CalculateTimeAgo(a.CreatedWhen ?? DateTime.Now)  // Sử dụng hàm tính thời gian đăng
                }).ToList();

            ViewBag.ArtworkImages = artworkImageUrls;

            // Kiểm tra trạng thái theo dõi của người dùng hiện tại
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            if (currentUserId != null)
            {
                var follow = await _followRepository.GetFollowAll();
                var userFollow = follow.FirstOrDefault(f => f.IdFollower == currentUserId && f.IdFollowing == id);
                ViewBag.IsFollowing = userFollow?.Active ?? false;  // Nếu có bản ghi và Active = true thì đang theo dõi
                ViewBag.FollowExists = userFollow != null;          // Nếu có bản ghi thì true, ngược lại thì false
            }

            return View(artist);  // Trả về view hiển thị thông tin nghệ sĩ và tác phẩm
        }

        [HttpPost]
        public async Task<IActionResult> Follow(int id)
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            if (currentUserId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var follow = await _followRepository.GetFollowByUsers(currentUserId.Value, id);

            if (follow == null)
            {
                // Nếu chưa theo dõi, tạo mới một đối tượng Follow
                var newFollow = new Follow
                {
                    IdFollower = currentUserId.Value,
                    IdFollowing = id,
                    Active = true,
                    CreatedWhen = DateTime.Now,
                    LastUpdateWhen = DateTime.Now
                };

                await _followRepository.Add(newFollow);
            }
            else
            {
                // Đã tồn tại đối tượng Follow, chuyển trạng thái Active
                follow.Active = !follow.Active;
                follow.LastUpdateWhen = DateTime.Now;

                await _followRepository.Update(follow);  // Cập nhật trạng thái
            }

            return RedirectToAction("Index", new { id });
        }

    }
}
