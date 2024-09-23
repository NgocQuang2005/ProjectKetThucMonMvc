var common = {
    init: function () {
        common.registerEvent();
    },
    registerEvent: function () {
        $(document).on("click", ".delete-link", function (e) {
            e.preventDefault();
            var id = $(this).data("id");
            var confirmMessage = $(this).data("confirm");

            $("#confirmMessage").text(confirmMessage);
            $("#confirmDelete").data("id", id); // Lưu trữ ID để sử dụng trong xử lý xóa

            $("#confirmModal").modal("show");
        });
        $(document).on("click", "#confirmDelete", function (e) {
            e.preventDefault();
            var id = $(this).data("id");
            //if (confirm($(this).data("confirm"))) {
            $.ajax({
                url: "/Admin/Artworks/DeleteId/" + id,
                dataType: "json",
                type: "POST",
                contentType: "application/json;charset=UTF-8",
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = '/Admin/Artworks';
                        //$("#getCodeModal").modal("toggle");
                    }
                },
                error: function (errormessage) {
                    alert(errormessage.responseText);
                }
            });
            //}
        });


        $(function () {
            $('#alertBox').removeClass('hide');
            $('#alertBox').delay(3000).slideUp(500);
        })

        $('.btn-active').off('click').on('click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            $.ajax({
                url: "/Admin/Artworks/ChangeActive",
                data: { id: id },
                datatype: "json",
                type: "POST",
                success: function (response) {
                    if (response.status == true) {
                        console.log(`Status is true`);
                        btn.text('Bật');
                        btn.removeClass("btn-danger");
                        btn.addClass('btn-primary');
                    }
                    else {
                        btn.text('Tắt');
                        btn.removeClass('btn-primary').addClass('btn-danger');
                    }
                }
            });
        });
    }
}
common.init();
function callIndexAction(select) {
    $("#loadingModal").modal('show');
    setTimeout(function () {
        $("#form-search").submit();
    }, 1000);
}