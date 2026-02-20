document.addEventListener('DOMContentLoaded', function () {
    // Pagination functionality
    const blogCards = document.querySelectorAll('.blog-card');
    const prevBtn = document.getElementById('prevBtn');
    const nextBtn = document.getElementById('nextBtn');
    const currentPageElement = document.getElementById('currentPage');

    let currentPage = 1;
    const cardsPerPage = 12;
    const totalPages = Math.ceil(blogCards.length / cardsPerPage);

    // Show page function
    function showPage(page) {
        // Hide all cards
        blogCards.forEach(card => {
            card.style.display = 'none';
        });

        // Show cards for current page
        const startIndex = (page - 1) * cardsPerPage;
        const endIndex = startIndex + cardsPerPage;

        for (let i = startIndex; i < endIndex && i < blogCards.length; i++) {
            blogCards[i].style.display = 'block';
        }

        // Update page info
        currentPageElement.textContent = page;

        // Update button states
        prevBtn.disabled = page === 1;
        nextBtn.disabled = page === totalPages;
    }

    // Initial page load
    showPage(currentPage);

    // Next button click
    nextBtn.addEventListener('click', function () {
        if (currentPage < totalPages) {
            currentPage++;
            showPage(currentPage);
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }
    });

    // Previous button click
    prevBtn.addEventListener('click', function () {
        if (currentPage > 1) {
            currentPage--;
            showPage(currentPage);
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }
    });

    // Modal functionality
    const learnMoreBtns = document.querySelectorAll('.learn-more');
    const modals = document.querySelectorAll('.modal-overlay');
    const modalCloseBtns = document.querySelectorAll('.modal-close');
    const backToBlogLinks = document.querySelectorAll('.back-to-blog');

    // Open modal
    learnMoreBtns.forEach(btn => {
        btn.addEventListener('click', function () {
            const modalId = this.getAttribute('data-modal');
            document.getElementById(modalId).classList.add('active');
            document.body.style.overflow = 'hidden';
        });
    });

    // Close modal
    function closeModal() {
        modals.forEach(modal => {
            modal.classList.remove('active');
        });
        document.body.style.overflow = 'auto';
    }

    modalCloseBtns.forEach(btn => {
        btn.addEventListener('click', closeModal);
    });

    backToBlogLinks.forEach(link => {
        link.addEventListener('click', closeModal);
    });

    // Close when clicking outside modal content
    modals.forEach(modal => {
        modal.addEventListener('click', function (e) {
            if (e.target === this) {
                closeModal();
            }
        });
    });

    // Close with ESC key
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            closeModal();
        }
    });
});
document.getElementById('mobile-menu').addEventListener('click', function () {
    document.querySelector('.nav-links').classList.toggle('show');
});
// Close menu when clicking outside
document.addEventListener('click', function (e) {
    const navLinks = document.querySelector('.nav-links');
    const menuToggle = document.getElementById('mobile-menu');

    if (!navLinks.contains(e.target) && e.target !== menuToggle) {
        navLinks.classList.remove('show');
    }
});