using BeyondMassages.Web.Features.Contact.Models;
using BeyondMassages.Web.Features.Contact.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text.Json;

namespace BeyondMassagesApp.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public EmailModel EmailDetails { get; set; } = new EmailModel();
    [BindProperty]
    public EmailResult EmailResult { get; set; } = new EmailResult();
    public bool ShowAlert => TempData.ContainsKey("ShowAlert") && (bool)TempData["ShowAlert"]!;

    private readonly IEmailService _emailService;

    public IndexModel(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public void OnGet()
    {
        if (TempData.ContainsKey("EmailResult"))
        {
            EmailResult = JsonSerializer.Deserialize<EmailResult>((string)TempData["EmailResult"]!)!;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        TempData["ShowAlert"] = true;

        PrepareMessage();
        EmailResult = await _emailService.SendEmailAsync(EmailDetails, HttpContext.RequestAborted);
        TempData["EmailResult"] = JsonSerializer.Serialize(EmailResult);

        return RedirectToPage();
    }

    private void PrepareMessage()
    {
        EmailDetails.Name = WebUtility.HtmlEncode(EmailDetails.Name);
        EmailDetails.EmailAddress = WebUtility.HtmlEncode(EmailDetails.EmailAddress);
        EmailDetails.Subject = WebUtility.HtmlEncode(EmailDetails.Subject);
        EmailDetails.Message = WebUtility.HtmlEncode(EmailDetails.Message)
            .Replace(Environment.NewLine, "<br>");
    }
}