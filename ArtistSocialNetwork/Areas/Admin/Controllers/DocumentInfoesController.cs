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
using AutoMapper;
using DocumentInfo = Business.DocumentInfo;
using Microsoft.AspNetCore.Hosting;

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

        public DocumentInfoesController(IAccountRepository accountRepository, IArtworkRepository artworkRepository , IProjectRepository projectRepository , IDocumentInfoRepository documentInfoRepository , IEventRepository eventRepository, IWebHostEnvironment webHostEnvironment, IMapper mapper)
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
        public async Task<IActionResult> Index( int? page, int IdAccount = 0 , int IdArtwork = 0 , int IdEvent = 0 , int IdProject = 0)
        {
            var document = await _documentInfoRepository.GetDocumentInfoAll();
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
            ViewData["IdAccount"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email");
            ViewData["IdArtwork"] = new SelectList(await _artworkRepository.GetArtworkAll(), "IdArtwork", "Title");
            ViewData["IdEvent"] = new SelectList(await _eventRepository.GetEventAll(), "IdEvent", "Title");
            ViewData["IdProject"] = new SelectList(await _projectRepository.GetProjectAll(), "IdProject", "Title");
            ViewBag.Page = 5;
            return View(document.ToPagedList(page ?? 1, (int)ViewBag.Page));
        }
        // GET: Admin/DocumentInfoes/Create
        public async Task<IActionResult> Create() 
        {
            var model = new DocumentInfo
            {
                Created_when = Commons.Library.GetServerDateTime() // Ngày tạo mặc định
            };
            ViewData["IdAccount"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email");
            ViewData["IdArtwork"] = new SelectList(await _artworkRepository.GetArtworkAll(), "IdArtwork", "Title");
            ViewData["IdEvent"] = new SelectList(await _eventRepository.GetEventAll(), "IdEvent", "Title");
            ViewData["IdProject"] = new SelectList(await _projectRepository.GetProjectAll(), "IdProject", "Title");
            return View(model);
        }

        // POST: Admin/DocumentInfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDcIf,Active,IdAc,IdEvent,IdProject,IdArtwork,TypeFile,Path,UrlDocument,Created_by,Created_when,Last_update_by,Last_update_when")] Business.DocumentInfo documentInfo)
        {
            if (ModelState.IsValid)
            {
                if (documentInfo.UrlDocument != null)
                {
                    string uniqueFileName = UploadedFile(documentInfo);
                    documentInfo.UrlDocument = uniqueFileName;
                }

                await _documentInfoRepository.Add(documentInfo);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAccount"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email", documentInfo.IdAc);
            ViewData["IdArtwork"] = new SelectList(await _artworkRepository.GetArtworkAll(), "IdArtwork", "Title", documentInfo.IdArtwork);
            ViewData["IdEvent"] = new SelectList(await _eventRepository.GetEventAll(), "IdEvent", "Title", documentInfo.IdEvent);
            ViewData["IdProject"] = new SelectList(await _projectRepository.GetProjectAll(), "IdProject", "Title", documentInfo.IdProject);

            return View(documentInfo);
        }

        // GET: Admin/DocumentInfoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var document = await _documentInfoRepository.GetDocumentInfoById(id.Value);
            if (document == null)
            {
                return NotFound();
            }
            ViewData["IdAccount"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email", document.IdAc);
            ViewData["IdArtwork"] = new SelectList(await _artworkRepository.GetArtworkAll(), "IdArtwork", "Title", document.IdArtwork);
            ViewData["IdEvent"] = new SelectList(await _eventRepository.GetEventAll(), "IdEvent", "Title", document.IdEvent);
            ViewData["IdProject"] = new SelectList(await _projectRepository.GetProjectAll(), "IdProject", "Title", document.IdProject);
            return View(document);
        }

        // POST: Admin/DocumentInfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDcIf,Active,IdAc,IdEvent,IdProject,IdArtwork,TypeFile,Path,UrlDocument,Created_by,Created_when,Last_update_by,Last_update_when")] Business.DocumentInfo documentInfo)
        {
            if(ModelState.IsValid)
            {
                if(documentInfo.ImageFile != null)
                {
                    string uniqueFileName = UploadedFile(documentInfo);
                    documentInfo.UrlDocument = uniqueFileName;
                }
                else
                {
                    // Giữ lại ImageUrl hiện có nếu không có file mới
                    var existingDocument = await _documentInfoRepository.GetDocumentInfoById(id);
                    documentInfo.UrlDocument = existingDocument?.UrlDocument;
                }

                await _documentInfoRepository.Update(documentInfo);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAccount"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email", documentInfo.IdAc);
            ViewData["IdArtwork"] = new SelectList(await _artworkRepository.GetArtworkAll(), "IdArtwork", "Title", documentInfo.IdArtwork);
            ViewData["IdEvent"] = new SelectList(await _eventRepository.GetEventAll(), "IdEvent", "Title", documentInfo.IdEvent);
            ViewData["IdProject"] = new SelectList(await _projectRepository.GetProjectAll(), "IdProject", "Title", documentInfo.IdProject);
            return View(documentInfo);
        }

        // POST: Admin/DocumentInfoes/Delete/5
        public async Task<JsonResult> DeleteId(int id)
        {
            try
            {
                var document = await _documentInfoRepository.GetDocumentInfoById(id);
                if (document == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bản ghi" });
                }
                await _documentInfoRepository.Delete(id);
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
        [HttpPost]
        public async Task<JsonResult> ChangeStatus(int id)
        {
            var result = await _documentInfoRepository.ChangeActive(id);
            return Json(new { status = result });
        }
        public string UploadedFile(DocumentInfo documentInfo)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(documentInfo.ImageFile.FileName);
            string extension = Path.GetExtension(documentInfo.ImageFile.FileName);
            documentInfo.UrlDocument = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            string path = Path.Combine(wwwRootPath + "/Upload/Images/", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                documentInfo.ImageFile.CopyTo(fileStream);
            }

            return fileName;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                var currentDate = DateTime.Now;
                var year = currentDate.Year.ToString();
                var month = currentDate.Month.ToString("D2");
                var day = currentDate.Day.ToString("D2");

                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload/images", year, month);
                Directory.CreateDirectory(directoryPath);

                var fileName = $"{year}{month}{day}_{Path.GetFileName(upload.FileName)}";
                var filePath = Path.Combine(directoryPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await upload.CopyToAsync(stream);
                }

                return Json(new { uploaded = true, url = $"/upload/images/{year}/{month}/{fileName}" });
            }

            return Json(new { uploaded = false, message = "Upload failed" });
        }
    }
}
