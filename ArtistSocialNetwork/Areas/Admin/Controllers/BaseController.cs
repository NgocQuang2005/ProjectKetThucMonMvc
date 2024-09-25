using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ArtistSocialNetwork.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        // Phương thức kiểm tra session trước khi thực hiện các hành động
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Lấy tên controller và action hiện tại
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();

            // Bỏ qua kiểm tra session nếu người dùng đang truy cập trang đăng nhập
            if (controller == "Login" && action == "Index")
            {
                base.OnActionExecuting(context);
                return;
            }

            // Kiểm tra session xem có thông tin người dùng hay không
            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");

            // Nếu không có session, chuyển hướng đến trang đăng nhập
            if (currentUserId == null)
            {
                context.Result = RedirectToAction("Index", "Login", new { area = "Admin" });
                return; // Dừng tiếp tục thực hiện các hành động khác
            }

            base.OnActionExecuting(context);
        }

        // Phương thức thiết lập thông báo tạm thời (TempData)
        public void SetAlert(string msg, string type)
        {
            TempData["AlertMessage"] = msg;

            if (type == "success")
            {
                TempData["Type"] = "alert-success";
            }
            else if (type == "warning")
            {
                TempData["Type"] = "alert-warning";
            }
            else if (type == "error")
            {
                TempData["Type"] = "alert-error";
            }
        }
    }
}
