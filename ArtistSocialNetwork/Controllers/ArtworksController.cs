using ArtistSocialNetwork.Models;
using Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.Diagnostics;
using System.IO;

namespace ArtistSocialNetwork.Controllers
{
    public class ArtworksController : BaseController
    {
        private readonly IArtworkRepository _artworkRepository;
        private readonly IDocumentInfoRepository _documentInfoRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITypeOfArtworkRepository _typeOfArtworkRepository;
        private readonly IReactionRepository _reactionRepository;

        public ArtworksController(ILogger<BaseController> logger, ApplicationDbContext context, IArtworkRepository artworkRepository,
            IDocumentInfoRepository documentInfoRepository,
            IAccountRepository accountRepository, ITypeOfArtworkRepository typeOfArtworkRepository, IReactionRepository reactionRepository)
            : base(logger, context)
        {
            _artworkRepository = artworkRepository;
            _documentInfoRepository = documentInfoRepository;
            _accountRepository = accountRepository;
            _typeOfArtworkRepository = typeOfArtworkRepository;
            _reactionRepository = reactionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? typeId)
        {
            await GetUsersByRoles(); // Get the list of artists or admins
            await GetArtworkTypes(); // Get the artwork types

            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            if (currentUserId != null)
            {
                var user = await _accountRepository.GetAccountById(currentUserId.Value);
                ViewBag.UserName = user?.AccountDetail?.Fullname ?? "Unknown";
                var userAvatar = await _documentInfoRepository.GetDocumentInfoByAccountId(currentUserId.Value);
                ViewBag.UserAvatar = userAvatar?.UrlDocument ?? "default-profile.png";
                Debug.WriteLine($"UserName: {ViewBag.UserName}, UserAvatar URL: {ViewBag.UserAvatar}");
            }
            else
            {
                ViewBag.UserName = "Unknown";
                ViewBag.UserAvatar = "default-profile.png";
                Debug.WriteLine("CurrentUserId is null.");
            }

            if (typeId.HasValue)
            {
                await GetArtworksByType(typeId); // Get artworks by type
            }
            else
            {
                await GetArtworkList(); // Get all artworks
            }

            ViewBag.SelectedTypeId = typeId; // Save selected artwork type for view
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ToggleLike(int artworkId)
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            if (currentUserId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thả tim." });
            }

            var existingReaction = (await _reactionRepository.GetReactionAll())
                .FirstOrDefault(r => r.IdArtwork == artworkId && r.IdAc == currentUserId);

            if (existingReaction != null)
            {
                // User has already liked the post, so remove the like
                await _reactionRepository.Delete(existingReaction.IdReaction);
            }
            else
            {
                // User has not liked the post, so add a new like
                var newReaction = new Reaction
                {
                    IdArtwork = artworkId,
                    IdAc = currentUserId.Value,
                    Action = true, // "Thả tim" action
                    CreatedAt = DateTime.Now
                };
                await _reactionRepository.Add(newReaction);
            }

            // Return the updated like count
            var likeCount = (await _reactionRepository.GetReactionAll()).Count(r => r.IdArtwork == artworkId && r.Action == true);
            return Json(new { success = true, likeCount });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Title,Description,Tags,IdTypeOfArtwork")] Artwork artwork, List<IFormFile> ImageFiles)
        {
            await GetArtworkTypes(); // Lấy danh sách loại tác phẩm

            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            if (currentUserId == null)
            {
                ModelState.AddModelError("", "Cannot identify the current user.");
                await GetArtworkList(); // Reload artwork list in case of errors
                return View(artwork);
            }

            var user = await _accountRepository.GetAccountById(currentUserId.Value);
            ViewBag.UserName = user?.AccountDetail?.Fullname ?? "Unknown";
            var userAvatar = await _documentInfoRepository.GetDocumentInfoByAccountId(currentUserId.Value);
            ViewBag.UserAvatar = userAvatar?.UrlDocument ?? "default-profile.png";

            if (ModelState.IsValid)
            {
                // Set metadata
                artwork.CreatedBy = currentUserId.Value;
                artwork.LastUpdateBy = currentUserId.Value;
                artwork.CreatedWhen = DateTime.Now;
                artwork.LastUpdateWhen = DateTime.Now;
                artwork.IdAc = currentUserId.Value;
                artwork.Active = true;

                // Add artwork to the repository
                await _artworkRepository.Add(artwork);

                // Handle image upload
                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    try
                    {
                        foreach (var file in ImageFiles)
                        {
                            if (file.Length > 0) // Check if the file is not empty
                            {
                                // Generate a unique file name to avoid duplication
                                var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                                var uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Images");

                                if (!Directory.Exists(uploadFolderPath))
                                {
                                    Directory.CreateDirectory(uploadFolderPath);
                                }

                                var filePath = Path.Combine(uploadFolderPath, fileName);

                                // Save the file
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                // Add the document information to the repository
                                var documentInfo = new DocumentInfo
                                {
                                    IdArtwork = artwork.IdArtwork,
                                    UrlDocument = fileName, // Ensure URL is correct
                                    Created_by = currentUserId.Value,
                                    Created_when = DateTime.Now,
                                    Active = true
                                };

                                await _documentInfoRepository.Add(documentInfo);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error uploading files: {ex.Message}");
                        ModelState.AddModelError("", "There was an error uploading the files. Please try again.");
                        await GetArtworkList(); // Reload artwork list in case of errors
                        return View(artwork);
                    }
                }

                // Refresh artwork list after posting
                await GetArtworkList();
                return RedirectToAction(nameof(Index));
            }

            // Reload artwork list in case of errors
            await GetArtworkList();
            return View(artwork);
        }

        // New: Get artwork details for editing

        [HttpGet]
        public async Task<IActionResult> GetArtworkById(int id)
        {
            Console.WriteLine($"Getting artwork with ID: {id}");

            var artwork = await _artworkRepository.GetArtworkById(id);
            if (artwork == null)
            {
                Console.WriteLine("Artwork not found.");
                return NotFound();
            }

            var images = await _documentInfoRepository.GetDocumentInfosByArtworkId(artwork.IdArtwork);

            var imageUrls = images.Select(img => new
            {
                IdDcIf = img.IdDcIf,
                UrlDocument = !string.IsNullOrEmpty(img.UrlDocument) ? Url.Content("~/Upload/Images/" + img.UrlDocument) : null
            }).ToList();

            Console.WriteLine($"Image URLs being returned: {string.Join(", ", imageUrls.Select(i => i.UrlDocument))}");

            return Json(new
            {
                idArtwork = artwork.IdArtwork,
                title = artwork.Title,
                description = artwork.Description,
                tags = artwork.Tags,
                idTypeOfArtwork = artwork.IdTypeOfArtwork,
                images = imageUrls
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArtwork([Bind("IdArtwork,Title,Description,Tags,IdTypeOfArtwork")] Artwork artwork, List<IFormFile> ImageFiles)
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            if (currentUserId == null)
            {
                Console.WriteLine("CurrentUserId is null. Cannot identify the user.");
                return Json(new { success = false, message = "Không thể xác định người dùng hiện tại." });
            }

            // Ghi log dữ liệu nhận được từ form
            Console.WriteLine($"Received Artwork Data: Id: {artwork.IdArtwork}, Title: {artwork.Title}, Description: {artwork.Description}, IdTypeOfArtwork: {artwork.IdTypeOfArtwork}");

            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                Console.WriteLine("Invalid artwork data received: " + string.Join(", ", errorMessages));
                return Json(new { success = false, message = "Dữ liệu tác phẩm không hợp lệ.", errors = errorMessages });
            }

            try
            {
                // Lấy Artwork hiện tại từ cơ sở dữ liệu
                var existingArtwork = await _artworkRepository.GetArtworkById(artwork.IdArtwork);
                if (existingArtwork == null)
                {
                    Console.WriteLine("Artwork không tồn tại.");
                    return Json(new { success = false, message = "Artwork không tồn tại." });
                }

                // Gán các giá trị cập nhật
                existingArtwork.Title = artwork.Title;
                existingArtwork.Description = artwork.Description;
                existingArtwork.Tags = artwork.Tags;
                existingArtwork.IdTypeOfArtwork = artwork.IdTypeOfArtwork;
                existingArtwork.LastUpdateBy = currentUserId.Value;
                existingArtwork.LastUpdateWhen = DateTime.Now;

                // Log thông tin trước khi cập nhật
                Console.WriteLine("Attempting to update artwork...");

                // Update artwork trong cơ sở dữ liệu
                await _artworkRepository.Update(existingArtwork);
                Console.WriteLine("Artwork updated successfully in the database.");

                // Xử lý xóa hình ảnh
                var deletedImages = Request.Form["DeletedImages"].ToString().Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
                Console.WriteLine($"Images to be deleted: {string.Join(", ", deletedImages)}");

                foreach (var imageId in deletedImages)
                {
                    await _documentInfoRepository.Delete(imageId);
                    Console.WriteLine($"Deleted image with ID: {imageId}");
                }

                // Xử lý tải hình ảnh mới
                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    foreach (var file in ImageFiles)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
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
                                IdArtwork = existingArtwork.IdArtwork,
                                UrlDocument = fileName,
                                Created_by = currentUserId.Value,
                                Created_when = DateTime.Now,
                                Active = true
                            };

                            await _documentInfoRepository.Add(documentInfo);
                            Console.WriteLine($"Uploaded new image: {fileName}");
                        }
                    }
                }

                await GetArtworkList();
                Console.WriteLine("Artwork list refreshed.");
                return Json(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database update error: {dbEx.InnerException?.Message}");
                return Json(new { success = false, message = "Lỗi khi cập nhật tác phẩm. Vui lòng kiểm tra lại dữ liệu." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while updating artwork: {ex.Message}");
                return Json(new { success = false, message = "Lỗi khi cập nhật tác phẩm." });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteArtwork(int id)
        {
            try
            {
                var artwork = await _artworkRepository.GetArtworkById(id);
                if (artwork == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy tác phẩm." });
                }

                // Xóa tất cả các hình ảnh liên quan đến artwork
                var images = await _documentInfoRepository.GetDocumentInfosByArtworkId(id);
                foreach (var image in images)
                {
                    await _documentInfoRepository.Delete(image.IdDcIf);
                    Console.WriteLine($"Deleted image with ID: {image.IdDcIf}");
                }

                // Xóa tất cả các phản ứng liên quan đến artwork (nếu có)
                var reactions = (await _reactionRepository.GetReactionAll()).Where(r => r.IdArtwork == id).ToList();
                foreach (var reaction in reactions)
                {
                    await _reactionRepository.Delete(reaction.IdReaction);
                    Console.WriteLine($"Deleted reaction with ID: {reaction.IdReaction}");
                }

                // Xóa tác phẩm
                await _artworkRepository.Delete(id);
                Console.WriteLine($"Deleted artwork with ID: {id}");

                // Làm mới danh sách artwork
                await GetArtworkList();
                return Json(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database update error: {dbEx.InnerException?.Message}");
                return Json(new { success = false, message = "Lỗi khi xóa tác phẩm. Vui lòng kiểm tra lại dữ liệu." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deleting artwork: {ex.Message}");
                return Json(new { success = false, message = "Lỗi khi xóa tác phẩm." });
            }
        }

        // Fetch artists or admins
        private async Task GetUsersByRoles()
        {
            var accounts = await _accountRepository.GetAccountAll();

            var usersWithRoles = accounts
                .Where(a => a.AccountRole != null &&
                            (a.AccountRole.RoleName.Equals("Nghệ Sỹ", StringComparison.OrdinalIgnoreCase) ||
                             a.AccountRole.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
                .Select(user => new
                {
                    Name = user.AccountDetail?.Fullname ?? "Unknown",
                    DateOfBirth = user.AccountDetail?.Birthday,
                    Country = user.AccountDetail?.Nationality ?? "Unknown",
                    ProfileImage = user.DocumentInfos.FirstOrDefault()?.UrlDocument ?? "default-profile.png"
                })
                .ToList();

            ViewBag.UsersWithRoles = usersWithRoles;
        }

        // Fetch artwork types
        private async Task GetArtworkTypes()
        {
            var artworkTypes = await _typeOfArtworkRepository.GetTypeOfArtworkAll();
            ViewBag.ArtworkTypes = artworkTypes;
        }

        // Fetch artworks by type
        private async Task GetArtworksByType(int? typeId)
        {
            var artworks = await _artworkRepository.GetArtworkAll();
            if (typeId.HasValue)
            {
                artworks = artworks
                    .Where(a => a.IdTypeOfArtwork == typeId.Value)
                    .OrderByDescending(a => a.LastUpdateWhen ?? a.CreatedWhen) // Sort by latest
                    .ToList();
            }

            ViewBag.ArtworkList = await CreateArtworkViewModelList(artworks);
        }

        // Fetch all artworks
        private async Task GetArtworkList()
        {
            var artworks = await _artworkRepository.GetArtworkAll();

            artworks = artworks
                .OrderByDescending(a => a.LastUpdateWhen ?? a.CreatedWhen)
                .ToList();

            ViewBag.ArtworkList = await CreateArtworkViewModelList(artworks);
        }

        private async Task<List<dynamic>> CreateArtworkViewModelList(IEnumerable<Artwork> artworks)
        {
            var artworkViewModelList = new List<dynamic>();
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

            foreach (var artwork in artworks)
            {
                Debug.WriteLine($"Processing artwork ID: {artwork.IdArtwork}");

                var images = await _documentInfoRepository.GetDocumentInfosByArtworkId(artwork.IdArtwork);
                var uploaderAvatarInfo = await _documentInfoRepository.GetDocumentInfoByAccountId(artwork.IdAc ?? 0);

                // Fetch like count and user like status
                var reactions = await _reactionRepository.GetReactionAll();
                int likeCount = reactions.Count(r => r.IdArtwork == artwork.IdArtwork && r.Action == true);
                bool isLikedByCurrentUser = currentUserId.HasValue && reactions.Any(r => r.IdArtwork == artwork.IdArtwork && r.IdAc == currentUserId.Value && r.Action == true);

                artworkViewModelList.Add(new
                {
                    Artwork = artwork,
                    Images = images.Where(img => !string.IsNullOrEmpty(img.UrlDocument))
                                   .Select(img => Url.Content(img.UrlDocument))
                                   .ToList(),
                    UploaderName = artwork.Account?.AccountDetail?.Fullname ?? "Unknown",
                    UploaderAvatar = uploaderAvatarInfo?.UrlDocument ?? "default-profile.png",
                    ArtworkType = artwork.TypeOfArtwork?.NameTypeOfArtwork ?? "Unknown",
                    TimeAgo = artwork.LastUpdateWhen.HasValue ? CalculateTimeAgo(artwork.LastUpdateWhen.Value) : "Unknown time",
                    LikeCount = likeCount,
                    IsLikedByCurrentUser = isLikedByCurrentUser
                });
            }

            return artworkViewModelList;
        }

        private string CalculateTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "Bây giờ";
            else if (timeSpan.TotalMinutes < 60)
                return $"{timeSpan.Minutes} phút trước";
            else if (timeSpan.TotalHours < 24)
                return $"{timeSpan.Hours} giờ trước";
            else if (timeSpan.TotalDays < 7)
                return $"{timeSpan.Days} ngày trước";
            else if (timeSpan.TotalDays < 30)
            {
                int weeks = (int)(timeSpan.TotalDays / 7);
                return $"{weeks} tuần trước";
            }
            else if (timeSpan.TotalDays < 365)
            {
                int months = (int)(timeSpan.TotalDays / 30);
                return $"{months} tháng trước";
            }
            else
            {
                int years = (int)(timeSpan.TotalDays / 365);
                return $"{years} năm trước";
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
