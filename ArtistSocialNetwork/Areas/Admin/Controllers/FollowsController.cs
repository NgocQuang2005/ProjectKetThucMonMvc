using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Repository;
using X.PagedList;
using Business;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class FollowsController : BaseController
    {
        private readonly IFollowRepository _followRepository;
        private readonly IAccountRepository _accountRepository;

        public FollowsController(IFollowRepository followRepository, IAccountRepository accountRepository)
        {
            _followRepository = followRepository;
            _accountRepository = accountRepository;
        }

        // GET: Admin/Follows
        public async Task<IActionResult> Index(string searchString, int? page, int IdFollower, int IdFollowing)
        {
            var follows = await _followRepository.GetFollowAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                follows = follows.Where(f =>
                    f.Follower.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    f.Following.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (IdFollower != 0)
            {
                follows = follows.Where(f => f.IdFollower == IdFollower).ToList();
            }
            if (IdFollowing != 0)
            {
                follows = follows.Where(f => f.IdFollowing == IdFollowing).ToList();
            }

            var accounts = await _accountRepository.GetAccountAll();

            ViewBag.Follower = accounts.ToDictionary(a => a.IdAccount, a => a.Email);
            ViewBag.Following = accounts.ToDictionary(a => a.IdAccount, a => a.Email);

            // Pass data for dropdown lists
            ViewBag.IdFollower = new SelectList(accounts, "IdAccount", "Email");
            ViewBag.IdFollowing = new SelectList(accounts, "IdAccount", "Email");

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var pagedFollows = follows.ToPagedList(pageNumber, pageSize);

            return View(pagedFollows);
        }

        // GET: Admin/Follows/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var follow = await _followRepository.GetFollowById(id.Value);
            if (follow == null)
            {
                return NotFound();
            }

            return View(follow);
        }

        // GET: Admin/Follows/Create
        public async Task<IActionResult> Create()
        {
            var accounts = await _accountRepository.GetAccountAll();
            ViewData["IdFollower"] = new SelectList(accounts, "IdAccount", "Email");
            ViewData["IdFollowing"] = new SelectList(accounts, "IdAccount", "Email");
            return View();
        }

        // POST: Admin/Follows/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdFollow,Active,IdFollower,IdFollowing,CreatedWhen,LastUpdateWhen")] Follow follow)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _followRepository.Add(follow);
                    SetAlert("Tạo mới follow thành công.", "success");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi tạo follow: " + ex.Message);
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi tạo follow.");
                }
            }

            // Reload the select lists in case of validation failure
            var accounts = await _accountRepository.GetAccountAll();
            ViewData["IdFollower"] = new SelectList(accounts, "IdAccount", "Email", follow.IdFollower);
            ViewData["IdFollowing"] = new SelectList(accounts, "IdAccount", "Email", follow.IdFollowing);
            return View(follow);
        }

        // GET: Admin/Follows/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var follow = await _followRepository.GetFollowById(id.Value);
            if (follow == null)
            {
                return NotFound();
            }

            var accounts = await _accountRepository.GetAccountAll();
            ViewData["IdFollower"] = new SelectList(accounts, "IdAccount", "Email", follow.IdFollower);
            ViewData["IdFollowing"] = new SelectList(accounts, "IdAccount", "Email", follow.IdFollowing);
            return View(follow);
        }

        // POST: Admin/Follows/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdFollow,Active,IdFollower,IdFollowing,CreatedWhen,LastUpdateWhen")] Follow follow)
        {
            if (id != follow.IdFollow)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _followRepository.Update(follow);
                    SetAlert("Cập nhật follow thành công.", "success");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi cập nhật follow: " + ex.Message);
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật follow.");
                }
            }

            var accounts = await _accountRepository.GetAccountAll();
            ViewData["IdFollower"] = new SelectList(accounts, "IdAccount", "Email", follow.IdFollower);
            ViewData["IdFollowing"] = new SelectList(accounts, "IdAccount", "Email", follow.IdFollowing);
            return View(follow);
        }

        // POST: Admin/Follows/DeleteId/5
        [HttpPost]
        public async Task<JsonResult> DeleteId(int id)
        {
            try
            {
                var follow = await _followRepository.GetFollowById(id);
                if (follow == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy follow." });
                }

                await _followRepository.Delete(id);
                SetAlert("Xóa follow thành công.", "success");
                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Admin/Follows/ChangeActive/5
        [HttpPost]
        public async Task<JsonResult> ChangeActive(int id)
        {
            var result = await _followRepository.ChangeActive(id);
            return Json(new { status = result });
        }

        private async Task<bool> FollowExists(int id)
        {
            return await _followRepository.GetFollowById(id) != null;
        }
    }
}
