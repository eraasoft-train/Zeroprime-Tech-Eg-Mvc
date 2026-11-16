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

    // Scroll to top button
    var scrollTop = document.getElementById('zpScrollTop');
    if (scrollTop) {
        var toggleScrollTop = function () {
            scrollTop.classList.toggle('is-visible', window.scrollY > 320);
        };
        toggleScrollTop();
        window.addEventListener('scroll', toggleScrollTop, { passive: true });

        scrollTop.addEventListener('click', function () {
            var prefersReduced = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
            window.scrollTo({ top: 0, left: 0, behavior: prefersReduced ? 'auto' : 'smooth' });
        });
    }

    // Dark / light theme toggle (initial theme is applied by theme.js before paint)
    var themeToggle = document.getElementById('zpThemeToggle');
    if (themeToggle) {
        var root = document.documentElement;
        var syncPressed = function () {
            themeToggle.setAttribute('aria-pressed', root.getAttribute('data-theme') === 'dark' ? 'true' : 'false');
        };
        syncPressed();

        themeToggle.addEventListener('click', function () {
            var next = root.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
            root.setAttribute('data-theme', next);
            try { window.localStorage.setItem('zp-theme', next); } catch (_) { /* private mode */ }
            syncPressed();
        });
    }
})();
