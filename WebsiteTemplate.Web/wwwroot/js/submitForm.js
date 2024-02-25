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

        changeSubmitButton();
        const formData = new URLSearchParams(new FormData(pageItems.form));

        submitEmail(formData);
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
                    displaySuccessAlert(data.message);
                    pageItems.form.reset();
                } else {
                    displayErrorsAlert(data.errorMessages);
                }
            })
            .catch(() => {
                const errorMessage = ['There was an unexpected error while sending the email. Please try again later'];
                displayErrorAlert(errorMessage);
            })
            .finally(() => {
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

    function displaySuccessAlert(message) {
        const fragment = new DocumentFragment();
        const alert = document.createElement('div');
        const icon = document.createElement('i');
        const textNode = document.createTextNode(message);
        const closeButton = document.createElement('button');

        icon.className = 'bi bi-check-circle-fill me-2';
        alert.className = `alert alert-success alert-dismissible fade show mt-3 mb-0`;

        closeButton.type = 'button';
        closeButton.className = 'btn-close';
        closeButton.dataset.bsDismiss = 'alert';

        fragment.append(icon, textNode, closeButton);
        alert.appendChild(fragment);

        pageItems.submitButton.insertAdjacentElement('afterend', alert);
    }

    function displayErrorsAlert(errors) {
        const fragment = new DocumentFragment();
        const errorFragment = new DocumentFragment();
        const alert = document.createElement('div');
        const closeButton = document.createElement('button');

        alert.className = 'alert alert-danger alert-dismissible fade show mt-3 mb-0';

        closeButton.type = 'button';
        closeButton.className = 'btn-close';
        closeButton.dataset.bsDismiss = 'alert';

        for (const error of errors) {
            const icon = document.createElement('i');
            icon.className = 'bi bi-x-circle-fill me-2';

            const textNode = document.createTextNode(error);
            const messagebreak = document.createElement('br');
            errorFragment.append(icon, textNode, messagebreak)
        }

        fragment.append(errorFragment, closeButton);
        alert.appendChild(fragment);

        pageItems.submitButton.insertAdjacentElement('afterend', alert);
    }
})(window.app = window.app || {});