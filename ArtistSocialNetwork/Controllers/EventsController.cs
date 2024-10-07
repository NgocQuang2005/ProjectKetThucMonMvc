using Business;
using Microsoft.AspNetCore.Mvc;

namespace ArtistSocialNetwork.Controllers
{
    public class EventsController : BaseController
    {
        public EventsController(ILogger<BaseController> logger, ApplicationDbContext context) : base(logger, context)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
