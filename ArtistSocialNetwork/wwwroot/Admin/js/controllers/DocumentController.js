var common = {
    init: function () {
        common.registerEvent();
    },
    registerEvent: function () {
        // Sự kiện xóa với xác nhận
        $(document).on("click", ".delete-link", function (e) {
            e.preventDefault();
            var id = $(this).data("id"); // Lấy ID từ phần tử được click
            var confirmMessage = $(this).data("confirm"); // Lấy thông báo xác nhận

            // Hiển thị thông báo xác nhận
            $("#confirmMessage").text(confirmMessage);
            // Lưu trữ ID để xử lý khi người dùng xác nhận xóa
            $("#confirmDelete").data("id", id);

            // Hiển thị modal xác nhận xóa
            $("#confirmModal").modal("show");
        });

        // Xử lý khi người dùng xác nhận xóa
        $(document).on("click", "#confirmDelete", function (e) {
            e.preventDefault();
            var id = $(this).data("id"); // Lấy ID từ nút xác nhận

            $.ajax({
                url: "/Admin/DocumentInfoes/DeleteId/" + id, // Đảm bảo URL đúng với phương thức trong controller
                dataType: "json",
                type: "POST",
                contentType: "application/json;charset=UTF-8",
                success: function (res) {
                    if (res.status == true) {
                        // Nếu xóa thành công, chuyển hướng về trang danh sách
                        window.location.href = '/Admin/DocumentInfoes';
                    } else {
                        // Hiển thị thông báo lỗi nếu có
                        alert(res.message);
                    }
                },
                error: function (errormessage) {
                    alert("Lỗi khi xóa: " + errormessage.responseText);
                }
            });
        });

        // Tự động ẩn thông báo sau 3 giây
        $(function () {
            $('#alertBox').removeClass('hide');
            $('#alertBox').delay(3000).slideUp(500);
        });

        // Sự kiện cập nhật trạng thái Bật/Tắt
        $(document).on('click', '.btn-active', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id'); // Lấy ID của bản ghi

            $.ajax({
                url: "/Admin/DocumentInfoes/ChangeActive", // Đảm bảo URL đúng với phương thức trong controller
                data: { id: id },
                dataType: "json",
                type: "POST",
                success: function (response) {
                    if (response.status == true) {
                        // Cập nhật nút thành "Bật"
                        btn.text('Bật');
                        btn.removeClass("btn-danger").addClass('btn-primary');
                    } else {
                        // Cập nhật nút thành "Tắt"
                        btn.text('Tắt');
                        btn.removeClass('btn-primary').addClass('btn-danger');
                    }
                },
                error: function (errormessage) {
                    alert("Lỗi khi thay đổi trạng thái: " + errormessage.responseText);
                }
            });
        });
    }
}

// Khởi tạo sự kiện chung
common.init();

// Gọi hàm khi thay đổi trong dropdown để lọc dữ liệu
function callIndexAction(select) {
    $("#loadingModal").modal('show');
    setTimeout(function () {
        $("#form-search").submit(); // Gửi form tìm kiếm sau 1 giây
    }, 1000);
}
