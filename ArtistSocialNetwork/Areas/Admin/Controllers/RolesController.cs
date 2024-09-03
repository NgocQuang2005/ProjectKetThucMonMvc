using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Business;
using Commons; // Thêm namespace để sử dụng lớp Contants
using DTO;
using Microsoft.AspNetCore.Authorization;
using Repository;
using X.PagedList;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class RolesController : BaseController
    {
        IRoleRepository roleReponsitory;
        public RolesController()
        {
            roleReponsitory = new RoleRepository();
        }
        // GET: Admin/Roles
        public async Task<IActionResult> Index(string searchString, int? page)
        {

            var role = await roleReponsitory.GetRoleAll();
            if (!string.IsNullOrEmpty(searchString))
            {
                role = role.Where(c => Commons.Library.ConvertToUnSign(c.RoleName.ToLower()).Contains(Commons.Library.ConvertToUnSign(searchString.ToLower())));
            }

            ViewBag.Page = 5;
            return View(role.ToPagedList(page ?? 1, (int)ViewBag.Page));

        }

        // GET: Admin/Roles/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRole,Active,RoleName")] Role role)
        {
            if (ModelState.IsValid)
            {
                await roleReponsitory.Add(role);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Admin/Roles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var role = await roleReponsitory.GetRoleById(Convert.ToInt32(id));
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Admin/Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRole,Active,RoleName")] Role role)
        {
            if (id != role.IdRole)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await roleReponsitory.Update(role);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        // GET: Admin/Roles/Delete/5
        [HttpPost]
        public async Task<JsonResult> DeleteId(int id)
        {
            try
            {
                var role = await roleReponsitory.GetRoleById(id);
                if (role == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bản ghi" });
                }
                await roleReponsitory.Delete(id);
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
        public async Task<JsonResult> ChangeActive(int id)
        {
            var result = await roleReponsitory.ChangeActive(id);
            return Json(new
            {
                status = result
            });
        }
    }
}
