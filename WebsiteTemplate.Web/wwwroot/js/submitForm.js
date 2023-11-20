(function (app) {
    'use-strict';
    const pageItems = {};

    app.indexStartup = () => {
        document.addEventListener('DOMContentLoaded', () => {
            pageItems.form = document.getElementById('contactForm');
            pageItems.submitButton = document.getElementById('submitButton');

            pageItems.form.addEventListener('submit', handleFormSubmission);
        })
    }

    function handleFormSubmission(e) {
        e.preventDefault();

        if (pageItems.form.checkValidity()) {

            changeSubmitButton();
            const formData = new URLSearchParams(new FormData(pageItems.form));

            submitEmail(formData);

        } else {
            pageItems.form.reportValidity();
        };
    }

    function submitEmail(formData) {
        fetch('', {
            method: pageItems.form.getAttribute('method'),
            body: formData,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data.isSent) {
                    displayAlert(true, data.message);
                } else {
                    displayAlert(false, data.message);
                }
            })
            .catch(() => {
                const errorMessage = 'There was an unexpected error while sending the email. Please try again later';
                displayAlert(false, errorMessage);
            })
            .finally(() => {
                pageItems.form.reset();
                resetSubmitButton();
            });
    }

    function changeSubmitButton() {
        pageItems.submitButton.replaceChildren();

        const fragment = new DocumentFragment();

        const textNode = document.createTextNode('Sending... ');
        fragment.appendChild(textNode);

        const spinnerSpan = document.createElement('span');
        spinnerSpan.className = 'spinner-border spinner-border-sm';
        spinnerSpan.setAttribute('role', 'status');
        fragment.appendChild(spinnerSpan);

        pageItems.submitButton.appendChild(fragment);
        pageItems.submitButton.disabled = true;
    }

    function resetSubmitButton() {
        pageItems.submitButton.replaceChildren();

        const fragment = new DocumentFragment();

        const textNode = document.createTextNode('Submit');
        fragment.appendChild(textNode);

        pageItems.submitButton.appendChild(fragment);
        pageItems.submitButton.disabled = false
    }

    function displayAlert(isSuccess, message) {
        const fragment = new DocumentFragment();
        const alert = document.createElement('div');
        const icon = document.createElement('i');
        const textNode = document.createTextNode(` ${message}`);
        const closeButton = document.createElement('button');

        closeButton.type = 'button';
        closeButton.className = 'btn-close';
        closeButton.dataset.bsDismiss = 'alert';

        if (isSuccess) {
            icon.className = 'bi bi-check-circle-fill';
            alert.className = `alert alert-success alert-dismissible fade show mt-3 mb-0`;
        } else {
            icon.className = 'bi bi-x-circle-fill';
            alert.className = `alert alert-danger alert-dismissible fade show mt-3 mb-0`;
        }

        fragment.append(icon, textNode, closeButton);
        alert.appendChild(fragment);

        pageItems.submitButton.insertAdjacentElement('afterend', alert);
    }
})(window.app = window.app || {});