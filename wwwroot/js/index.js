document.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.getElementById('sidebar');
    const hoverArea = document.getElementById('sidebarHoverArea');
    const toggleBtn = document.getElementById('toggleSidebar');
    const overlay = document.getElementById('overlay');
    const mainContent = document.getElementById('mainContent');

    // Show sidebar on hover
    hoverArea.addEventListener('mouseenter', function () {
        sidebar.classList.add('open');
    });

    // Hide sidebar when mouse leaves sidebar area
    sidebar.addEventListener('mouseleave', function () {
        sidebar.classList.remove('open');
    });

    // Keep toggle button functionality for mobile
    toggleBtn.addEventListener('click', function () {
        sidebar.classList.toggle('open');
        overlay.classList.toggle('active');
        mainContent.classList.toggle('shifted');
    });

    overlay.addEventListener('click', function () {
        sidebar.classList.remove('open');
        overlay.classList.remove('active');
        mainContent.classList.remove('shifted');
    });

    // Simulate login functionality
    const loginBtn = document.querySelector('.admin-btn:first-child');
    loginBtn.addEventListener('click', function () {
        alert('Admin login functionality would go here.');
    });

    // Simulate logout functionality
    const logoutBtn = document.querySelector('.admin-btn:last-child');
    logoutBtn.addEventListener('click', function () {
        alert('Logout functionality would go here.');
    });
});
//=============================================
const track = document.getElementById("track");
const next = document.getElementById("next");
const prev = document.getElementById("prev");

const itemWidth = 210; // item + margin
const visibleItems = 5; // ایک وقت میں 5 دکھیں
let position = 0;

next.addEventListener("click", () => {
    const maxScroll = track.scrollWidth - (itemWidth * visibleItems);
    position += itemWidth * visibleItems;
    if (position > maxScroll) position = maxScroll;
    track.style.transform = `translateX(-${position}px)`;
});

prev.addEventListener("click", () => {
    position -= itemWidth * visibleItems;
    if (position < 0) position = 0;
    track.style.transform = `translateX(-${position}px)`;
});

