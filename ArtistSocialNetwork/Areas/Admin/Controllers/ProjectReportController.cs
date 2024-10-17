using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class ProjectReportController : BaseController
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectParticipantRepository _projectParticipantRepository;

        public ProjectReportController(IProjectRepository projectRepository, IProjectParticipantRepository projectParticipantRepository)
        {
            _projectRepository = projectRepository;
            _projectParticipantRepository = projectParticipantRepository;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            // Lấy thống kê tổng số dự án
            var totalProjects = await _projectRepository.GetTotalProjects();
            var activeProjects = (await _projectRepository.GetActiveProjects()).Count();
            var completedProjects = (await _projectRepository.GetCompletedProjects()).Count();

            var projects = await _projectRepository.GetProjectAll();

            // Thống kê số dự án theo ngày
            var projectStats = await GetProjectStatisticsPerDay();
            ViewBag.ProjectStats = projectStats;

            var projectDetails = new List<dynamic>();
            foreach (var project in projects)
            {
                var participantCount = await _projectParticipantRepository.GetParticipantCountByProjectId(project.IdProject);
                projectDetails.Add(new
                {
                    Project = project,
                    Participants = participantCount
                });
            }

            ViewBag.TotalProjects = totalProjects;
            ViewBag.ActiveProjects = activeProjects;  // Sửa lại để đếm số lượng
            ViewBag.CompletedProjects = completedProjects;  // Sửa lại để đếm số lượng
            ViewBag.ProjectDetails = projectDetails;

            // Phân trang
            var paginatedProjects = projectDetails.Skip((page - 1) * 10).Take(10).ToList();  // 10 dự án mỗi trang
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)projectDetails.Count() / 10);  // Tổng số trang

            return View(paginatedProjects);
        }

        // Thống kê số lượng dự án theo ngày
        private async Task<List<dynamic>> GetProjectStatisticsPerDay()
        {
            var projects = await _projectRepository.GetProjectAll();
            var dailyStats = projects
                .Where(p => p.CreatedWhen.HasValue)
                .GroupBy(p => p.CreatedWhen.Value.Date)
                .Select(group => new
                {
                    Date = group.Key,
                    TotalProjects = group.Count()
                })
                .OrderBy(x => x.Date)
                .ToList<dynamic>();

            return dailyStats;
        }
    }
}
