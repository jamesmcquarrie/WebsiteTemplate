//document.addEventListener('DOMContentLoaded', function() {
//    var submitButton = document.getElementById('submitButton');
//    var form = document.querySelector('form');

//    submitButton.addEventListener('click', function(e) {
//        e.preventDefault();  // Prevent the form from submitting the traditional way

//        if($(form).valid()) {  // Check if the form is valid
//            submitButton.innerHTML = 'Sending... <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>';  // Change button text and add spinner
//            submitButton.setAttribute('disabled', 'disabled');  // Disable the button to prevent multiple submissions
//            form.submit();  // Submit the form using JavaScript
//        }
//    });
//});

//$(function () {
//    $('#submit').on('click', function (evt) {
//        evt.preventDefault();
//        $.post('', $('form').serialize(), function () {
//            alert('Posted using jQuery');
//        });
//    });
//});

$(document).ready(function () {
    var submitButton = $('#submitButton');
    var form = $('form');

    submitButton.on('click', function (e) {
        e.preventDefault();

        if ($(form).valid()) {
            submitButton.html('Sending... <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>');  // Change button text and add spinner
            submitButton.attr('disabled', 'disabled');

            var formData = $(form).serialize();

            $.ajax({
                url: form.attr('action'),
                method: form.attr('method'),
                data: formData,
                contentType: "application/x-www-form-urlencoded",
                dataType: "json",
                success: function (response) {
                    submitButton.html('Submit');
                    submitButton.removeAttr('disabled');

                    if (response.success) {
                        $("#submitButton").after(
                            '<div class="alert alert-success alert-dismissible fade show mt-3 mb-0">' +
                            response.message +
                            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
                            '</div>'
                        );
                        form[0].reset();
                    } else {
                        $("#submitButton").after(
                            '<div class="alert alert-danger alert-dismissible fade show mt-3 mb-0">' +
                            response.message +
                            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
                            '</div>'
                        );
                    }
                },
                error: function () {
                    submitButton.html('Submit');
                    submitButton.removeAttr('disabled');

                    $("#submitButton").after(
                        '<div class="alert alert-danger alert-dismissible fade show mt-3 mb-0">' +
                        'There was an unexpected error while sending the email. Please try again later' +
                        '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
                        '</div>'
                    );
                }
            });
        }
    });
});