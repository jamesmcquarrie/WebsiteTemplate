document.addEventListener('DOMContentLoaded', function() {
    var submitButton = document.getElementById('submitButton');
    var form = document.querySelector('form');

    submitButton.addEventListener('click', function(e) {
        e.preventDefault();  // Prevent the form from submitting the traditional way
        
        if($(form).valid()) {  // Check if the form is valid
            submitButton.innerHTML = 'Sending... <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>';  // Change button text and add spinner
            submitButton.setAttribute('disabled', 'disabled');  // Disable the button to prevent multiple submissions
            form.submit();  // Submit the form using JavaScript
        }
    });
});
