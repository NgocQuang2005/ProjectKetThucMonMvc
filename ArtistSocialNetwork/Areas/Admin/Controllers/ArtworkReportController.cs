using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System.Linq;
using System.Threading.Tasks;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class ArtworkReportController : BaseController
    {
        private readonly IArtworkRepository _artworkRepository;
        private readonly ITypeOfArtworkRepository _typeOfArtworkRepository;
        private const int PageSize = 10; // Số lượng tác phẩm mỗi trang

        public ArtworkReportController(IArtworkRepository artworkRepository, ITypeOfArtworkRepository typeOfArtworkRepository)
        {
            _artworkRepository = artworkRepository;
            _typeOfArtworkRepository = typeOfArtworkRepository;
        }

        // Hiển thị trang báo cáo với bộ lọc và phân trang
        public async Task<IActionResult> Index(int page = 1, int? typeOfArtworkId = null)
        {
            // In ra giá trị của typeOfArtworkId
            Console.WriteLine($"TypeOfArtworkId: {typeOfArtworkId}");

            // Lấy tất cả loại tác phẩm cho dropdown
            var typesOfArtwork = await _typeOfArtworkRepository.GetTypeOfArtworkAll();

            ViewBag.TypesOfArtwork = typesOfArtwork;
            ViewBag.SelectedTypeOfArtworkId = typeOfArtworkId;

            // Lọc tác phẩm theo loại, bao gồm cả loại tác phẩm và kiểm tra khóa ngoại
            var filteredArtworks = await _artworkRepository.GetArtworkAll();

            // In ra tổng số tác phẩm trước khi lọc
            Console.WriteLine($"Total Artworks Before Filter: {filteredArtworks.Count()}");

            if (typeOfArtworkId.HasValue)
            {
                filteredArtworks = filteredArtworks.Where(a => a.IdTypeOfArtwork == typeOfArtworkId).ToList();
                // In ra số lượng tác phẩm sau khi lọc
                Console.WriteLine($"Filtered Artworks Count: {filteredArtworks.Count()}");
            }

            // Tính tổng số tác phẩm sau khi lọc
            var totalFilteredArtworks = filteredArtworks.Count();
            Console.WriteLine($"Total Filtered Artworks: {totalFilteredArtworks}");

            // Tính tổng số tác phẩm (không lọc)
            var totalArtworks = await _artworkRepository.GetTotalArtwork();
            ViewBag.TotalArtworks = totalArtworks;

            // Phân trang tác phẩm
            var paginatedArtworks = filteredArtworks
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // In ra tổng số tác phẩm sau khi phân trang
            Console.WriteLine($"Paginated Artworks Count: {paginatedArtworks.Count()}");

            // Kiểm tra nếu loại tác phẩm chưa có thông tin
            foreach (var artwork in paginatedArtworks)
            {
                if (artwork.TypeOfArtwork == null && artwork.IdTypeOfArtwork.HasValue)
                {
                    artwork.TypeOfArtwork = await _typeOfArtworkRepository.GetTypeOfArtworkById(artwork.IdTypeOfArtwork.Value);
                }
            }

            // Tính tổng số trang
            ViewBag.TotalPages = (int)System.Math.Ceiling((double)totalFilteredArtworks / PageSize);
            ViewBag.CurrentPage = page;

            // Lấy thống kê theo ngày, lọc theo loại tác phẩm (nếu có)
            var artworkStats = await GetArtworkStatisticsPerDay(typeOfArtworkId);
            ViewBag.ArtworkStats = artworkStats;

            return View(paginatedArtworks);
        }
        // Thống kê số lượng tác phẩm được tạo theo thời gian và lọc theo loại tác phẩm
        private async Task<List<dynamic>> GetArtworkStatisticsPerDay(int? typeOfArtworkId)
        {
            var artworks = await _artworkRepository.GetArtworkAll();
            var filteredArtworks = artworks
                .Where(a => !typeOfArtworkId.HasValue || a.IdTypeOfArtwork == typeOfArtworkId)
                .ToList();

            var dailyStats = filteredArtworks
                .Where(a => a.CreatedWhen.HasValue)
                .GroupBy(a => a.CreatedWhen.Value.Date)
                .Select(group => new
                {
                    Date = group.Key,
                    TotalArtworks = group.Count()
                })
                .OrderBy(x => x.Date)
                .ToList<dynamic>();

            return dailyStats;
        }
    }
}
