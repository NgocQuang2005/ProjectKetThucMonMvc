$(document).ready(function () {
    let currentArtworkId = null;

    $('.editPost').on('click', function () {
        currentArtworkId = $(this).data('id');

        // Lấy dữ liệu bài viết bằng AJAX
        $.ajax({
            url: '/Artworks/GetArtworkById',
            type: 'GET',
            data: { id: currentArtworkId },
            success: function (data) {
                if (data) {
                    // Cập nhật thông tin bài viết
                    $('#editArtworkId').val(data.idArtwork);
                    $('#editTitle').val(data.title);
                    $('#editDescription').val(data.description);
                    $('#editArtworkType').val(data.idTypeOfArtwork);

                    // Xóa các hình ảnh cũ và làm mới danh sách hình ảnh
                    $('#editImagePreview').empty();
                    $('#editNewImagePreview').empty();
                    $('#deletedImages').val(''); // Đảm bảo deletedImages trống khi bắt đầu

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
                            }
                        });
                    }

                    $('#editModal').modal('show');
                } else {
                    alert('Không tìm thấy dữ liệu bài viết.');
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Lỗi khi tải dữ liệu bài viết.');
            }
        });
    });

    // Xử lý sự kiện submit chỉnh sửa bài viết
    $('#editForm').on('submit', function (e) {
        e.preventDefault();

        var formData = new FormData(this);

        // Chỉ append các ảnh mới từ input 'editImageUpload'
        var files = $('#editImageUpload')[0].files;
        for (var i = 0; i < files.length; i++) {
            formData.append('ImageFiles', files[i]);
        }

        // Gửi danh sách các ảnh đã bị xóa
        var deletedImages = $('#deletedImages').val();
        if (deletedImages) {
            formData.append('DeletedImages', deletedImages);
        }

        // Gửi yêu cầu AJAX để cập nhật bài viết
        $.ajax({
            url: '/Artworks/EditArtwork',
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

    // Xử lý sự kiện xóa bài viết
    $('.deletePost').on('click', function () {
        currentArtworkId = $(this).data('id');
        $('#deleteModal').modal('show');
    });

    $('#confirmDelete').on('click', function () {
        if (currentArtworkId) {
            $.ajax({
                url: '/Artworks/DeleteArtwork',
                type: 'POST',
                data: { id: currentArtworkId },
                success: function (response) {
                    if (response.success) {
                        $('#deleteModal').modal('hide');
                        showSuccessDelete();
                        setTimeout(function () {
                            location.reload();
                        }, 2000);
                    } else {
                        alert(response.message); // Thông báo lỗi từ server
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.error('AJAX error: ', jqXHR.responseText); // Hiển thị chi tiết lỗi
                    console.log('Lỗi khi xóa bài viết. Lỗi từ máy chủ: ' + jqXHR.responseText);
                    showWarningToast();
                }
            });
        } else {
                showWarningToast();
        }
    });


    // Xử lý sự kiện thả tim
    $('.like-btn').on('click', function () {
        var artworkId = $(this).data('id');

        $.ajax({
            url: '/Artworks/ToggleLike',
            type: 'POST',
            data: { artworkId: artworkId },
            success: function (response) {
                if (response.success) {
                    $('#like-count-' + artworkId).text(response.likeCount + ' Lượt Thích');
                    var likeButton = $('.like-btn[data-id="' + artworkId + '"] i');
                    likeButton.toggleClass('far fas text-danger');
                } else {
                    alert(response.message);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('AJAX error:', textStatus, errorThrown);
                alert('Lỗi khi thực hiện thả tim.');
            }
        });
    });

    // Xem trước hình ảnh trong edit modal
    $('#editImageUpload').on('change', function (event) {
        const previewContainer = $('#editNewImagePreview');
        previewContainer.empty();

        Array.from(event.target.files).forEach(file => {
            const reader = new FileReader();
            reader.onload = function (e) {
                const imageDiv = $('<div>').addClass('position-relative m-1');
                const img = $('<img>').attr('src', e.target.result).addClass('img-thumbnail').css({ width: '100px', height: '100px', objectFit: 'cover' });
                const removeButton = $('<button>').addClass('btn btn-danger btn-sm remove-image').html('&times;').on('click', function () {
                    $(this).parent().remove();
                });

                imageDiv.append(img).append(removeButton);
                previewContainer.append(imageDiv);
            };
            reader.readAsDataURL(file);
        });
    });

    $('#imageUpload').off('change').on('change', function (event) {
        const previewContainer = $('#imagePreview');
        previewContainer.empty();

        Array.from(event.target.files).forEach(file => {
            const reader = new FileReader();
            reader.onload = function (e) {
                const imgDiv = $('<div>').addClass('position-relative m-1');
                const img = $('<img>').attr('src', e.target.result).addClass('img-thumbnail').css({ width: '100px', height: '100px', objectFit: 'cover' });
                const removeButton = $('<button>').addClass('btn btn-danger btn-sm remove-image').html('&times;').on('click', function () {
                    $(this).parent().remove();
                });

                imgDiv.append(img).append(removeButton);
                previewContainer.append(imgDiv);
            };
            reader.readAsDataURL(file);
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

        const maxHeight = 100; // Adjust this height to the appropriate size for 4 lines

        // Set the initial state: if the content is larger than the max height, show the "Xem thêm" button
        if (content.scrollHeight > maxHeight) {
            showMoreBtn.style.display = 'inline'; // Show the button if content is too long
            content.style.maxHeight = `${maxHeight}px`;
            content.style.overflow = 'hidden'; // Prevent content overflow
        }

        // Event listener for the "Xem thêm" / "Ẩn bớt" button
        showMoreBtn.addEventListener('click', function () {
            if (content.classList.contains('expanded')) {
                content.classList.remove('expanded');
                content.style.maxHeight = `${maxHeight}px`;
                content.style.overflow = 'hidden';
                showMoreBtn.textContent = 'Xem thêm';
            } else {
                content.classList.add('expanded');
                content.style.maxHeight = content.scrollHeight + 'px'; // Dynamically adjust to full height
                content.style.overflow = 'visible';
                showMoreBtn.textContent = 'Ẩn bớt';
            }
        });
    });
});