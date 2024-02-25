using WebsiteTemplate.Web.Features.Contact.Models;
using WebsiteTemplate.Web.Features.Contact.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;

namespace BeyondMassagesApp.Pages;

[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(EmailResult))]
[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(InvalidResponseResult))]
public class IndexModel : PageModel
{
    [BindProperty]
    public EmailModel EmailDetails { get; set; } = new EmailModel();

    private readonly EmailService _emailService;

    public IndexModel(EmailService emailService)
    {
        _emailService = emailService;
    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var invalidResponseResult = new InvalidResponseResult
            {
                IsSent = false,
                ErrorMessages = ModelState.SelectMany(x => x.Value!.Errors)
                    .Select(e => e.ErrorMessage)
            };

            return new UnprocessableEntityObjectResult(invalidResponseResult);
        }

        SanitizeFormInput();
        var emailResult = await _emailService.SendEmailAsync(EmailDetails, HttpContext.RequestAborted);

        return new CreatedResult(nameof(OnPostAsync), emailResult);
    }

    private void SanitizeFormInput()
    {
        EmailDetails.Name = WebUtility.HtmlEncode(EmailDetails.Name);
        EmailDetails.EmailAddress = WebUtility.HtmlEncode(EmailDetails.EmailAddress);
        EmailDetails.Subject = WebUtility.HtmlEncode(EmailDetails.Subject);
        EmailDetails.Message = WebUtility.HtmlEncode(EmailDetails.Message);
    }
}