// ZeroPrime Technologies - theme bootstrap.
// Loaded synchronously in <head> so the saved theme applies before first paint.
(function () {
    'use strict';
    try {
        var stored = window.localStorage.getItem('zp-theme');
        var prefersDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
        var theme = stored === 'dark' || stored === 'light'
            ? stored
            : (prefersDark ? 'dark' : 'light');
        document.documentElement.setAttribute('data-theme', theme);
    } catch (_) {
        document.documentElement.setAttribute('data-theme', 'light');
    }
})();
