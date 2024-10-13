// Image modal logic
const modal = document.querySelector("#imageModal");
const modalImage = document.querySelector("#modalImage");
const closeModal = document.querySelector(".close");
const prevImage = document.querySelector("#prevImage");
const nextImage = document.querySelector("#nextImage");
let currentImageIndex = 0;

function openModal(images, index) {
    if (!images || images.length === 0) {
        console.error("No images found to display in modal.");
        return;
    }

    modal.style.display = "flex";
    modalImage.src = images[index] || "chuacoanh"; // Đảm bảo có dự phòng nếu hình ảnh không được xác định

    currentImageIndex = index;

    prevImage.onclick = () => navigateImages(images, -1);
    nextImage.onclick = () => navigateImages(images, 1);
}

function navigateImages(images, direction) {
    currentImageIndex = (currentImageIndex + direction + images.length) % images.length;
    modalImage.src = images[currentImageIndex] || "chuacoanh"; // Đảm bảo có dự phòng nếu hình ảnh không được xác định

}

closeModal.onclick = () => {
    modal.style.display = "none";
};

// Open image modal when an image is clicked
document.querySelectorAll(".post-images img").forEach((img, index) => {
    img.addEventListener("click", () => {
        const images = Array.from(img.closest(".post-images").querySelectorAll("img"))
            .map(i => i.src)
            .filter(src => src !== undefined && src !== ''); // Filter out invalid URLs

        if (images.length > 0) {
            openModal(images, index);
        } else {
            console.error("No valid images found in this post.");
        }
    });
});


$(document).ready(function () {
    // Handle the like button click event
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
