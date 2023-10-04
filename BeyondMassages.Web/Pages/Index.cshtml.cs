using BeyondMassages.Web.Features.Contact.Models;
using BeyondMassages.Web.Features.Contact.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;

namespace BeyondMassagesApp.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public EmailModel EmailDetails { get; set; } = new EmailModel();

    private readonly IEmailService _emailService;

    public IndexModel(IEmailService emailService)
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
            var errors = ModelState.SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage)).ToList();
            return new JsonResult(new { isSent = false, message = errors });
        }

        PrepareMessage();
        var emailResult = await _emailService.SendEmailAsync(EmailDetails, HttpContext.RequestAborted);

        return new JsonResult(new { isSent = emailResult.IsSent, message = emailResult.Message });
    }

    private void PrepareMessage()
    {
        EmailDetails.Name = WebUtility.HtmlEncode(EmailDetails.Name);
        EmailDetails.EmailAddress = WebUtility.HtmlEncode(EmailDetails.EmailAddress);
        EmailDetails.Subject = WebUtility.HtmlEncode(EmailDetails.Subject);
        EmailDetails.Message = WebUtility.HtmlEncode(EmailDetails.Message)
            .ReplaceLineEndings("<br>");
    }
}