// ZeroPrime Technologies - global site interactions.
(function () {
    'use strict';

    // Sticky header shadow on scroll
    var header = document.getElementById('zpHeader');
    if (header) {
        var onScroll = function () {
            header.classList.toggle('is-scrolled', window.scrollY > 24);
        };
        onScroll();
        window.addEventListener('scroll', onScroll, { passive: true });
    }

    // Mobile navigation toggle
    var toggle = document.getElementById('zpNavToggle');
    var panel = document.getElementById('zpNavPanel');
    if (toggle && panel) {
        toggle.addEventListener('click', function () {
            var isOpen = panel.classList.toggle('is-open');
            toggle.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
        });

        // Close the mobile menu after navigating
        panel.addEventListener('click', function (event) {
            if (event.target.closest('a') && window.innerWidth < 992) {
                panel.classList.remove('is-open');
                toggle.setAttribute('aria-expanded', 'false');
            }
        });
    }
})();
