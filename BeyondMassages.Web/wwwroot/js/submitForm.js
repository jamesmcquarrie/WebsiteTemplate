document.addEventListener('DOMContentLoaded', function () {
    var submitButton = document.getElementById('submitButton');
    var form = document.querySelector('form');

    form.addEventListener('submit', function (e) {
        e.preventDefault(); 

        if (form.checkValidity()) {  

            submitButton.innerHTML = 'Sending... <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>';  
            submitButton.setAttribute('disabled', 'disabled');

            var formData = new URLSearchParams(new FormData(form));  // Convert form data to URLSearchParams

            fetch('', {
                method: form.getAttribute('method'),
                body: formData,
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            })
            .then(response => response.json())
            .then(data => {
                submitButton.innerHTML = 'Submit';
                submitButton.removeAttribute('disabled');

                if (data.success) {
                    submitButton.insertAdjacentHTML('afterend',
                        '<div class="alert alert-success alert-dismissible fade show mt-3 mb-0">' +
                        data.message +
                        '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
                        '</div>'
                    );
                    form.reset();
                } else {
                    submitButton.insertAdjacentHTML('afterend',
                        '<div class="alert alert-danger alert-dismissible fade show mt-3 mb-0">' +
                        data.message +
                        '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
                        '</div>'
                    );
                }
            })
            .catch(error => {
                submitButton.innerHTML = 'Submit';
                submitButton.removeAttribute('disabled');

                submitButton.insertAdjacentHTML('afterend',
                    '<div class="alert alert-danger alert-dismissible fade show mt-3 mb-0">' +
                    'There was an unexpected error while sending the email. Please try again later' +
                    '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
                    '</div>'
                );
            });
        } else {
            form.reportValidity();  
        }
    });
});
