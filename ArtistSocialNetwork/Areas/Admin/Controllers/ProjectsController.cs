using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Business;
using Microsoft.AspNetCore.Authorization;
using Repository;
using X.PagedList;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class ProjectsController : BaseController
    {
        private readonly IProjectRepository projectRepository;
        private readonly IAccountRepository accountRepository;
        private readonly IDocumentInfoRepository documentInfoRepository;

        public ProjectsController()
        {
            projectRepository = new ProjectRepository();
            accountRepository = new AccountRepository();
            documentInfoRepository = new DocumentInfoRepository();
        }

        // GET: Admin/Projects
        public async Task<IActionResult> Index(string searchString, int? page, int IdAccount)
        {
            var projects = await projectRepository.GetProjectAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                projects = projects
                    .Where(p => Commons.Library.ConvertToUnSign(p.Title.ToLower())
                    .Contains(Commons.Library.ConvertToUnSign(searchString.ToLower())))
                    .ToList();
            }
            if (IdAccount != 0)
            {
                projects = projects.Where(a => a.IdAc == IdAccount).ToList();
            }

            var accountList = await accountRepository.GetAccountAll();
            var accountEmails = accountList.ToDictionary(a => a.IdAccount, a => a.Email);

            ViewBag.AccountEmails = accountEmails;
            ViewBag.IdAccount = new SelectList(accountList, "IdAccount", "Email"); // Sửa lại từ ViewData["IdAc"] sang ViewBag.IdAccount
            ViewBag.Page = 5;

            return View(projects.ToPagedList(page ?? 1, (int)ViewBag.Page));
        }


        // GET: Admin/Projects/Create
        public async Task<IActionResult> Create()
        {
            ViewData["IdAc"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email");
            return View();
        }

        // POST: Admin/Projects/Create
        // POST: Admin/Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProject,Active,Title,IdAc,Description,StartDate,EndDate")] Project project, List<IFormFile> ImageFiles)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

                if (currentUserId == null)
                {
                    ModelState.AddModelError("", "Cannot identify the current user.");
                    ViewData["IdAc"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", project.IdAc);
                    return View(project);
                }

                project.CreatedBy = currentUserId.Value;
                project.CreatedWhen = DateTime.Now;
                project.LastUpdateBy = currentUserId.Value;
                project.LastUpdateWhen = DateTime.Now;

                try
                {
                    // Add project to the database
                    await projectRepository.Add(project);

                    // Handle new image uploads
                    if (ImageFiles != null && ImageFiles.Count > 0)
                    {
                        foreach (var file in ImageFiles)
                        {
                            var fileExtension = Path.GetExtension(file.FileName);
                            var newFileName = $"{Guid.NewGuid()}{fileExtension}";
                            var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Images");

                            if (!Directory.Exists(uploadFolderPath))
                            {
                                Directory.CreateDirectory(uploadFolderPath);
                            }

                            var filePath = Path.Combine(uploadFolderPath, newFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var documentInfo = new DocumentInfo
                            {
                                Active = true,
                                UrlDocument = newFileName,
                                IdProject = project.IdProject,
                                Created_by = currentUserId.Value,
                                Created_when = DateTime.Now,
                                Last_update_by = currentUserId.Value,
                                Last_update_when = DateTime.Now
                            };

                            await documentInfoRepository.Add(documentInfo);
                        }
                    }

                    SetAlert(Commons.Contants.success, Commons.Contants.success);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the exception if any error occurs during the database operation
                    Console.WriteLine("Error occurred while creating the project: " + ex.Message);
                    ModelState.AddModelError("", "Error occurred while creating the project.");
                }
            }

            // If ModelState is not valid, log the errors
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is not valid. Errors:");
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var value = ModelState[modelStateKey];
                    foreach (var error in value.Errors)
                    {
                        Console.WriteLine($"Error in {modelStateKey}: {error.ErrorMessage}");
                    }
                }
            }

            ViewData["IdAc"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", project.IdAc);
            return View(project);
        }

        // GET: Admin/Projects/Edit/5
        // Modify the Edit method in ProjectsController
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await projectRepository.GetProjectById(id.Value);
            if (project == null)
            {
                return NotFound();
            }

            ViewData["IdAc"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", project.IdAc);

            // Fetch document information by project ID
            ViewBag.ProjectImages = await documentInfoRepository.GetDocumentInfoByProjectId(id.Value);

            return View(project);
        }

        // POST: Admin/Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProject,Active,Title,IdAc,Description,StartDate,EndDate,CreatedBy,CreatedWhen")] Project project, List<IFormFile> ImageFiles, string DeletedImages = "")
        {
            if (id != project.IdProject)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

                if (currentUserId == null)
                {
                    ModelState.AddModelError("", "Cannot identify the current user.");
                    ViewData["IdAc"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", project.IdAc);
                    return View(project);
                }

                // Lấy dự án từ database qua repository (dùng AsNoTracking để tránh bị theo dõi)
                var originalProject = await projectRepository.GetProjectById(id);
                if (originalProject == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin của dự án
                project.CreatedBy = originalProject.CreatedBy;
                project.CreatedWhen = originalProject.CreatedWhen;
                project.LastUpdateBy = currentUserId.Value;
                project.LastUpdateWhen = DateTime.Now;

                try
                {
                    // Cập nhật dự án
                    await projectRepository.Update(project);

                    // Xử lý hình ảnh bị xóa
                    if (!string.IsNullOrEmpty(DeletedImages))
                    {
                        var deletedImageIds = DeletedImages.Split(',');
                        foreach (var imageId in deletedImageIds)
                        {
                            var documentInfo = await documentInfoRepository.GetDocumentInfoById(int.Parse(imageId));
                            if (documentInfo != null)
                            {
                                var filePath = Path.Combine("wwwroot/Upload/Images", documentInfo.UrlDocument);
                                if (System.IO.File.Exists(filePath))
                                {
                                    System.IO.File.Delete(filePath); // Xóa tệp từ thư mục
                                }

                                // Xóa DocumentInfo khỏi cơ sở dữ liệu
                                await documentInfoRepository.Delete(documentInfo.IdDcIf);
                            }
                        }
                    }

                    // Xử lý thêm hình ảnh mới
                    if (ImageFiles != null && ImageFiles.Count > 0)
                    {
                        foreach (var file in ImageFiles)
                        {
                            var fileExtension = Path.GetExtension(file.FileName);
                            var newFileName = $"{Guid.NewGuid()}{fileExtension}";
                            var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Images");

                            if (!Directory.Exists(uploadFolderPath))
                            {
                                Directory.CreateDirectory(uploadFolderPath);
                            }

                            var filePath = Path.Combine(uploadFolderPath, newFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var documentInfo = new DocumentInfo
                            {
                                Active = true,
                                UrlDocument = newFileName,
                                IdProject = project.IdProject,
                                Created_by = currentUserId.Value,
                                Created_when = DateTime.Now,
                                Last_update_by = currentUserId.Value,
                                Last_update_when = DateTime.Now
                            };

                            await documentInfoRepository.Add(documentInfo);
                        }
                    }

                    SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred while updating the project: " + ex.Message);
                    ModelState.AddModelError("", "An error occurred while updating the project.");
                }
            }

            ViewData["IdAc"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", project.IdAc);
            return View(project);
        }

        // POST: Admin/Projects/Delete/5
        [HttpPost]
        public async Task<JsonResult> DeleteId(int id)
        {
            try
            {
                // Lấy dự án từ repository với AsNoTracking để không bị theo dõi
                var project = await projectRepository.GetProjectById(id);
                if (project == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sự kiện" });
                }

                // Xử lý xóa các DocumentInfos liên quan đến dự án
                var documentInfos = await documentInfoRepository.GetDocumentInfoByProjectId(id);
                foreach (var documentInfo in documentInfos)
                {
                    var filePath = Path.Combine("wwwroot/Upload/Images", documentInfo.UrlDocument);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath); // Xóa tệp từ thư mục
                    }

                    // Xóa bản ghi DocumentInfo khỏi cơ sở dữ liệu
                    await documentInfoRepository.Delete(documentInfo.IdDcIf);
                }

                // Xóa dự án khỏi cơ sở dữ liệu
                await projectRepository.Delete(id);

                SetAlert(Commons.Contants.Delete_success, Commons.Contants.success);
                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                // Ghi lỗi ra console
                Console.WriteLine($"Lỗi khi xóa dự án với id: {id}. Chi tiết lỗi: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ChangeActive(int id)
        {
            var result = await projectRepository.ChangeActive(id);
            return Json(new { status = result });
        }
    }
}
