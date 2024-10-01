using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Business;
using Repository;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class ProjectParticipantsController : BaseController
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IProjectParticipantRepository _projectParticipantRepository;

        public ProjectParticipantsController(IProjectRepository projectRepository, IAccountRepository accountRepository, IProjectParticipantRepository projectParticipantRepository)
        {
            _projectRepository = projectRepository;
            _accountRepository = accountRepository;
            _projectParticipantRepository = projectParticipantRepository;
        }

        // GET: Admin/ProjectParticipants
        public async Task<IActionResult> Index(string searchString, int? page, int IdAccount, int IdProject)
        {
            var projectParticipants = await _projectParticipantRepository.GetProjectParticipantAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                projectParticipants = projectParticipants.Where(pp =>
                    pp.Project.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    pp.Account.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (IdAccount != 0)
            {
                projectParticipants = projectParticipants.Where(a => a.IdAc == IdAccount).ToList();
            }
            if (IdProject != 0)
            {
                projectParticipants = projectParticipants.Where(taw => taw.IdProject == IdProject).ToList();
            }

            var projects = await _projectRepository.GetProjectAll();
            var accounts = await _accountRepository.GetAccountAll();

            ViewBag.ProjectTitle = projects.ToDictionary(p => p.IdProject, p => p.Title);
            ViewBag.AccountEmails = accounts.ToDictionary(a => a.IdAccount, a => a.Email);

            // Truyền dữ liệu cho dropdown lists
            ViewBag.IdAccount = new SelectList(accounts, "IdAccount", "Email");
            ViewBag.IdProject = new SelectList(projects, "IdProject", "Title");

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var pagedProjectParticipants = projectParticipants.ToPagedList(pageNumber, pageSize);

            return View(pagedProjectParticipants);
        }


        // GET: Admin/ProjectParticipants/Create
        public async Task<IActionResult> Create()
        {
            var accounts = await _accountRepository.GetAccountAll();
            var projects = await _projectRepository.GetProjectAll();

            // Tạo danh sách chọn cho dự án và tài khoản
            ViewData["IdAc"] = new SelectList(accounts, "IdAccount", "Email");
            ViewData["IdProject"] = new SelectList(projects, "IdProject", "Title");

            return View();
        }

        // POST: Admin/ProjectParticipants/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProjectParticipant,Active,IdProject,IdAc")] ProjectParticipant projectParticipant)
        {
            Console.WriteLine($"IdProject: {projectParticipant.IdProject}, IdAc: {projectParticipant.IdAc}");

            if (ModelState.IsValid)
            {
                try
                {
                    await _projectParticipantRepository.Add(projectParticipant);
                    SetAlert("Thêm mới người tham gia dự án thành công.", "success");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi thêm mới ProjectParticipant: " + ex.Message);
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi thêm mới người tham gia dự án.");
                }
            }

            // Log errors if ModelState is not valid
            if (!ModelState.IsValid)
            {
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var value = ModelState[modelStateKey];
                    foreach (var error in value.Errors)
                    {
                        Console.WriteLine($"Error in {modelStateKey}: {error.ErrorMessage}");
                    }
                }
            }

            // Reload the select lists in case of validation failure
            ViewData["IdAc"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email", projectParticipant.IdAc);
            ViewData["IdProject"] = new SelectList(await _projectRepository.GetProjectAll(), "IdProject", "Title", projectParticipant.IdProject);
            return View(projectParticipant);
        }

        // GET: Admin/ProjectParticipants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectParticipant = await _projectParticipantRepository.GetProjectParticipantById(id.Value);
            if (projectParticipant == null)
            {
                return NotFound();
            }

            ViewData["IdAc"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email", projectParticipant.IdAc);
            ViewData["IdProject"] = new SelectList(await _projectRepository.GetProjectAll(), "IdProject", "Description", projectParticipant.IdProject);
            return View(projectParticipant);
        }

        // POST: Admin/ProjectParticipants/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProjectParticipant,Active,IdProject,IdAc")] ProjectParticipant projectParticipant)
        {
            if (id != projectParticipant.IdProjectParticipant)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _projectParticipantRepository.Update(projectParticipant);
                    SetAlert("Project participant updated successfully.", "success");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProjectParticipantExists(projectParticipant.IdProjectParticipant))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdAc"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email", projectParticipant.IdAc);
            ViewData["IdProject"] = new SelectList(await _projectRepository.GetProjectAll(), "IdProject", "Description", projectParticipant.IdProject);
            return View(projectParticipant);
        }

        // POST: Admin/ProjectParticipants/DeleteId/5
        [HttpPost]
        public async Task<JsonResult> DeleteId(int id)
        {
            try
            {
                var projectParticipant = await _projectParticipantRepository.GetProjectParticipantById(id);
                if (projectParticipant == null)
                {
                    return Json(new { success = false, message = "Project participant not found." });
                }

                await _projectParticipantRepository.Delete(id);
                SetAlert("Project participant deleted successfully.", "success");
                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Admin/ProjectParticipants/ChangeActive/5
        [HttpPost]
        public async Task<JsonResult> ChangeActive(int id)
        {
            var result = await _projectParticipantRepository.ChangeActive(id);
            return Json(new { status = result });
        }

        private async Task<bool> ProjectParticipantExists(int id)
        {
            return await _projectParticipantRepository.GetProjectParticipantById(id) != null;
        }
    }
}
