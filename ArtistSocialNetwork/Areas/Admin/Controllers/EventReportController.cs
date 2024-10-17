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
    public class EventReportController : BaseController
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventParticipantRepository _eventParticipantRepository;
        private const int PageSize = 10; // Số lượng sự kiện mỗi trang

        public EventReportController(IEventRepository eventRepository, IEventParticipantRepository eventParticipantRepository)
        {
            _eventRepository = eventRepository;
            _eventParticipantRepository = eventParticipantRepository;
        }

        // Hàm hiển thị trang báo cáo sự kiện
        public async Task<IActionResult> Index(int page = 1)
        {
            // Lấy tổng số sự kiện
            var totalEvents = await _eventRepository.GetTotalEvents();

            // Lấy số lượng sự kiện đang hoạt động và đã kết thúc
            var activeEvents = await _eventRepository.GetActiveEvents();
            var inactiveEvents = await _eventRepository.GetInactiveEvents();

            // Phân trang sự kiện
            var events = (await _eventRepository.GetEventAll())
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList(); // Chuyển thành danh sách để tránh lỗi truy cập song song DbContext

            // Lấy số lượng người tham gia cho mỗi sự kiện tuần tự
            var eventList = new List<dynamic>();
            foreach (var ev in events)
            {
                var participantsCount = (await _eventParticipantRepository.GetEventParticipantsAll())
                    .Count(ep => ep.IdEvent == ev.IdEvent);

                eventList.Add(new
                {
                    Event = ev,
                    ParticipantsCount = participantsCount
                });
            }

            // Tính tổng số trang
            ViewBag.TotalPages = (int)System.Math.Ceiling((double)totalEvents / PageSize);
            ViewBag.CurrentPage = page;

            // Danh sách sự kiện và người tham gia
            ViewBag.EventDetails = eventList;
            ViewBag.TotalEvents = totalEvents;
            ViewBag.TotalActiveEvents = activeEvents.Count();
            ViewBag.TotalInactiveEvents = inactiveEvents.Count();

            // Thống kê số lượng sự kiện được tạo theo thời gian
            var eventStats = await GetEventStatisticsPerDay();
            ViewBag.EventStats = eventStats;

            return View();
        }

        // Hàm hỗ trợ tính thống kê sự kiện theo ngày
        private async Task<List<dynamic>> GetEventStatisticsPerDay()
        {
            var events = await _eventRepository.GetEventAll();
            var dailyStats = events
                .Where(e => e.CreatedWhen.HasValue)
                .GroupBy(e => e.CreatedWhen.Value.Date)
                .Select(group => new
                {
                    Date = group.Key,
                    TotalEvents = group.Count()
                })
                .OrderBy(x => x.Date)
                .ToList<dynamic>();

            return dailyStats;
        }
    }
}
