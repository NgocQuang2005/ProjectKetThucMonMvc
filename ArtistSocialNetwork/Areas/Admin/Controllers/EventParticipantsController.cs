using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Business;
using Repository;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class EventParticipantsController : BaseController
    {
        private readonly IEventRepository eventRepository;
        private readonly IAccountRepository accountRepository;
        private readonly IEventParticipantRepository eventParticipantRepository;

        public EventParticipantsController(IEventRepository eventRepo, IAccountRepository accountRepo, IEventParticipantRepository eventParticipantRepo)
        {
            eventRepository = eventRepo;
            accountRepository = accountRepo;
            eventParticipantRepository = eventParticipantRepo;
        }

        // GET: Admin/EventParticipants
        public async Task<IActionResult> Index(string searchString, int? page, int IdEvent, int IdAccount)
        {
            var eventParticipants = await eventParticipantRepository.GetEventParticipantsAll();

            // Optional: Apply search filter if needed
            if (!string.IsNullOrEmpty(searchString))
            {
                eventParticipants = eventParticipants.Where(ep =>
                    ep.Event.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    ep.Account.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (IdAccount != 0)
            {
                eventParticipants = eventParticipants.Where(a => a.IdAc == IdAccount).ToList();
            }
            if (IdEvent != 0)
            {
                eventParticipants = eventParticipants.Where(taw => taw.IdEvent == IdEvent).ToList();
            }

            // Get events and accounts for display in ViewBag
            var events = await eventRepository.GetEventAll();
            var accounts = await accountRepository.GetAccountAll();

            ViewBag.EventNames = events.ToDictionary(e => e.IdEvent, e => e.Title);
            ViewBag.AccountEmails = accounts.ToDictionary(a => a.IdAccount, a => a.Email);

            // Pass SelectList to ViewBag for dropdowns
            ViewBag.IdAccount = new SelectList(accounts, "IdAccount", "Email");
            ViewBag.IdEvent = new SelectList(events, "IdEvent", "Title");

            // Pagination settings
            int pageSize = 10; // Number of items per page
            int pageNumber = page ?? 1; // Current page number

            // Convert to IPagedList
            var pagedEventParticipants = eventParticipants.ToPagedList(pageNumber, pageSize);

            return View(pagedEventParticipants);
        }


        // GET: Admin/EventParticipants/Create
        public async Task<IActionResult> Create()
        {
            ViewData["IdAc"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email");
            ViewData["IdEvent"] = new SelectList(await eventRepository.GetEventAll(), "IdEvent", "Title");
            return View();
        }

        // POST: Admin/EventParticipants/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEventParticipant,Active,IdEvent,IdAc,RegistrationTime")] EventParticipants eventParticipants)
        {
            if (ModelState.IsValid)
            {
                eventParticipants.RegistrationTime = DateTime.Now;
                await eventParticipantRepository.Add(eventParticipants);
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdAc"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", eventParticipants.IdAc);
            ViewData["IdEvent"] = new SelectList(await eventRepository.GetEventAll(), "IdEvent", "Title", eventParticipants.IdEvent);
            return View(eventParticipants);
        }

        // POST: Admin/EventParticipants/Delete/5
        

        // GET: Admin/EventParticipants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventParticipant = await eventParticipantRepository.GetEventParticipantsById(id.Value);
            if (eventParticipant == null)
            {
                return NotFound();
            }

            ViewData["IdAc"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", eventParticipant.IdAc);
            ViewData["IdEvent"] = new SelectList(await eventRepository.GetEventAll(), "IdEvent", "Title", eventParticipant.IdEvent);
            return View(eventParticipant);
        }

        // POST: Admin/EventParticipants/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEventParticipant,Active,IdEvent,IdAc,RegistrationTime")] EventParticipants eventParticipants)
        {
            if (id != eventParticipants.IdEventParticipant)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await eventParticipantRepository.Update(eventParticipants);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EventParticipantExists(eventParticipants.IdEventParticipant))
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

            ViewData["IdAc"] = new SelectList(await accountRepository.GetAccountAll(), "IdAccount", "Email", eventParticipants.IdAc);
            ViewData["IdEvent"] = new SelectList(await eventRepository.GetEventAll(), "IdEvent", "Title", eventParticipants.IdEvent);
            return View(eventParticipants);
        }

        private async Task<bool> EventParticipantExists(int id)
        {
            return await eventParticipantRepository.GetEventParticipantsById(id) != null;
        }
        [HttpPost]
        public async Task<JsonResult> DeleteId(int id)
        {
            try
            {
                var eventParticipant = await eventParticipantRepository.GetEventParticipantsById(id);
                if (eventParticipant == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người tham gia sự kiện." });
                }

                await eventParticipantRepository.Delete(id);
                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Admin/EventParticipants/ChangeActive/5
        [HttpPost]
        public async Task<JsonResult> ChangeActive(int id)
        {
            var result = await eventParticipantRepository.ChangeActive(id);
            return Json(new { status = result });
        }
    }
}
