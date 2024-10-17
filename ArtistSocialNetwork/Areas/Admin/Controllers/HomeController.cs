using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class HomeController : BaseController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IArtworkRepository _artworkRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IProjectRepository _projectRepository;
        public HomeController(
            IAccountRepository accountRepository,
            IArtworkRepository artworkRepository,
            IEventRepository eventRepository,
            IProjectRepository projectRepository)
        {
            _accountRepository = accountRepository;
            _artworkRepository = artworkRepository;
            _eventRepository = eventRepository;
            _projectRepository = projectRepository;
        }
        public async Task<IActionResult> Index()
        {
            var accountStats = await GetAccountStatisticsPerDay();
            var totalUsers = await _accountRepository.GetTotalAccounts();
            var totalArtworks = await _artworkRepository.GetTotalArtwork();
            var totalEvents = await _eventRepository.GetTotalEvents();
            var totalProjects = await _projectRepository.GetTotalProjects();

            // Chuyển dữ liệu vào ViewBag
            ViewBag.Dates = accountStats.Select(x => x.Date?.ToString("dd/MM/yyyy")).ToList();
            ViewBag.Totals = accountStats.Select(x => x.TotalRegistrations).ToList();

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalArtworks = totalArtworks;
            ViewBag.TotalEvents = totalEvents;
            ViewBag.TotalProjects = totalProjects;
            return View();
        }


        public async Task<List<dynamic>> GetAccountStatisticsPerDay()
        {
            var accounts = await _accountRepository.GetAccountAll();
            var dailyStats = accounts
                .Where(a => a.CreatedWhen.HasValue)
                .GroupBy(a => a.CreatedWhen.Value.Date)
                .Select(group => new
                {
                    Date = group.Key,
                    TotalRegistrations = group.Count()
                })
                .OrderBy(x => x.Date)
                .ToList<dynamic>(); // Chuyển sang List<dynamic> để dễ xử lý trong view

            return dailyStats;
        }

    }
}
