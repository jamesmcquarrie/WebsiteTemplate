using BeyondMassages.Web.Features.Contact.Exceptions;
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
    private readonly ILogger<IndexModel> _logger;
    public bool ShowAlert => TempData.ContainsKey("ShowAlert") && (bool)TempData["ShowAlert"]!;
    public bool IsSent => TempData.ContainsKey("EmailIsSent") && (bool)TempData["EmailIsSent"]!;
    public bool IsCancelled => TempData.ContainsKey("EmailIsCancelled") && (bool)TempData["EmailIsCancelled"]!;

    public IndexModel(IEmailService emailService, ILogger<IndexModel> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        TempData["ShowAlert"] = true;

        try
        {
            PrepareMessage();
            await _emailService.SendEmailAsync(EmailDetails, HttpContext.RequestAborted);

            TempData["EmailIsSent"] = true;
            TempData["AlertMessage"] = "Email has been sent successfully!";
        }

        catch (OperationCanceledException)
        {
            TempData["EmailIsCancelled"] = true;
            TempData["AlertMessage"] = "Email sending operation cancelled";
        }

        catch (Exception ex) when (ex is ArgumentException || ex is UserFriendlyException)
        {
            TempData["AlertMessage"] = ex.Message;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            TempData["AlertMessage"] = "An unexpected error occurred. Please try again later.";
        }

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