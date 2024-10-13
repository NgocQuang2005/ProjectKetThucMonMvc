using ArtistSocialNetwork.Models;
using Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly IRoleRepository _roleRepository;

        public ArtworksController(ILogger<BaseController> logger, ApplicationDbContext context, IArtworkRepository artworkRepository,
            IDocumentInfoRepository documentInfoRepository,
            IAccountRepository accountRepository, ITypeOfArtworkRepository typeOfArtworkRepository, IReactionRepository reactionRepository, IRoleRepository roleRepository)
            : base(logger, context)
        {
            _artworkRepository = artworkRepository;
            _documentInfoRepository = documentInfoRepository;
            _accountRepository = accountRepository;
            _typeOfArtworkRepository = typeOfArtworkRepository;
            _reactionRepository = reactionRepository;
            _roleRepository = roleRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? typeId, string filter = "all", string searchTerm = "", int page = 1, int pageSize = 5)
        {
            await GetUsersByRoles(); // Lấy danh sách nghệ sỹ hoặc admin
            await GetArtworkTypes(); // Lấy danh sách loại tác phẩm

            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.Filter = filter;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedTypeId = typeId; // Lưu loại tác phẩm đã chọn để hiển thị trong view

            if (currentUserId != null)
            {
                var user = await _accountRepository.GetAccountById(currentUserId.Value);

                if (user != null && user.AccountRole != null)
                {
                    var userRole = await _roleRepository.GetRoleById(user.AccountRole.IdRole);
                    ViewBag.UserRoleId = userRole?.IdRole;
                    ViewBag.UserName = user?.AccountDetail?.Fullname ?? "Unknown";
                    var userAvatar = await _documentInfoRepository.GetDocumentInfoByAccountId(currentUserId.Value);
                    ViewBag.UserAvatar = userAvatar?.UrlDocument ?? "default-profile.png";
                }
            }
            else
            {
                ViewBag.UserRoleId = 0;
                ViewBag.UserName = "Unknown";
                ViewBag.UserAvatar = "default-profile.png";
            }

            IEnumerable<Artwork> artworks = await _artworkRepository.GetArtworkAll();

            // Lọc bài viết theo người dùng, loại tác phẩm, và từ khóa tìm kiếm
            if (filter == "mine" && currentUserId != null)
            {
                artworks = artworks.Where(a => a.IdAc == currentUserId.Value).ToList();
            }

            if (typeId.HasValue)
            {
                artworks = artworks.Where(a => a.IdTypeOfArtwork == typeId.Value).ToList();
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                artworks = artworks.Where(a => a.Title != null && a.Title.ToLower().Contains(searchTerm.ToLower())).ToList();
            }

            // Sắp xếp bài viết theo thời gian cập nhật
            artworks = artworks.OrderByDescending(a => a.LastUpdateWhen ?? a.CreatedWhen).ToList();

            // Phân trang
            int totalArtworks = artworks.Count();
            int totalPages = (int)Math.Ceiling(totalArtworks / (double)pageSize);
            artworks = artworks.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Gọi phương thức để tạo danh sách view model
            ViewBag.ArtworkList = await CreateArtworkViewModelList(artworks);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

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
                await _reactionRepository.Delete(existingReaction.IdReaction);
            }
            else
            {
                var newReaction = new Reaction
                {
                    IdArtwork = artworkId,
                    IdAc = currentUserId.Value,
                    Action = true, // "Thả tim" action
                    CreatedAt = DateTime.Now
                };
                await _reactionRepository.Add(newReaction);
            }
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
                artwork.CreatedBy = currentUserId.Value;
                artwork.LastUpdateBy = currentUserId.Value;
                artwork.CreatedWhen = DateTime.Now;
                artwork.LastUpdateWhen = DateTime.Now;
                artwork.IdAc = currentUserId.Value;
                artwork.Active = true;

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

                await GetArtworkList();
                return RedirectToAction(nameof(Index));
            }
            await GetArtworkList();
            return View(artwork);
        }

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
                return Json(new { success = false, message = "Không thể xác định người dùng hiện tại." });
            }

            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Dữ liệu tác phẩm không hợp lệ.", errors = errorMessages });
            }

            try
            {
                // Lấy Artwork hiện tại từ cơ sở dữ liệu mà không theo dõi
                var existingArtwork = await _artworkRepository.GetArtworkByIdAsNoTracking(artwork.IdArtwork);
                if (existingArtwork == null)
                {
                    return Json(new { success = false, message = "Artwork không tồn tại." });
                }

                // Cập nhật các giá trị của Artwork
                existingArtwork.Title = artwork.Title;
                existingArtwork.Description = artwork.Description;
                existingArtwork.Tags = artwork.Tags;
                existingArtwork.IdTypeOfArtwork = artwork.IdTypeOfArtwork;
                existingArtwork.LastUpdateBy = currentUserId.Value;
                existingArtwork.LastUpdateWhen = DateTime.Now;

                // Đặt trạng thái thực thể là Modified để báo cho EF Core biết cần cập nhật
                _context.Entry(existingArtwork).State = EntityState.Modified;

                // Lưu thay đổi Artwork vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                // Xử lý xóa hình ảnh nếu có
                var deletedImages = Request.Form["DeletedImages"].ToString().Split(',')
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(int.Parse).ToList();
                foreach (var imageId in deletedImages)
                {
                    var imageToDelete = await _documentInfoRepository.GetDocumentInfoById(imageId);
                    if (imageToDelete != null)
                    {
                        _context.Entry(imageToDelete).State = EntityState.Deleted; // Đánh dấu để xóa
                        await _documentInfoRepository.Delete(imageId);
                    }
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
                        }
                    }
                }

                await GetArtworkList(); // Lấy danh sách tác phẩm sau khi cập nhật để hiển thị lại
                return Json(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Lỗi khi cập nhật dữ liệu vào cơ sở dữ liệu: {dbEx.Message}");
                return Json(new { success = false, message = "Lỗi khi cập nhật tác phẩm. Vui lòng kiểm tra lại dữ liệu." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi không xác định: {ex.Message}");
                return Json(new { success = false, message = "Lỗi khi cập nhật tác phẩm." });
            }
        }
        [HttpPost]
        public async Task<JsonResult> DeleteArtwork(int id)
        {
            try
            {
                // Bước 1: Lấy tác phẩm từ cơ sở dữ liệu bao gồm các thông tin liên quan
                var artwork = await _context.Artworks
                                            .Include(a => a.DocumentInfos)   // Bao gồm hình ảnh liên quan
                                            .Include(a => a.Reactions)       // Bao gồm phản hồi (Reaction) liên quan
                                            .AsNoTracking()                  // Không theo dõi artwork
                                            .FirstOrDefaultAsync(a => a.IdArtwork == id);

                if (artwork == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy tác phẩm." });
                }

                // Bước 2: Xóa tất cả các phản hồi (Reaction) liên quan đến artwork
                if (artwork.Reactions != null && artwork.Reactions.Any())
                {
                    _context.Reactions.RemoveRange(artwork.Reactions); // Xóa tất cả các phản hồi khỏi context
                }

                // Bước 3: Xóa tất cả các hình ảnh liên quan đến artwork
                if (artwork.DocumentInfos != null && artwork.DocumentInfos.Any())
                {
                    _context.DocumentInfos.RemoveRange(artwork.DocumentInfos); // Xóa tất cả các hình ảnh khỏi context
                }

                // Bước 4: Xóa artwork chính
                _context.Artworks.Remove(artwork);  // Xóa tác phẩm
                await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

                return Json(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                // Ghi lại lỗi cập nhật cơ sở dữ liệu
                Console.WriteLine($"Database update error: {dbEx.InnerException?.Message}");
                return Json(new { success = false, message = "Lỗi khi xóa tác phẩm. Vui lòng kiểm tra lại dữ liệu liên quan." });
            }
            catch (Exception ex)
            {
                // Ghi lại lỗi chung
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
                    IdAccount = user.IdAccount,  // Include IdAccount in the anonymous object
                    Name = user.AccountDetail?.Fullname ?? "Unknown",
                    DateOfBirth = user.AccountDetail?.Birthday,
                    Country = user.AccountDetail?.Nationality ?? "Unknown",
                    ProfileImage = user.DocumentInfos.FirstOrDefault()?.UrlDocument ?? "default-profile.png"
                })
                .Take(5)
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
                    IdAc = artwork.IdAc, // Thêm thuộc tính IdAc
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
