function toast({ title = "", message = "", type = "info", duration = 3000 }) {
    const main = document.getElementById("toast");
    if (main) {
        const toast = document.createElement("div");

        // Auto remove toast
        const autoRemoveId = setTimeout(function () {
            main.removeChild(toast);
        }, duration + 1000);

        // Remove toast when clicked
        toast.onclick = function (e) {
            if (e.target.closest(".toast__close")) {
                main.removeChild(toast);
                clearTimeout(autoRemoveId);
            }
        };
         
        const icons = {
            success: "fas fa-check-circle",
            //info: "fas fa-info-circle",
            warning: "fas fa-exclamation-circle",
            error: "fas fa-exclamation-circle"
        };
        const icon = icons[type];
        const delay = (duration / 1000).toFixed(2);

        toast.classList.add("toast", `toast--${type}`);
        toast.style.animation = `slideInLefts ease .3s, fadeOut linear 1s ${delay}s forwards`;

        toast.innerHTML = `
                <div class="toast__icon">
                    <i class="${icon}"></i>
                </div>
                <div class="toast__body">
                    <h3 class="toast__title">${title}</h3>
                    <p class="toast__msg">${message}</p>
                </div>
                <div class="toast__close">
                    <i class="fas fa-times"></i>
                </div>
            `;
        main.appendChild(toast);
    }
}


function showSuccessDelete() {
    toast({
        title: "Thành công!",
        message: "Bạn đã xóa bài viết thành công",
        type: "success",
        duration: 5000
    });
    console.log("deo cho m click đò")
}
function showSuccessEdit() {
    toast({
        title: "Thành công!",
        message: "Bạn đã chỉnh sửa bài viết thành công",
        type: "success",
        duration: 5000
    });
}
function showSuccessEventDki() {
    toast({
        title: "Thành công!",
        message: "Bạn đã đăng kí tham gia thành công",
        type: "success",
        duration: 5000
    });
}
function showSuccessHuyEventDki() {
    toast({
        title: "Thành công!",
        message: "Bạn đã hủy tham gia sự kiện này thành công.",
        type: "success",
        duration: 5000
    });
}
function showWarningHuyEventDki() {
    toast({
        title: "Thất Bại!",
        message: "Bạn vui lòng chờ hệ thống xác thực nhé  !!!.",
        type: "warning",
        duration: 5000
    });
    return false;
}
function showWarningEventDki() {
    toast({
        title: "Thất Bại!",
        message: "Hiện tại bạn chưa thể đăng kí , vui lòng quay lại sau  !!!.",
        type: "warning",
        duration: 5000
    });
    return false;
}
function showWarningToast() {
    toast({
        title: "Thất Bại!",
        message: "Vui lòng kiểm tra lại thông tin..  !!!.",
        type: "warning",
        duration: 5000
    });
    return false;
}