using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Repository;
using Microsoft.AspNetCore.Hosting;
using AutoMapper;
using ArtistSocialNetwork.Models;
using Business;
using DocumentInfo = Business.DocumentInfo;
using X.PagedList;
using Microsoft.AspNetCore.Http; // For session handling

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class DocumentInfoesController : BaseController
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IArtworkRepository _artworkRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDocumentInfoRepository _documentInfoRepository;
        private readonly IMapper _mapper;

        public DocumentInfoesController(IAccountRepository accountRepository, IArtworkRepository artworkRepository, IProjectRepository projectRepository, IDocumentInfoRepository documentInfoRepository, IEventRepository eventRepository, IWebHostEnvironment webHostEnvironment, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _artworkRepository = artworkRepository;
            _projectRepository = projectRepository;
            _eventRepository = eventRepository;
            _webHostEnvironment = webHostEnvironment;
            _documentInfoRepository = documentInfoRepository;
            _mapper = mapper;
        }

        // GET: Admin/DocumentInfoes
        public async Task<IActionResult> Index(int? page, int IdAccount = 0, int IdArtwork = 0, int IdEvent = 0, int IdProject = 0)
        {
            var documents = await _documentInfoRepository.GetDocumentInfoAll();

            if (IdAccount != 0) documents = documents.Where(u => u.IdAc == IdAccount);
            if (IdArtwork != 0) documents = documents.Where(u => u.IdArtwork == IdArtwork);
            if (IdEvent != 0) documents = documents.Where(u => u.IdEvent == IdEvent);
            if (IdProject != 0) documents = documents.Where(u => u.IdProject == IdProject);

            var documentDTOs = documents.Select(doc => new DocumentInfoDTO
            {
                IdDcIf = doc.IdDcIf,
                Active = doc.Active,
                IdAc = doc.IdAc,
                Account = doc.Account,
                IdEvent = doc.IdEvent,
                IdEventNavigation = doc.IdEventNavigation,
                IdProject = doc.IdProject,
                IdProjectNavigation = doc.IdProjectNavigation,
                IdArtwork = doc.IdArtwork,
                IdArtworkNavigation = doc.IdArtworkNavigation,
                UrlDocument = doc.UrlDocument,
                Created_by = doc.Created_by,
                Created_when = doc.Created_when,
                Last_update_by = doc.Last_update_by,
                Last_update_when = doc.Last_update_when
            }).ToList();

            var pagedDocuments = documentDTOs.ToPagedList(page ?? 1, 5); // Đặt giá trị mặc định cho page nếu null

            ViewBag.Page = 5; // Đảm bảo ViewBag.Page có giá trị

            ViewData["IdAccount"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email");
            ViewData["IdArtwork"] = new SelectList(await _artworkRepository.GetArtworkAll(), "IdArtwork", "Title");
            ViewData["IdEvent"] = new SelectList(await _eventRepository.GetEventAll(), "IdEvent", "Title");
            ViewData["IdProject"] = new SelectList(await _projectRepository.GetProjectAll(), "IdProject", "Title");

            return View(pagedDocuments);
        }

        // GET: Admin/DocumentInfoes/Create
        public async Task<IActionResult> Create()
        {
            var model = new DocumentInfoDTO
            {
                Created_when = Commons.Library.GetServerDateTime() // Default creation date
            };

            ViewData["IdAccount"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email");
            ViewData["IdArtwork"] = new SelectList(await _artworkRepository.GetArtworkAll(), "IdArtwork", "Title");
            ViewData["IdEvent"] = new SelectList(await _eventRepository.GetEventAll(), "IdEvent", "Title");
            ViewData["IdProject"] = new SelectList(await _projectRepository.GetProjectAll(), "IdProject", "Title");

            return View(model);
        }

        // POST: Admin/DocumentInfoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDcIf,Active,IdAc,IdEvent,IdProject,IdArtwork,UrlDocument,Created_by,Created_when,Last_update_by,Last_update_when,ImageFile")] DocumentInfoDTO documentInfoDTO)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the current user ID from the session
                var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

                if (currentUserId == null)
                {
                    ModelState.AddModelError("", "Unable to determine the current user.");
                    return View(documentInfoDTO);
                }

                if (documentInfoDTO.ImageFile != null)
                {
                    string uniqueFileName = UploadedFile(documentInfoDTO);
                    documentInfoDTO.UrlDocument = "/Upload/Images/" + uniqueFileName;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please upload an image file.");
                    return View(documentInfoDTO);
                }

                // Set Created_by and Last_update_by fields to the current user ID from session
                documentInfoDTO.Created_by = currentUserId.Value;
                documentInfoDTO.Created_when = DateTime.Now;

                var documentInfo = _mapper.Map<DocumentInfo>(documentInfoDTO);

                await _documentInfoRepository.Add(documentInfo);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdAccount"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email", documentInfoDTO.IdAc);
            ViewData["IdArtwork"] = new SelectList(await _artworkRepository.GetArtworkAll(), "IdArtwork", "Title", documentInfoDTO.IdArtwork);
            ViewData["IdEvent"] = new SelectList(await _eventRepository.GetEventAll(), "IdEvent", "Title", documentInfoDTO.IdEvent);
            ViewData["IdProject"] = new SelectList(await _projectRepository.GetProjectAll(), "IdProject", "Title", documentInfoDTO.IdProject);

            return View(documentInfoDTO);
        }

        // GET: Admin/DocumentInfoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _documentInfoRepository.GetDocumentInfoById(id.Value);
            if (document == null)
            {
                return NotFound();
            }

            var documentInfoDTO = _mapper.Map<DocumentInfoDTO>(document);

            ViewData["IdAccount"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email", document.IdAc);
            ViewData["IdArtwork"] = new SelectList(await _artworkRepository.GetArtworkAll(), "IdArtwork", "Title", document.IdArtwork);
            ViewData["IdEvent"] = new SelectList(await _eventRepository.GetEventAll(), "IdEvent", "Title", document.IdEvent);
            ViewData["IdProject"] = new SelectList(await _projectRepository.GetProjectAll(), "IdProject", "Title", document.IdProject);

            return View(documentInfoDTO);
        }

        // POST: Admin/DocumentInfoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDcIf,Active,IdAc,IdEvent,IdProject,IdArtwork,UrlDocument,Created_by,Created_when,Last_update_by,Last_update_when,ImageFile")] DocumentInfoDTO documentInfoDTO)
        {
            if (id != documentInfoDTO.IdDcIf)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the current user ID from the session
                    var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

                    if (currentUserId == null)
                    {
                        ModelState.AddModelError("", "Unable to determine the current user.");
                        return View(documentInfoDTO);
                    }

                    if (documentInfoDTO.ImageFile != null)
                    {
                        string uniqueFileName = UploadedFile(documentInfoDTO);
                        documentInfoDTO.UrlDocument = "/Upload/Images/" + uniqueFileName;
                    }
                    else
                    {
                        var existingDocument = await _documentInfoRepository.GetDocumentInfoById(id);
                        documentInfoDTO.UrlDocument = existingDocument?.UrlDocument;
                    }

                    // Update Last_update_by and Last_update_when fields
                    documentInfoDTO.Last_update_by = currentUserId.Value;
                    documentInfoDTO.Last_update_when = DateTime.Now;

                    var documentInfo = _mapper.Map<DocumentInfo>(documentInfoDTO);
                    await _documentInfoRepository.Update(documentInfo);
                    SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await DocumentInfoExists(documentInfoDTO.IdDcIf))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["IdAccount"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email", documentInfoDTO.IdAc);
            ViewData["IdArtwork"] = new SelectList(await _artworkRepository.GetArtworkAll(), "IdArtwork", "Title", documentInfoDTO.IdArtwork);
            ViewData["IdEvent"] = new SelectList(await _eventRepository.GetEventAll(), "IdEvent", "Title", documentInfoDTO.IdEvent);
            ViewData["IdProject"] = new SelectList(await _projectRepository.GetProjectAll(), "IdProject", "Title", documentInfoDTO.IdProject);

            return View(documentInfoDTO);
        }

        // POST: Admin/DocumentInfoes/Delete/5
        public async Task<JsonResult> DeleteId(int id)
        {
            try
            {
                var document = await _documentInfoRepository.GetDocumentInfoById(id);
                if (document == null)
                {
                    return Json(new { success = false, message = "Record not found" });
                }
                await _documentInfoRepository.Delete(id);
                SetAlert(Commons.Contants.Delete_success, Commons.Contants.success);
                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ChangeActive(int id)
        {
            var result = await _documentInfoRepository.ChangeActive(id);
            return Json(new { status = result });
        }

        private string UploadedFile(DocumentInfoDTO documentInfoDTO)
        {
            string uniqueFileName = null;

            if (documentInfoDTO.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Upload/Images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + documentInfoDTO.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    documentInfoDTO.ImageFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        private async Task<bool> DocumentInfoExists(int id)
        {
            return await _documentInfoRepository.GetDocumentInfoById(id) != null;
        }
    }
}
