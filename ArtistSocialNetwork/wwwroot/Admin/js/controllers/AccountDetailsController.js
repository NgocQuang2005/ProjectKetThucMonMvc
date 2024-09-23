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
                url: "/Admin/AccountDetails/DeleteId/" + id,
                dataType: "json",
                type: "POST",
                contentType: "application/json;charset=UTF-8",
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = '/Admin/AccountDetails';
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
        
        $(document).ready(function () {
            $('.btn-active').on('click', function (e) {
                e.preventDefault();
                var btn = $(this);
                var id = btn.data('id');
                console.log(`Clicked button with ID: ${id}`);

                $.ajax({
                    url: "/Admin/AccountDetails/ChangeActive",
                    data: { id: id },
                    dataType: "json",
                    type: "POST",
                    success: function (response) {
                        console.log(response);
                        if (response.status == true) {
                            btn.text('Bật');
                            btn.removeClass("btn-danger");
                            btn.addClass('btn-primary');
                        } else {
                            btn.text('Tắt');
                            btn.removeClass('btn-primary').addClass('btn-danger');
                        }
                    },
                    error: function (error) {
                        console.log("Error in AJAX call:", error);
                    }
                });
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