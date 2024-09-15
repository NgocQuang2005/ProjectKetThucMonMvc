using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Business;
using Microsoft.AspNetCore.Authorization;
using Repository;
using Microsoft.AspNetCore.Rewrite;
using X.PagedList;
using Microsoft.CodeAnalysis;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class DocumentInfoesController : BaseController
    {
        private readonly IAccountRepository accountRepository;
        private readonly IArtworkRepository artworkRepository;
        private readonly IProjectRepository projectRepository;
        private readonly IEventRepository eventRepository;
        private readonly IDocumentInfoRepository documentInfoRepository;

        public DocumentInfoesController()
        {
            accountRepository = new AccountRepository();
            artworkRepository = new ArtworkRepository();
            projectRepository = new ProjectRepository();
            eventRepository = new EventRepository();
            documentInfoRepository = new DocumentInfoRepository();
        }

        // GET: Admin/DocumentInfoes
        public async Task<IActionResult> Index(string searchString, int? page, int IdAccount , int IdArtwork , int IdEvent , int IdProject)
        {
            var document = await documentInfoRepository.GetDocumentInfoAll();
            if (IdAccount != 0)
            {
                document = document.Where(u => u.IdAc == IdAccount);
            }
            if (IdArtwork != 0)
            {
                document = document.Where(u => u.IdArtwork == IdArtwork);
            }
            if (IdEvent != 0)
            {
                document = document.Where(u => u.IdEvent == IdEvent);

            }
            if (IdProject != 0)
            {
                document = document.Where(u => u.IdProject == IdProject);
            }
            // Populate the SelectList for IdRole and IdAccount
            ViewData["IdAccount"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email");
            ViewData["IdArtwork"] = new SelectList(await artworkRepository.GetArtworkAll(), "IdArtwork", "Title");
            ViewData["IdEvent"] = new SelectList(await eventRepository.GetEventAll(), "IdEvent", "Title");
            ViewData["IdProject"] = new SelectList(await projectRepository.GetProjectAll(), "IdProject", "Title");
            ViewBag.Page = 5;
            return View(document.ToPagedList(page ?? 1, (int)ViewBag.Page));
        }
        // GET: Admin/DocumentInfoes/Create
        public async Task<IActionResult> Create() 
        {
            ViewData["IdAccount"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email");
            ViewData["IdArtwork"] = new SelectList(await artworkRepository.GetArtworkAll(), "IdArtwork", "Title");
            ViewData["IdEvent"] = new SelectList(await eventRepository.GetEventAll(), "IdEvent", "Title");
            ViewData["IdProject"] = new SelectList(await projectRepository.GetProjectAll(), "IdProject", "Title");
            return View();
        }

        // POST: Admin/DocumentInfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDcIf,Active,IdAc,IdEvent,IdProject,IdArtwork,TypeFile,Path,UrlDocument,Created_by,Created_when,Last_update_by,Last_update_when")] Business.DocumentInfo documentInfo)
        {
            ViewData["IdAccount"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email" , documentInfo.IdAc);
            ViewData["IdArtwork"] = new SelectList(await artworkRepository.GetArtworkAll(), "IdArtwork", "Title" , documentInfo.IdArtwork);
            ViewData["IdEvent"] = new SelectList(await eventRepository.GetEventAll(), "IdEvent", "Title", documentInfo.IdEvent);
            ViewData["IdProject"] = new SelectList(await projectRepository.GetProjectAll(), "IdProject", "Title" , documentInfo.IdProject);
            if (ModelState.IsValid)
            {
                await documentInfoRepository.Add(documentInfo);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }
            return View(documentInfo);
        }

        // GET: Admin/DocumentInfoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var document = await documentInfoRepository.GetDocumentInfoById(Convert.ToInt32(id));
            if (document == null)
            {
                return NotFound();
            }
            ViewData["IdAccount"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", document.IdAc);
            ViewData["IdArtwork"] = new SelectList(await artworkRepository.GetArtworkAll(), "IdArtwork", "Title", document.IdArtwork);
            ViewData["IdEvent"] = new SelectList(await eventRepository.GetEventAll(), "IdEvent", "Title", document.IdEvent);
            ViewData["IdProject"] = new SelectList(await projectRepository.GetProjectAll(), "IdProject", "Title", document.IdProject);
            return View(document);
        }

        // POST: Admin/DocumentInfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDcIf,Active,IdAc,IdEvent,IdProject,IdArtwork,TypeFile,Path,UrlDocument,Created_by,Created_when,Last_update_by,Last_update_when")] Business.DocumentInfo documentInfo)
        {
            if (id != documentInfo.IdDcIf)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await documentInfoRepository.Update(documentInfo);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAccount"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", documentInfo.IdAc);
            ViewData["IdArtwork"] = new SelectList(await artworkRepository.GetArtworkAll(), "IdArtwork", "Title", documentInfo.IdArtwork);
            ViewData["IdEvent"] = new SelectList(await eventRepository.GetEventAll(), "IdEvent", "Title", documentInfo.IdEvent);
            ViewData["IdProject"] = new SelectList(await projectRepository.GetProjectAll(), "IdProject", "Title", documentInfo.IdProject);
            return View(documentInfo);
        }

        // POST: Admin/DocumentInfoes/Delete/5
        public async Task<JsonResult> DeleteId(int id)
        {
            try
            {
                var document = await documentInfoRepository.GetDocumentInfoById(id);
                if (document == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bản ghi" });
                }
                await documentInfoRepository.Delete(id);
                SetAlert(Commons.Contants.Delete_success, Commons.Contants.success);
                return Json(new
                {
                    status = true
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
