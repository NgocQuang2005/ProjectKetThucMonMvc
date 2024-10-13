using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Business; // Import models
using System.Linq;

namespace ArtistSocialNetwork.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ILogger<BaseController> _logger;
        protected readonly ApplicationDbContext _context; // Add ApplicationDbContext for database access

        // Constructor chính thức, nhận cả ILogger và ApplicationDbContext
        public BaseController(ILogger<BaseController> logger, ApplicationDbContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Đảm bảo logger không null
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Đảm bảo _context không null
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();
            var area = context.RouteData.Values["area"]?.ToString();

            // Bỏ qua kiểm tra session nếu yêu cầu đến từ Admin, trang đăng nhập của Web, hoặc trang đăng ký
            if (area == "Admin" || (controller == "Login" && action == "Index") ||
                (controller == "Login" && action == "Logout") || (controller == "SignUp" && action == "Index"))
            {
                base.OnActionExecuting(context);
                return;
            }

            // Kiểm tra session xem có thông tin người dùng hay không
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            _logger.LogInformation($"CurrentUserId in session: {currentUserId}");

            // Nếu không có session, chuyển hướng đến trang đăng nhập
            if (currentUserId == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Login" },
                    { "action", "Index" }
                });
                return;
            }

            // Gán tên người dùng vào ViewBag
            ViewBag.CurrentUserName = GetCurrentUserName();

            base.OnActionExecuting(context);
        }

        // Method to get the full name of the current logged-in user
        protected string GetCurrentUserName()
        {
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

            if (currentUserId != null)
            {
                var accountDetail = _context.AccountDetails.FirstOrDefault(ad => ad.IdAccount == currentUserId);
                if (accountDetail != null)
                {
                    return accountDetail.Fullname;
                }
            }

            return "Người dùng không xác định";
        }

        public void SetAlert(string msg, string type)
        {
            TempData["AlertMessage"] = msg;
            switch (type)
            {
                case "success":
                    TempData["Type"] = "alert-success";
                    break;
                case "warning":
                    TempData["Type"] = "alert-warning";
                    break;
                case "error":
                    TempData["Type"] = "alert-danger";
                    break;
                default:
                    TempData["Type"] = "alert-info";
                    break;
            }
        }
    }
}
