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
using X.PagedList;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class DocumentInfoController : BaseController
    {
        IDocumentInfoRepository documentInfoRepository;
        IAccountDetailRepository accountDetailRepository;
        IArtworkRepository artworkRepository;
        IProjectRepository projectRepository;
        IEventRepository eventRepository;

        public DocumentInfoController()
        {
            documentInfoRepository = new DocumentInfoRepository();
            accountDetailRepository = new AccountDetailRepository();
            artworkRepository = new ArtworkRepository();
            projectRepository = new ProjectRepository();
            eventRepository = new EventRepository();
        }

        // GET: Admin/DocumentInfo
        public async Task<IActionResult> Index( int? page , int IdAccountDetails , int IdArtWork , int IdProject , int IdEvent )
        {
            var doccument = await documentInfoRepository.GetDocumentInfoAll();
            
            if(IdAccountDetails != 0)
            {
                doccument = doccument.Where(ad => ad.IdAcDt == IdAccountDetails);
            }
            if (IdProject != 0)
            {
                doccument = doccument.Where(p => p.IdProject == IdProject);
            }
            if (IdArtWork != 0)
            {
                doccument = doccument.Where(art => art.IdArtwork == IdArtWork);
            }
            if (IdEvent != 0)
            {
                doccument = doccument.Where(e => e.IdEvent == IdEvent);
            }
            ViewBag.IdAccountDetails = new SelectList(await accountDetailRepository.GetAccountDetailAll(), "IdAccountDt", "Fullname");
            ViewBag.IdProject = new SelectList(await projectRepository.GetProjectAll(), "IdProject", "Title");
            ViewBag.IdArtWork = new SelectList(await artworkRepository.GetArtworkAll(), "IdArtWork", "Title");
            ViewBag.IdEvent = new SelectList(await eventRepository.GetEventAll(), "IdEvent", "Title");
            ViewBag.Page = 5;
            return View(doccument.ToPagedList(page ?? 1, (int)ViewBag.Page));
        }
        // GET: Admin/DocumentInfo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/DocumentInfo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDcIf,Active,IdAcDt,IdEvent,IdProject,IdArtwork,TypeFile,Path,UrlDocument,Created_by,Created_when,Last_update_by,Last_update_when")] DocumentInfo documentInfo, IFormFile UrlDocument)
        {
            if (ModelState.IsValid)
            {
                if (UrlDocument != null && UrlDocument.Length > 0)
                {
                    var fileName = Path.GetFileName(UrlDocument.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", newFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await UrlDocument.CopyToAsync(fileStream);
                    }

                    documentInfo.Path = newFileName;
                    documentInfo.TypeFile = fileExtension;
                }

                await documentInfoRepository.Add(documentInfo);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }
            return View(documentInfo);
        }
        // GET: Admin/DocumentInfo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var documentInfo = await documentInfoRepository.GetDocumentInfoById(Convert.ToInt32(id));
            if (documentInfo == null)
            {
                return NotFound();
            }
           return View(documentInfo);
        }
        // POST: Admin/DocumentInfo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDcIf,Active,IdAcDt,IdEvent,IdProject,IdArtwork,TypeFile,Path,UrlDocument,Created_by,Created_when,Last_update_by,Last_update_when")] DocumentInfo documentInfo, IFormFile UrlDocument)
        {
            if (id != documentInfo.IdDcIf)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (UrlDocument != null && UrlDocument.Length > 0)
                {
                    var fileName = Path.GetFileName(UrlDocument.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", newFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await UrlDocument.CopyToAsync(fileStream);
                    }

                    // Delete old file if it exists
                    if (!string.IsNullOrEmpty(documentInfo.Path))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", documentInfo.Path);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    documentInfo.Path = newFileName;
                    documentInfo.TypeFile = fileExtension;
                }

                await documentInfoRepository.Update(documentInfo);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }
            return View(documentInfo);
        }
        [HttpPost]
        public async Task<JsonResult> DeleteId(int id)
        {
            try
            {
                var documentInfo = await documentInfoRepository.GetDocumentInfoById(id);
                if (documentInfo == null)
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
        public async Task<JsonResult> ChangeActive(int id)
        {
            var result = await documentInfoRepository.ChangeActive(id);
            return Json(new
            {
                status = result
            });
        }
    }
}
