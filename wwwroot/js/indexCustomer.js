/* ===============================
   DROPDOWN TOGGLE
=================================*/

function toggleDropdown(type) {

    const categories = document.getElementById("categories-dropdown");
    const user = document.getElementById("user-dropdown");

    if (type === "categories") {
        categories.classList.toggle("show");
        user.classList.remove("show");
    }

    if (type === "user") {
        user.classList.toggle("show");
        categories.classList.remove("show");
    }
}


/* ===============================
   CLOSE DROPDOWN OUTSIDE CLICK
=================================*/

document.addEventListener("click", function (event) {

    const isToggle = event.target.closest(".dropdown-toggle");

    if (!isToggle) {
        document.querySelectorAll(".dropdown-content, .user-dropdown-content")
            .forEach(drop => drop.classList.remove("show"));
    }

});


/* ===============================
   CATEGORY SCROLL BUTTON
=================================*/

function scrollCategories(direction) {
    const carousel = document.querySelector(".category-carousel");

    if (!carousel) return;

    carousel.scrollBy({
        left: direction * 220,
        behavior: "smooth"
    });
}


/* ===============================
   SIMPLE TRACK CAROUSEL
=================================*/

document.querySelectorAll(".carousel-container").forEach(container => {

    const track = container.querySelector(".carousel-track");
    const nextBtn = container.querySelector(".right");
    const prevBtn = container.querySelector(".left");

    if (!track || !nextBtn || !prevBtn) return;

    let position = 0;
    const item = container.querySelector(".item");

    if (!item) return;

    const itemWidth = item.offsetWidth + 15;

    nextBtn.addEventListener("click", () => {
        const maxScroll = track.scrollWidth - container.querySelector(".carousel-window").offsetWidth;

        position += itemWidth * 4;

        if (position > maxScroll) position = maxScroll;

        track.style.transform = `translateX(-${position}px)`;
    });

    prevBtn.addEventListener("click", () => {

        position -= itemWidth * 4;

        if (position < 0) position = 0;

        track.style.transform = `translateX(-${position}px)`;
    });

});


/* ===============================
   NX HOME CAROUSEL
=================================*/

document.querySelectorAll(".nx-carousel").forEach(carousel => {

    const track = carousel.querySelector(".nx-carousel-track");
    const leftBtn = carousel.querySelector(".nx-arrow-btn.left");
    const rightBtn = carousel.querySelector(".nx-arrow-btn.right");

    if (!track || !leftBtn || !rightBtn) return;

    let position = 0;
    const step = 250;

    rightBtn.addEventListener("click", () => {
        const maxScroll = track.scrollWidth - carousel.offsetWidth;

        position -= step;

        if (Math.abs(position) > maxScroll) {
            position = -maxScroll;
        }

        track.style.transform = `translateX(${position}px)`;
    });

    leftBtn.addEventListener("click", () => {

        position += step;

        if (position > 0) position = 0;

        track.style.transform = `translateX(${position}px)`;
    });

});
