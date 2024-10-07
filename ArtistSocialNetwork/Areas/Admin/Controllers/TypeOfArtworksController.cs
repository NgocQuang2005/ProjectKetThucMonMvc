using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Business;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;
using Repository;
using System.Data;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class TypeOfArtworksController : BaseController
    {
        private readonly ITypeOfArtworkRepository typeOfArtworkRepository;

        public TypeOfArtworksController()
        {
           typeOfArtworkRepository = new TypeOfArtworkRepository();
        }

        // GET: Admin/TypeOfArtworks
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            var typeofartwork = await typeOfArtworkRepository.GetTypeOfArtworkAll();
            if (!string.IsNullOrEmpty(searchString))
            {
                typeofartwork = typeofartwork.Where(c => Commons.Library.ConvertToUnSign(c.NameTypeOfArtwork.ToLower()).Contains(Commons.Library.ConvertToUnSign(searchString.ToLower())));
            }

            ViewBag.Page = 5;
            return View(typeofartwork.ToPagedList(page ?? 1, (int)ViewBag.Page));
        }


        // GET: Admin/TypeOfArtworks/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTypeOfArtwork,Active,NameTypeOfArtwork,Description")] TypeOfArtwork typeOfArtwork)
        {
            if (ModelState.IsValid)
            {
                await typeOfArtworkRepository.Add(typeOfArtwork);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }

            // If the model is not valid, check the ModelState for errors
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                if (state.Errors.Count > 0)
                {
                    foreach (var error in state.Errors)
                    {
                        // Log the error or set a message to view
                        // For demonstration, let's log the error to the console
                        Console.WriteLine($"Error in {key}: {error.ErrorMessage}");
                    }
                }
            }

            // Optionally, set a custom error message to alert the user
            SetAlert("There were errors with the form submission. Please check the inputs and try again.", Commons.Contants.FAIL);

            return View(typeOfArtwork);
        }


        // GET: Admin/TypeOfArtworks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var typeOfArtwork = await typeOfArtworkRepository.GetTypeOfArtworkById(Convert.ToInt32(id));
            if (typeOfArtwork == null)
            {
                return NotFound();
            }
            return View(typeOfArtwork);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTypeOfArtwork,Active,NameTypeOfArtwork,Description")] TypeOfArtwork typeOfArtwork)
        {
            if (id != typeOfArtwork.IdTypeOfArtwork)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await typeOfArtworkRepository.Update(typeOfArtwork);
                SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }
            return View(typeOfArtwork);
        }

        // GET: Admin/TypeOfArtworks/Delete/5
        [HttpPost]
        public async Task<JsonResult> DeleteId(int id)
        {
            try
            {
                var role = await typeOfArtworkRepository.GetTypeOfArtworkById(id);
                if (role == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bản ghi" });
                }
                await typeOfArtworkRepository.Delete(id);
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
            var result = await typeOfArtworkRepository.ChangeActive(id);
            return Json(new
            {
                status = result
            });
        }
    }
}
