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
using System.Data;
namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class ArtworksController : BaseController
    {
        //private readonly IArtworkRepository artworkRepository;

        private readonly ApplicationDbContext _context;

        public ArtworksController(ApplicationDbContext context)
        {
            //artworkRepository = new ArtworkRepository();

            _context = context;
        }

        // GET: Admin/Artworks
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            //var artworks = await artworkRepository.GetArtworkAll();
            //if (!string.IsNullOrEmpty(searchString))
            //{
            //    artworks = artworks.Where(c => Commons.Library.ConvertToUnSign(c.Title.ToLower()).Contains(Commons.Library.ConvertToUnSign(searchString.ToLower())));
            //}

            //ViewBag.Page = 5;
            //return View(artworks.ToPagedList(page ?? 1, (int)ViewBag.Page));
            var applicationDbContext = _context.Artworks
                .Include(a => a.Account)
                .Include(a => a.Creator)
                .Include(a => a.TypeOfArtwork)
                .Include(a => a.Updater);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Artworks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Artworks == null)
            {
                return NotFound();
            }

            var artwork = await _context.Artworks
                .Include(a => a.Account)
                .Include(a => a.Creator)
                .Include(a => a.TypeOfArtwork)
                .Include(a => a.Updater)
                .FirstOrDefaultAsync(m => m.IdArtwork == id);
            if (artwork == null)
            {
                return NotFound();
            }

            return View(artwork);
        }

        // GET: Admin/Artworks/Create
        public IActionResult Create()
        {
            ViewData["IdAc"] = new SelectList(_context.Accounts, "IdAccount", "Email");
            ViewData["CreatedBy"] = new SelectList(_context.Accounts, "IdAccount", "Email");
            ViewData["IdTypeOfArtwork"] = new SelectList(_context.TypeOfArtworks, "IdTypeOfArtwork", "Description");
            ViewData["LastUpdateBy"] = new SelectList(_context.Accounts, "IdAccount", "Email");
            return View();
        }

        // POST: Admin/Artworks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdArtwork,Active,IdAc,IdTypeOfArtwork,Title,Description,Tags,MediaType,MediaUrl,Watched,CreatedBy,CreatedWhen,LastUpdateBy,LastUpdateWhen")] Artwork artwork)
        {
            if (ModelState.IsValid)
            {
                _context.Add(artwork);
                await _context.SaveChangesAsync();
                SetAlert(Commons.Contants.success, Commons.Contants.success);
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAc"] = new SelectList(_context.Accounts, "IdAccount", "Email", artwork.IdAc);
            ViewData["CreatedBy"] = new SelectList(_context.Accounts, "IdAccount", "Email", artwork.CreatedBy);
            ViewData["IdTypeOfArtwork"] = new SelectList(_context.TypeOfArtworks, "IdTypeOfArtwork", "Description", artwork.IdTypeOfArtwork);
            ViewData["LastUpdateBy"] = new SelectList(_context.Accounts, "IdAccount", "Email", artwork.LastUpdateBy);
            return View(artwork);
        }

        // GET: Admin/Artworks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Artworks == null)
            {
                return NotFound();
            }

            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null)
            {
                return NotFound();
            }
            ViewData["IdAc"] = new SelectList(_context.Accounts, "IdAccount", "Email", artwork.IdAc);
            ViewData["CreatedBy"] = new SelectList(_context.Accounts, "IdAccount", "Email", artwork.CreatedBy);
            ViewData["IdTypeOfArtwork"] = new SelectList(_context.TypeOfArtworks, "IdTypeOfArtwork", "Description", artwork.IdTypeOfArtwork);
            ViewData["LastUpdateBy"] = new SelectList(_context.Accounts, "IdAccount", "Email", artwork.LastUpdateBy);
            return View(artwork);
        }

        // POST: Admin/Artworks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdArtwork,Active,IdAc,IdTypeOfArtwork,Title,Description,Tags,MediaType,MediaUrl,Watched,CreatedBy,CreatedWhen,LastUpdateBy,LastUpdateWhen")] Artwork artwork)
        {
            if (id != artwork.IdArtwork)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(artwork);
                    await _context.SaveChangesAsync();
                    SetAlert(Commons.Contants.Update_success, Commons.Contants.success);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtworkExists(artwork.IdArtwork))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAc"] = new SelectList(_context.Accounts, "IdAccount", "Email", artwork.IdAc);
            ViewData["CreatedBy"] = new SelectList(_context.Accounts, "IdAccount", "Email", artwork.CreatedBy);
            ViewData["IdTypeOfArtwork"] = new SelectList(_context.TypeOfArtworks, "IdTypeOfArtwork", "Description", artwork.IdTypeOfArtwork);
            ViewData["LastUpdateBy"] = new SelectList(_context.Accounts, "IdAccount", "Email", artwork.LastUpdateBy);
            return View(artwork);
        }

        // GET: Admin/Artworks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Artworks == null)
            {
                return NotFound();
            }

            var artwork = await _context.Artworks
                .Include(a => a.Account)
                .Include(a => a.Creator)
                .Include(a => a.TypeOfArtwork)
                .Include(a => a.Updater)
                .FirstOrDefaultAsync(m => m.IdArtwork == id);
            if (artwork == null)
            {
                return NotFound();
            }

            return View(artwork);
        }

        // POST: Admin/Artworks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Artworks == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Artworks'  is null.");
            }
            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork != null)
            {
                _context.Artworks.Remove(artwork);
                await _context.SaveChangesAsync();
                SetAlert(Commons.Contants.Delete_success, Commons.Contants.success);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ArtworkExists(int id)
        {
            return (_context.Artworks?.Any(e => e.IdArtwork == id)).GetValueOrDefault();
        }
    }
}
