// Contact form UX: disable the submit button while the request is in flight
// and show a spinner. Server-side validation remains the source of truth.
(function () {
    'use strict';

    var form = document.getElementById('contactForm');
    var button = document.getElementById('contactSubmit');
    if (!form || !button) {
        return;
    }

    form.addEventListener('submit', function () {
        // If jQuery unobtrusive validation is present and the form is invalid, do not lock the UI.
        if (window.jQuery && window.jQuery(form).data('validator')) {
            var validator = window.jQuery(form).data('validator');
            if (typeof validator.form === 'function' && !validator.form()) {
                return;
            }
        }

        button.disabled = true;
        var spinner = button.querySelector('.spinner-border');
        var label = button.querySelector('.submit-label');
        if (spinner) { spinner.classList.remove('d-none'); }
        if (label) { label.textContent = 'Sending…'; }
    });
})();
