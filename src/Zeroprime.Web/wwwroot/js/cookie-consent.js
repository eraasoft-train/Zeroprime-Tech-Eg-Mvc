// ZeroPrime Technologies - cookie consent.
// Real-app behavior: choice persisted to localStorage + a first-party cookie,
// banner fades in ~3s after page load (never on first paint), Escape/close
// dismiss for the session only. Honors prefers-reduced-motion via CSS.
(function () {
    'use strict';

    var CONSENT_KEY = 'zp-consent';           // localStorage + cookie name (mirror)
    var DISMISS_KEY = 'zp-consent-dismissed'; // sessionStorage - dismiss for this session
    var COOKIE_NAME = 'zp_consent';           // first-party cookie mirror
    var SHOW_DELAY_MS = 3000;                 // appears 3s after page load, not before
    var COOKIE_MAX_AGE = 365 * 24 * 60 * 60;  // 1 year

    function readCookie(name) {
        var prefix = name + '=';
        var parts = document.cookie ? document.cookie.split('; ') : [];
        for (var i = 0; i < parts.length; i++) {
            if (parts[i].indexOf(prefix) === 0) {
                return decodeURIComponent(parts[i].substring(prefix.length));
            }
        }
        return null;
    }

    function writeCookie(name, value, maxAgeSeconds) {
        document.cookie =
            name + '=' + encodeURIComponent(value) +
            '; Max-Age=' + maxAgeSeconds +
            '; Path=/; SameSite=Lax';
    }

    function getChoice() {
        try {
            var stored = window.localStorage.getItem(CONSENT_KEY);
            if (stored === 'accepted' || stored === 'essentials') return stored;
        } catch (_) { /* localStorage unavailable (private mode) - fall back to cookie */ }
        var cookie = readCookie(COOKIE_NAME);
        return cookie === 'accepted' || cookie === 'essentials' ? cookie : null;
    }

    function persistChoice(value) {
        try { window.localStorage.setItem(CONSENT_KEY, value); } catch (_) { /* non-fatal */ }
        writeCookie(COOKIE_NAME, value, COOKIE_MAX_AGE);
        try { window.sessionStorage.setItem(DISMISS_KEY, '1'); } catch (_) { /* non-fatal */ }
    }

    function dismissForSession() {
        try { window.sessionStorage.setItem(DISMISS_KEY, '1'); } catch (_) { /* non-fatal */ }
    }

    function sessionDismissed() {
        try { return window.sessionStorage.getItem(DISMISS_KEY) === '1'; } catch (_) { return false; }
    }

    document.addEventListener('DOMContentLoaded', function () {
        var banner = document.getElementById('zpCookieConsent');
        if (!banner) return;

        // Already chose, or dismissed this session: banner never appears.
        if (getChoice() || sessionDismissed()) {
            banner.remove();
            return;
        }

        var timer = window.setTimeout(function () {
            banner.classList.add('is-visible');
            banner.setAttribute('aria-hidden', 'false');
            document.body.classList.add('zp-consent-visible');
        }, SHOW_DELAY_MS);

        function hide() {
            window.clearTimeout(timer);
            document.body.classList.remove('zp-consent-visible');
            banner.classList.add('leaving');
            window.setTimeout(function () {
                if (banner.parentNode) banner.parentNode.removeChild(banner);
            }, 450);
        }

        function choose(value) {
            persistChoice(value);
            hide();
        }

        var accept = document.getElementById('zpConsentAccept');
        var essentials = document.getElementById('zpConsentEssentials');
        var close = document.getElementById('zpConsentClose');

        if (accept) accept.addEventListener('click', function () { choose('accepted'); });
        if (essentials) essentials.addEventListener('click', function () { choose('essentials'); });
        if (close) close.addEventListener('click', function () { dismissForSession(); hide(); });

        document.addEventListener('keydown', function onKeydown(event) {
            if (event.key === 'Escape') {
                dismissForSession();
                hide();
                document.removeEventListener('keydown', onKeydown);
            }
        });
    });

    // Minimal consent API for future non-essential features (analytics, etc.).
    // Currently unused - the site sets no non-essential cookies or storage.
    window.ZeroPrimeConsent = {
        get: getChoice,
        accepted: function () { return getChoice() === 'accepted'; },
        essentials: function () { return getChoice() === 'essentials'; }
    };
})();
