using Business;
using Microsoft.AspNetCore.Mvc;

namespace ArtistSocialNetwork.Controllers
{
    public class ProjectsController : BaseController
    {
        public ProjectsController(ILogger<BaseController> logger, ApplicationDbContext context) : base(logger, context)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
