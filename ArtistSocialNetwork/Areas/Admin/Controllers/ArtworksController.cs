using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http; // Để sử dụng Session
using Business;
using Microsoft.AspNetCore.Authorization;
using Repository;
using X.PagedList;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class ArtworksController : BaseController
    {
        private readonly IArtworkRepository _artworkRepository;
        private readonly IDocumentInfoRepository _documentInfoRepository;
        private readonly ITypeOfArtworkRepository _typeOfArtworkRepository;
        private readonly IAccountRepository _accountRepository;

        public ArtworksController(
            IArtworkRepository artworkRepository,
            IDocumentInfoRepository documentInfoRepository,
            ITypeOfArtworkRepository typeOfArtworkRepository,
            IAccountRepository accountRepository)
        {
            _artworkRepository = artworkRepository;
            _documentInfoRepository = documentInfoRepository;
            _typeOfArtworkRepository = typeOfArtworkRepository;
            _accountRepository = accountRepository;
        }

        // GET: Admin/Artworks
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            var artworks = await _artworkRepository.GetArtworkAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                artworks = artworks.Where(c => c.Title.Contains(searchString)).ToList();
            }

            ViewBag.Page = 5;
            return View(artworks.ToPagedList(page ?? 1, (int)ViewBag.Page)); // Trả về đối tượng IPagedList cho View
        }

        // GET: Admin/Artworks/Create
        public async Task<IActionResult> Create()
        {
            ViewData["IdAccount"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email");
            ViewData["IdTypeOfArtwork"] = new SelectList(await _typeOfArtworkRepository.GetTypeOfArtworkAll(), "IdTypeOfArtwork", "NameTypeOfArtwork");

            return View();
        }

        // POST: Admin/Artworks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdArtwork,Active,IdAc,IdTypeOfArtwork,Title,Description,Tags,MediaType,MediaUrl,Watched")] Artwork artwork, List<IFormFile> ImageFiles)
        {
            if (ModelState.IsValid)
            {
                // Lấy ID người dùng hiện tại từ Session
                var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

                if (currentUserId == null)
                {
                    ModelState.AddModelError("", "Không thể xác định người dùng hiện tại.");
                    return View(artwork);
                }

                // Gán giá trị cho CreatedBy và LastUpdateBy
                artwork.CreatedBy = currentUserId.Value;
                artwork.LastUpdateBy = currentUserId.Value;
                artwork.CreatedWhen = DateTime.Now;
                artwork.LastUpdateWhen = DateTime.Now;

                await _artworkRepository.Add(artwork);

                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    foreach (var file in ImageFiles)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Images");

                        if (!Directory.Exists(uploadFolderPath))
                        {
                            Directory.CreateDirectory(uploadFolderPath);
                        }

                        var filePath = Path.Combine(uploadFolderPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var documentInfo = new DocumentInfo
                        {
                            IdArtwork = artwork.IdArtwork,
                            UrlDocument = fileName,
                            Created_by = currentUserId.Value,
                            Created_when = DateTime.Now,
                            Active = true
                        };

                        await _documentInfoRepository.Add(documentInfo);
                    }
                }

                SetAlert(Commons.Contants.success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdAccount"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email", artwork.IdAc);
            ViewData["IdTypeOfArtwork"] = new SelectList(await _typeOfArtworkRepository.GetTypeOfArtworkAll(), "IdTypeOfArtwork", "NameTypeOfArtwork", artwork.IdTypeOfArtwork);

            return View(artwork);
        }

        // GET: Admin/Artworks/Edit/5
        // GET: Admin/Artworks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artwork = await _artworkRepository.GetArtworkById(id.Value);
            if (artwork == null)
            {
                return NotFound();
            }

            // Lấy các DocumentInfos liên quan đến Artwork
            artwork.DocumentInfos = (ICollection<DocumentInfo>?)await _documentInfoRepository.GetDocumentInfosByArtworkId(id.Value);

            ViewData["IdAccount"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email", artwork.IdAc);
            ViewData["IdTypeOfArtwork"] = new SelectList(await _typeOfArtworkRepository.GetTypeOfArtworkAll(), "IdTypeOfArtwork", "NameTypeOfArtwork", artwork.IdTypeOfArtwork);

            return View(artwork);
        }


        // POST: Admin/Artworks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdArtwork,Active,IdAc,IdTypeOfArtwork,Title,Description,Tags,MediaType,MediaUrl,Watched")] Artwork artwork, List<IFormFile> ImageFiles)
        {
            if (id != artwork.IdArtwork)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy ID người dùng hiện tại từ Session
                    var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

                    if (currentUserId == null)
                    {
                        ModelState.AddModelError("", "Không thể xác định người dùng hiện tại.");
                        return View(artwork);
                    }

                    // Cập nhật LastUpdateBy
                    artwork.LastUpdateBy = currentUserId.Value;
                    artwork.LastUpdateWhen = DateTime.Now;

                    await _artworkRepository.Update(artwork);

                    if (ImageFiles != null && ImageFiles.Count > 0)
                    {
                        foreach (var file in ImageFiles)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Images");

                            if (!Directory.Exists(uploadFolderPath))
                            {
                                Directory.CreateDirectory(uploadFolderPath);
                            }

                            var filePath = Path.Combine(uploadFolderPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var documentInfo = new DocumentInfo
                            {
                                IdArtwork = artwork.IdArtwork,
                                UrlDocument = fileName,
                                Created_by = currentUserId.Value,
                                Created_when = DateTime.Now,
                                Active = true
                            };

                            await _documentInfoRepository.Add(documentInfo);
                        }
                    }

                    SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    if (await _artworkRepository.GetArtworkById(artwork.IdArtwork) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["IdAccount"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email", artwork.IdAc);
            ViewData["IdTypeOfArtwork"] = new SelectList(await _typeOfArtworkRepository.GetTypeOfArtworkAll(), "IdTypeOfArtwork", "NameTypeOfArtwork", artwork.IdTypeOfArtwork);

            return View(artwork);
        }

        // POST: Admin/Artworks/Delete/5
        [HttpPost]
        public async Task<JsonResult> DeleteId(int id)
        {
            try
            {
                var artwork = await _artworkRepository.GetArtworkById(id);
                if (artwork == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bản ghi" });
                }

                await _artworkRepository.Delete(id);
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
            var result = await _typeOfArtworkRepository.ChangeActive(id);
            return Json(new { status = result });
        }
    }
}
