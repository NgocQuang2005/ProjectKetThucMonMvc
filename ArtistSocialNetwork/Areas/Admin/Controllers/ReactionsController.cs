using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repository;
using Business;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class ReactionsController : BaseController
    {
        private readonly IReactionRepository _reactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IArtworkRepository _artworkRepository;

        public ReactionsController(IReactionRepository reactionRepository, IAccountRepository accountRepository, IArtworkRepository artworkRepository)
        {
            _reactionRepository = reactionRepository;
            _accountRepository = accountRepository;
            _artworkRepository = artworkRepository;
        }

        // GET: Admin/Reactions
        public async Task<IActionResult> Index(string searchString, int? page, int IdAccount, int IdArtwork)
        {
            var reactions = await _reactionRepository.GetReactionAll();

            // Search functionality
            if (!string.IsNullOrEmpty(searchString))
            {
                reactions = reactions
                    .Where(r => Commons.Library.ConvertToUnSign(r.Artwork.Title.ToLower())
                    .Contains(Commons.Library.ConvertToUnSign(searchString.ToLower())) ||
                    Commons.Library.ConvertToUnSign(r.Account.Email.ToLower())
                    .Contains(Commons.Library.ConvertToUnSign(searchString.ToLower())))
                    .ToList();
            }
            if (IdAccount != 0)
            {
                reactions = reactions.Where(a => a.IdAc == IdAccount).ToList();
            }
            if (IdArtwork != 0)
            {
                reactions = reactions.Where(taw => taw.IdArtwork == IdArtwork).ToList();
            }

            // Fetch accounts and artworks for dropdowns
            var accounts = await _accountRepository.GetAccountAll();
            var artworks = await _artworkRepository.GetArtworkAll();

            // Set ViewBag for dropdowns
            ViewBag.IdAccount = new SelectList(accounts, "IdAccount", "Email");
            ViewBag.IdArtwork = new SelectList(artworks, "IdArtwork", "Title");

            // Pagination settings
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var pagedReactions = reactions.ToPagedList(pageNumber, pageSize);

            // Pass the current page and total pages to the view using ViewBag
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = pagedReactions.PageCount;

            return View(pagedReactions);
        }

        // GET: Admin/Reactions/Create
        public async Task<IActionResult> Create()
        {
            ViewData["IdAc"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email");
            ViewData["IdArtwork"] = new SelectList(await _artworkRepository.GetArtworkAll(), "IdArtwork", "Title");
            return View();
        }

        // POST: Admin/Reactions/Create
        // POST: Admin/Reactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdReaction,IdArtwork,IdAc,Action,CreatedAt")] Reaction reaction)
        {
            if (ModelState.IsValid)
            {
                await _reactionRepository.Add(reaction);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, tái tạo lại ViewData cho danh sách thả xuống
            ViewData["IdAc"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email", reaction.IdAc);
            ViewData["IdArtwork"] = new SelectList(await _artworkRepository.GetArtworkAll(), "IdArtwork", "Title", reaction.IdArtwork);

            // Thêm thông báo lỗi vào ModelState nếu cần thiết
            if (reaction.IdAc == 0)
            {
                ModelState.AddModelError("IdAc", "Vui lòng chọn người dùng.");
            }
            if (reaction.IdArtwork == 0)
            {
                ModelState.AddModelError("IdArtwork", "Vui lòng chọn tác phẩm.");
            }

            ModelState.AddModelError("", "Có lỗi xảy ra khi tạo phản ứng. Vui lòng kiểm tra lại các thông tin đã nhập.");
            return View(reaction);
        }


        // POST: Admin/Reactions/Delete/5
        [HttpPost]
        public async Task<JsonResult> DeleteId(int id)
        {
            try
            {
                var reaction = await _reactionRepository.GetReactionById(id);
                if (reaction == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bản ghi" });
                }

                await _reactionRepository.Delete(id);
                SetAlert(Commons.Contants.Delete_success, Commons.Contants.success);
                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Admin/Reactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reaction = await _reactionRepository.GetReactionById(id.Value);
            if (reaction == null)
            {
                return NotFound();
            }

            ViewData["IdAc"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email", reaction.IdAc);
            ViewData["IdArtwork"] = new SelectList(await _artworkRepository.GetArtworkAll(), "IdArtwork", "Title", reaction.IdArtwork);
            return View(reaction);
        }

        // POST: Admin/Reactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdReaction,IdArtwork,IdAc,Action,CreatedAt")] Reaction reaction)
        {
            if (id != reaction.IdReaction)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _reactionRepository.Update(reaction);
                    SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ReactionExists(reaction.IdReaction))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Tái tạo lại ViewData nếu ModelState không hợp lệ
            ViewData["IdAc"] = new SelectList(await _accountRepository.GetAccountAll(), "IdAccount", "Email", reaction.IdAc);
            ViewData["IdArtwork"] = new SelectList(await _artworkRepository.GetArtworkAll(), "IdArtwork", "Title", reaction.IdArtwork);

            return View(reaction);
        }

        private async Task<bool> ReactionExists(int id)
        {
            var reaction = await _reactionRepository.GetReactionById(id);
            return reaction != null;
        }
    }
}
