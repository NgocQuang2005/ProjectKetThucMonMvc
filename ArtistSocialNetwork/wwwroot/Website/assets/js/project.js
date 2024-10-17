$(document).ready(function () {
    let currentProjectId;

    // Helper function to add image previews
    function addImagePreview(container, src, id = null) {
        const imageDiv = $('<div>').addClass('position-relative m-1');
        const img = $('<img>')
            .attr('src', src)  // Đường dẫn của hình ảnh
            .addClass('img-thumbnail')
            .css({ width: '100px', height: '100px', objectFit: 'cover' });

        const removeButton = $('<button>')
            .addClass('btn btn-danger btn-sm remove-image')
            .html('&times;')
            .on('click', function () {
                if (id) {
                    let deletedImages = $('#deletedImages').val();
                    $('#deletedImages').val(deletedImages ? deletedImages + ',' + id : id);
                }
                $(this).parent().remove();  // Xóa hình ảnh khi nhấn nút xóa
            });

        imageDiv.append(img).append(removeButton);
        container.append(imageDiv);  // Thêm hình ảnh vào container
    }

    // Handle image preview for project creation
    $('#imageUpload').on('change', function (event) {
        const previewContainer = $('#imagePreview').empty();
        Array.from(event.target.files).forEach(file => {
            const reader = new FileReader();
            reader.onload = function (e) {
                addImagePreview(previewContainer, e.target.result);
            };
            reader.readAsDataURL(file);
        });
    });

    // Handle image preview for project editing
    $('#editImageUpload').on('change', function (event) {
        const previewContainer = $('#editNewImagePreview').empty();
        Array.from(event.target.files).forEach(file => {
            const reader = new FileReader();
            reader.onload = function (e) {
                addImagePreview(previewContainer, e.target.result);
            };
            reader.readAsDataURL(file);
        });
    });

    // Open modal when image is clicked for viewing images in a gallery modal
    $('.post-images img').on('click', function () {
        const images = $(this).closest('.post-images').find('img').map((_, img) => $(img).attr('src')).get();
        openModal(images, $(this).index());
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

    // Show more/less functionality for project descriptions
    const maxHeight = 100;
    $('.post-content-wrapper').each(function () {
        const content = $(this).find('.post-content');
        const showMoreBtn = $(this).find('.show-more');

        if (content.prop('scrollHeight') > maxHeight) {
            content.css({ maxHeight, overflow: 'hidden' });
            showMoreBtn.show().on('click', function () {
                if (content.hasClass('expanded')) {
                    content.removeClass('expanded').css({ maxHeight, overflow: 'hidden' });
                    $(this).text('Xem thêm');
                } else {
                    content.addClass('expanded').css({ maxHeight: content.prop('scrollHeight'), overflow: 'visible' });
                    $(this).text('Ẩn bớt');
                }
            });
        }
    });

    // Handle project registration
    $(document).on('click', '.register-btn', function () {
        const projectId = $(this).data('project-id');

        $.ajax({
            type: 'POST',
            url: '/Projects/RegisterProject',
            data: { IdProject: projectId },
            success: function (response) {
                if (response.success) {
                    setTimeout(() => location.reload(), 2000);
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert('Lỗi khi đăng ký tham gia dự án.');
            }
        });
    });

    // Xử lý khi mở modal chỉnh sửa dự án
    $('.editPost').on('click', function () {
        currentProjectId = $(this).data('id');

        // Gửi AJAX để lấy dữ liệu dự án
        $.ajax({
            url: '/Projects/GetProjectById',
            type: 'GET',
            data: { id: currentProjectId },
            success: function (data) {
                if (data) {
                    // Điền dữ liệu vào form chỉnh sửa
                    $('#editProjectId').val(data.idProject);
                    $('#editTitle').val(data.title);
                    $('#editDescription').val(data.description);

                    // Định dạng lại ngày bắt đầu và kết thúc
                    let startDate = new Date(data.startDate);
                    let endDate = new Date(data.endDate);
                    $('#editStartDate').val(startDate.toISOString().slice(0, 16));
                    $('#editEndDate').val(endDate.toISOString().slice(0, 16));

                    // Xóa ảnh cũ trước khi hiển thị
                    const previewContainer = $('#editImagePreview').empty();

                    // Hiển thị ảnh nếu có
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
                            }
                        });
                    }

                    // Xử lý chọn người tham gia
                    const selectedParticipants = data.participants; // Mảng chứa các ID người tham gia đã được chọn

                    // Duyệt qua tất cả checkbox và kiểm tra xem có trùng với người tham gia không
                    $('#editParticipantsList input[type="checkbox"]').each(function () {
                        const participantId = parseInt($(this).val()); // Lấy ID của người tham gia từ checkbox
                        if (selectedParticipants.includes(participantId)) {
                            $(this).prop('checked', true); // Nếu trùng ID, tích checkbox
                        } else {
                            $(this).prop('checked', false); // Nếu không, bỏ tích
                        }
                    });

                    // Hiển thị modal chỉnh sửa
                    $('#editModal').modal('show');
                } else {
                    alert('Không tìm thấy dữ liệu dự án.');
                }
            },
            error: function () {
                alert('Lỗi khi lấy dữ liệu dự án.');
            }
        });
    });

    // Xử lý form chỉnh sửa dự án
    $('#editForm').on('submit', function (e) {
        e.preventDefault();
        const formData = new FormData(this);

        // Thêm các file ảnh mới
        const files = $('#editImageUpload')[0].files;
        for (let i = 0; i < files.length; i++) {
            formData.append('ImageFiles', files[i]);
        }

        // Thêm các hình ảnh bị xóa
        const deletedImages = $('#deletedImages').val();
        if (deletedImages) {
            formData.append('DeletedImages', deletedImages);
        }

        $.ajax({
            url: '/Projects/EditProject',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    showSuccessEdit();
                    $('#editModal').modal('hide');
                    setTimeout(() => location.reload(), 2000);
                } else {
                    console.error('Error in response:', response.message);
                    if (response.errors) {
                        console.error('Validation errors:', response.errors.join(', '));
                    }
                    showWarningToast();
                }
            },
            error: function () {
                showWarningToast();
            }
        });
    });

    // Xử lý chọn người tham gia (ẩn/hiện list người tham gia)
    $('#toggleCreateParticipantsBtn').on('click', function () {
        $('#createParticipantsListContainer').toggle();
    });

    $('#toggleEditParticipantsBtn').on('click', function () {
        $('#editParticipantsListContainer').toggle();
    });
    $(document).on('click', '.deletePost', function (e) {
        e.preventDefault();  // Ngăn không cho hành vi mặc định của thẻ <a> xảy ra
        currentProjectId = $(this).data('id');
        $('#deleteModal').modal('show');  // Hiển thị modal xóa
    });

    // Xử lý xóa dự án
    $('#confirmDelete').on('click', function () {
        $.ajax({
            url: '/Projects/DeleteProject',
            type: 'POST',
            data: { id: currentProjectId },
            success: function (response) {
                if (response.success) {
                    $('#deleteModal').modal('hide');
                    showSuccessDelete();
                    setTimeout(() => location.reload(), 2000);
                } else {
                    showWarningToast();
                }
            },
            error: function () {
                showWarningToast();
            }
        });
    });
});
document.addEventListener("DOMContentLoaded", function () {
    const posts = document.querySelectorAll('.post-content-wrapper');

    posts.forEach(post => {
        const content = post.querySelector('.post-content');
        const showMoreBtn = post.querySelector('.show-more');

        const maxHeight = 100; // Điều chỉnh độ cao này cho phù hợp với 4 dòng
         // Đặt trạng thái ban đầu: nếu nội dung lớn hơn chiều cao tối đa thì hiển thị nút "Xem thêm"

        if (content.scrollHeight > maxHeight) {
            showMoreBtn.style.display = 'inline'; 
            content.style.maxHeight = `${maxHeight}px`;
            content.style.overflow = 'hidden'; 
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