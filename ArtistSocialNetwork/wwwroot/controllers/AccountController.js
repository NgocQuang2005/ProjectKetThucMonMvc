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
                url: "/Admin/Accounts/DeleteId/" + id,
                dataType: "json",
                type: "POST",
                contentType: "application/json;charset=UTF-8",
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = '/Admin/Accounts';
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

        
        $(function () {
            $('#alertBox').removeClass('hide');
            $('#alertBox').delay(3000).slideUp(500);
        })
    }
}
common.init();
function callIndexAction(select) {
    $("#loadingModal").modal('show');
    setTimeout(function () {
        $("#form-search").submit();
    }, 1000);
}