var common = {
    init: function () {
        this.registerEvent();
    },
    registerEvent: function () {
        // Xử lý sự kiện click nút Xóa
        $(document).on("click", ".delete-link", function (e) {
            e.preventDefault();
            var id = $(this).data("id");
            var confirmMessage = $(this).data("confirm");

            $("#confirmMessage").text(confirmMessage);
            $("#confirmDelete").data("id", id); // Lưu trữ ID để sử dụng trong xử lý xóa

            $("#confirmModal").modal("show");
        });

        // Xử lý sự kiện xác nhận xóa
        $(document).on("click", "#confirmDelete", function (e) {
            e.preventDefault();
            var id = $(this).data("id");

            $.ajax({
                url: "/Admin/TypeOfArtworks/DeleteId/" + id,
                dataType: "json",
                type: "POST",
                contentType: "application/json;charset=UTF-8",
                success: function (res) {
                    if (res.status === true) {
                        window.location.href = '/Admin/TypeOfArtworks';
                    }
                },
                error: function (errormessage) {
                    alert(errormessage.responseText);
                }
            });
        });

        // Xử lý sự kiện thay đổi trạng thái Active
        $(document).on('click', '.btn-active', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');

            $.ajax({
                url: "/Admin/TypeOfArtworks/ChangeStatus",
                data: { id: id },
                datatype: "json",
                type: "POST",
                success: function (response) {
                    if (response.status === true) {
                        btn.text('Bật').removeClass("btn-danger").addClass('btn-primary');
                    } else {
                        btn.text('Tắt').removeClass('btn-primary').addClass('btn-danger');
                    }
                },
                error: function (err) {
                    console.error("Error changing status: ", err.responseText);
                }
            });
        });

        // Tự động ẩn thông báo sau 3 giây
        $('#alertBox').delay(3000).slideUp(500);
    }
};

$(document).ready(function () {
    common.init();
});
