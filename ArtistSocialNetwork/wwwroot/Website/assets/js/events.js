$(document).ready(function () {
    let currentEventId;

    // Xử lý sự kiện chỉnh sửa sự kiện
    $('.editPost').on('click', function () {
        currentEventId = $(this).data('id');

        // Lấy dữ liệu sự kiện bằng AJAX
        $.ajax({
            url: '/Events/GetEventById',
            type: 'GET',
            data: { id: currentEventId },
            success: function (data) {
                console.log('Data from server:', data);

                if (data) {
                    $('#editEventId').val(data.idEvent);
                    $('#editTitle').val(data.title);
                    $('#editDescription').val(data.description);
                    $('#editNumberOfPeople').val(data.numberOfPeople);

                    // Format the datetime for input
                    let startDate = new Date(data.startDate);
                    let endDate = new Date(data.endDate);

                    // Convert to the 'YYYY-MM-DDTHH:mm' format required by datetime-local input
                    $('#editStartDate').val(startDate.toISOString().slice(0, 16));
                    $('#editEndDate').val(endDate.toISOString().slice(0, 16));

                    // Xóa các hình ảnh cũ
                    $('#editImagePreview').empty();
                    $('#editNewImagePreview').empty();

                    // Hiển thị hình ảnh hiện tại
                    if (data.images && data.images.length > 0) {
                        data.images.forEach(img => {
                            if (img.urlDocument && img.idDcIf) {
                                const imgDiv = $('<div>').addClass('position-relative m-1');
                                const image = $('<img>')
                                    .attr('src', img.urlDocument)
                                    .addClass('img-thumbnail')
                                    .css({ width: '100px', height: '100px', objectFit: 'cover' });
                                const removeButton = $('<button>')
                                    .addClass('btn btn-danger btn-sm remove-image')
                                    .html('&times;')
                                    .on('click', function () {
                                        let deletedImages = $('#deletedImages').val();
                                        $('#deletedImages').val(deletedImages ? deletedImages + ',' + img.idDcIf : img.idDcIf);
                                        $(this).parent().remove();
                                    });

                                imgDiv.append(image).append(removeButton);
                                $('#editImagePreview').append(imgDiv);
                            } else {
                                console.error('Image URL or ID is undefined for image:', img);
                            }
                        });
                    } else {
                        console.error('No images found for this event.');
                    }

                    $('#editModal').modal('show');
                } else {
                    alert('Không tìm thấy dữ liệu sự kiện.');
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('AJAX error:', textStatus, errorThrown);
                alert('Error retrieving event data.');
            }
        });
    });

    // Handle form submission for event editing
    $('#editForm').on('submit', function (e) {
        e.preventDefault();

        // Create a new FormData object
        var formData = new FormData(this);

        // Append image files to the FormData object
        var files = $('#editImageUpload')[0].files;
        for (var i = 0; i < files.length; i++) {
            formData.append('ImageFiles', files[i]);
        }

        // Append deleted image IDs to the FormData object
        var deletedImages = $('#deletedImages').val();
        if (deletedImages) {
            formData.append('DeletedImages', deletedImages);
        }

        // Make AJAX POST request with FormData
        $.ajax({
            url: '/Events/EditEvent',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    showSuccessEdit();
                    $('#editModal').modal('hide');
                    setTimeout(function () {
                        location.reload();
                    }, 2000);
                } else {
                    console.error('Error in response:', response.message);
                    if (response.errors) {
                        console.error('Validation errors:', response.errors.join(', '));
                    }
                    showWarningToast();
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('AJAX error:', textStatus, errorThrown);
                showWarningToast();
            }
        });
    });

    // Handle event deletion
    $('.deletePost').on('click', function () {
        currentEventId = $(this).data('id');
        $('#deleteModal').modal('show');
    });

    $('#confirmDelete').on('click', function () {
        $.ajax({
            url: '/Events/DeleteEvent',
            type: 'POST',
            data: { id: currentEventId },
            success: function (response) {
                if (response.success) {
                    $('#deleteModal').modal('hide');
                    showSuccessDelete();
                    setTimeout(function () {
                        location.reload();
                    }, 2000);
                } else {
                    alert(response.message);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('AJAX error:', textStatus, errorThrown);
                alert('Lỗi khi xóa sự kiện.');
            }
        });
    });

    // Handle image preview for editing
    $('#editImageUpload').on('change', function (event) {
        const previewContainer = $('#editNewImagePreview');
        previewContainer.empty();

        Array.from(event.target.files).forEach(file => {
            const reader = new FileReader();
            reader.onload = function (e) {
                const imageDiv = $('<div>').addClass('position-relative m-1');
                const img = $('<img>')
                    .attr('src', e.target.result)
                    .addClass('img-thumbnail')
                    .css({ width: '100px', height: '100px', objectFit: 'cover' });

                const removeButton = $('<button>')
                    .addClass('btn btn-danger btn-sm remove-image')
                    .html('&times;')
                    .on('click', function () {
                        $(this).parent().remove();
                    });

                imageDiv.append(img).append(removeButton);
                previewContainer.append(imageDiv);
            };
            reader.readAsDataURL(file);
        });
    });

    // Handle image preview for event creation
    $('#imageUpload').on('change', function (event) {
        const previewContainer = $('#imagePreview');
        previewContainer.empty();

        Array.from(event.target.files).forEach(file => {
            const reader = new FileReader();
            reader.onload = function (e) {
                const imageDiv = $('<div>').addClass('position-relative m-1');
                const img = $('<img>')
                    .attr('src', e.target.result)
                    .addClass('img-thumbnail')
                    .css({ width: '100px', height: '100px', objectFit: 'cover' });

                const removeButton = $('<button>')
                    .addClass('btn btn-danger btn-sm remove-image')
                    .html('&times;')
                    .on('click', function () {
                        $(this).parent().remove();
                    });

                imageDiv.append(img).append(removeButton);
                previewContainer.append(imageDiv);
            };
            reader.readAsDataURL(file);
        });
    });
    // Đăng ký tham gia sự kiện
    $(document).on('click', '.register-btn', function () {
        var eventId = $(this).data('event-id'); // Lấy ID sự kiện từ thuộc tính data-event-id của nút

        $.ajax({
            type: "POST",
            url: '/Events/RegisterEvent', // Gọi trực tiếp URL của phương thức trong controller
            data: { IdEvent: eventId },
            success: function (response) {
                if (response.success) {
                    showSuccessEventDki(); // Hiển thị thông báo đăng ký thành công
                    setTimeout(function () {
                        location.reload(); // Tải lại trang sau khi thông báo được hiển thị trong 2 giây
                    }, 2000); // Đặt thời gian chờ 2 giây (2000ms) trước khi tải lại trang
                } else {
                    alert(response.message); // Hiển thị thông báo lỗi nếu có
                }
            },
            error: function () {
                showWarningEventDki(); // Hiển thị thông báo lỗi khi đăng ký
            }
        });
    });



    // Hiển thị modal xác nhận khi hủy sự kiện
    $(document).on('click', '.cancel-registration-btn', function () {
        var eventId = $(this).data('event-id');
        $('#confirmCancelRegistrationBtn').data('event-id', eventId);
        $('#cancelRegistrationModal').modal('show');
    });

    $('#confirmCancelRegistrationBtn').on('click', function () {
        var eventId = $(this).data('event-id');

        $.ajax({
            type: "POST",
            url: '/Events/CancelRegistration', // Gọi trực tiếp URL của phương thức trong controller
            data: { IdEvent: eventId },
            success: function (response) {
                if (response.success) {
                    $('#cancelRegistrationModal').modal('hide');
                    showSuccessHuyEventDki(); // Hiển thị thông báo hủy thành công
                    setTimeout(function () {
                        location.reload(); // Tải lại trang sau khi thông báo được hiển thị trong 2 giây
                    }, 2000); // Đặt thời gian chờ 2 giây (2000ms) trước khi tải lại trang
                } else {
                    alert(response.message); // Hiển thị thông báo lỗi nếu có
                }
            },
            error: function () {
                showWarningHuyEventDki(); // Hiển thị thông báo lỗi khi hủy đăng ký
            }
        });
    });


    // Modal logic for image viewing
    const modal = document.querySelector("#imageModal");
    const modalImage = document.querySelector("#modalImage");
    const closeModal = document.querySelector(".close");
    const prevImage = document.querySelector("#prevImage");
    const nextImage = document.querySelector("#nextImage");
    let currentImageIndex = 0;

    function openModal(images, index) {
        modal.style.display = "flex";
        modalImage.src = images[index];
        currentImageIndex = index;

        prevImage.onclick = () => navigateImages(images, -1);
        nextImage.onclick = () => navigateImages(images, 1);
    }

    function navigateImages(images, direction) {
        currentImageIndex = (currentImageIndex + direction + images.length) % images.length;
        modalImage.src = images[currentImageIndex];
    }

    closeModal.onclick = () => {
        modal.style.display = "none";
    };

    // Open image modal when an image is clicked
    document.querySelectorAll(".post-images img").forEach((img, index) => {
        img.addEventListener("click", () => {
            const images = Array.from(img.closest(".post-images").querySelectorAll("img")).map(i => i.src);
            openModal(images, index);
        });
    });
});
document.addEventListener("DOMContentLoaded", function () {
    const posts = document.querySelectorAll('.post-content-wrapper');

    posts.forEach(post => {
        const content = post.querySelector('.post-content');
        const showMoreBtn = post.querySelector('.show-more');
        const maxHeight = 100; 
        // Đặt trạng thái ban đầu: nếu nội dung lớn hơn chiều cao tối đa thì hiển thị nút "Xem thêm"
        if (content.scrollHeight > maxHeight) {
            showMoreBtn.style.display = 'inline'; // Show the button if content is too long
            content.style.maxHeight = `${maxHeight}px`;
            content.style.overflow = 'hidden'; // Prevent content overflow
        }

        //  xử lý sự kiện cho nút "Xem thêm"/"Ẩn giảm"
        showMoreBtn.addEventListener('click', function () {
            if (content.classList.contains('expanded')) {
                content.classList.remove('expanded');
                content.style.maxHeight = `${maxHeight}px`;
                content.style.overflow = 'hidden';
                showMoreBtn.textContent = 'Xem thêm';
            } else {
                content.classList.add('expanded');
                content.style.maxHeight = content.scrollHeight + 'px'; // Tự động điều chỉnh theo chiều cao đầy đủ
                content.style.overflow = 'visible';
                showMoreBtn.textContent = 'Ẩn bớt';
            }
        });
    });
});